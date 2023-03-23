using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace StarterTemplate.GraphQL.Sockets
{
    public class CustomSocketSessionInterceptor : DefaultSocketSessionInterceptor
    {
        // This is the key to the auth token in the HTTP Context
        public static readonly string HTTP_CONTEXT_WEBSOCKET_AUTH_KEY = "websocket-auth-token";

        // This is the key that apollo uses in the connection init request
        public static readonly string WEBOCKET_PAYLOAD_AUTH_KEY = "Authorization";

        public override async ValueTask OnCloseAsync(ISocketConnection connection, CancellationToken cancellationToken)
        {
            var user = connection.HttpContext.User;
            // Add logic here to run when a user disconnects from a socket
            await base.OnCloseAsync(connection, cancellationToken);
        }

        public override async ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message,
                    CancellationToken cancellationToken)
        {
            HttpContext context = connection.HttpContext;

            //using browser client during dev, we wont any other time use query auth
            var queryAuth = context.Request.Query[WEBOCKET_PAYLOAD_AUTH_KEY];

            if (!message.Payload!.TryGetValue(WEBOCKET_PAYLOAD_AUTH_KEY, out var token))
            {
                if (queryAuth.Any())
                {
                    token = queryAuth.FirstOrDefault();
                }
            }

            if (!(token is string stringToken)) return (ConnectionStatus.Reject("No JWT token found in connection init payload"));
            context.Items[HTTP_CONTEXT_WEBSOCKET_AUTH_KEY] = stringToken;
            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });

            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

            foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(context, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                {
                    return (ConnectionStatus.Reject("Request Handling Error"));
                }
            }

            var defaultAuthenticate = await schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate == null) return (ConnectionStatus.Reject("No default authentication scheme found"));
            var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
            if (result?.Principal == null) return (ConnectionStatus.Reject("Could not authenticate user from JWT token. No principal user found."));
            context.User = result.Principal;
            var email = context.User.Claims.First(x => x.Type.Equals("email", StringComparison.OrdinalIgnoreCase));
            // add logic here that you want run when a user is authenticated.
            return (ConnectionStatus.Accept());
        }

        public override ValueTask OnRequestAsync(ISocketConnection connection, IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            var user = connection.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
                return base.OnRequestAsync(connection, requestBuilder, cancellationToken);
            // Here you can add global properties to every websocket request,
            // the below snippet adds the users email address extracted from JWT claim and adds it to every request.
            var email = user.Claims.First(x => x.Type.Equals("email", StringComparison.OrdinalIgnoreCase));
            requestBuilder.TryAddProperty("currentUserEmail", email.Value);
            return base.OnRequestAsync(connection, requestBuilder, cancellationToken);
        }
    }
}