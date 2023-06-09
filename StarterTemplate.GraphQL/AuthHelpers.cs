﻿using System;
using System.Linq;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;

namespace StarterTemplate.GraphQL
{
    public static class AccessToken
    {
        /// <summary>
        /// This is the key to the auth token in the HTTP Context.
        /// </summary>
        public static readonly string ContextKey = "websocket-auth-token";

        /// <summary>
        /// This is the key that apollo uses in the connection init request.
        /// </summary>
        public static readonly string PayloadKey = "Authorization";
    }

    public class ApiAuthenticationSchemes // Don't forget to rename
    {
        public static readonly string DefaultScheme = "Bearer";
        public static readonly string IntrospectionScheme = "introspection";
        public static readonly string WebsocketScheme = "Websockets";

        public static Func<HttpContext, string> ForwardWebsocket()
        {
            return Select;

            static string Select(HttpContext context)
            {
                if (context.Items.TryGetValue(AccessToken.ContextKey, out var boxedToken) &&
                    boxedToken is string token && context.WebSockets.IsWebSocketRequest)
                {
                    return IntrospectionScheme;
                }
                else if (context.Request.Headers.TryGetValue("Upgrade", out var value))
                {
                    if (value.Count > 0 && value[0] is string protocol && protocol.Equals("websocket", StringComparison.OrdinalIgnoreCase))
                        return WebsocketScheme; // No authentication before switching protocols.
                }

                // Common workflow with authentication by header.

                var (scheme, credential) = GetSchemeAndCredential(context);

                return scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) ? IntrospectionScheme : null;
            }
        }

        /// <summary>
        /// Extracts scheme and credential from Authorization header (if present)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static (string, string) GetSchemeAndCredential(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(header))
            {
                return ("", "");
            }

            var parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return ("", "");
            }

            return (parts[0], parts[1]);
        }

        public static Func<HttpRequest, string> TokenRetriever()
        {
            return Retrieve;

            static string Retrieve(HttpRequest request)
            {
                if (request.HttpContext.Items.TryGetValue(AccessToken.ContextKey, out object boxedToken) &&
                    boxedToken is string token)
                {
                    return token;
                }

                token = TokenRetrieval.FromAuthorizationHeader()(request) ?? TokenRetrieval.FromQueryString()(request);

                return token;
            }
        }
    }
}