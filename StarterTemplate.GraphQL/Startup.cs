using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StarterTemplate.GraphQL.Examples;
using StarterTemplate.GraphQL.Logging;

namespace StarterTemplate.GraphQL
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();
            app.UseWebSockets();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/graphql", true, true);
                    return Task.CompletedTask;
                });
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = null;
                options.KnownProxies.Add(IPAddress.Parse("102.65.37.117"));
            });

            services.AddSha256DocumentHashProvider(HashFormat.Hex);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api");
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ApiAuthenticationSchemes.DefaultScheme;
                options.DefaultAuthenticateScheme = ApiAuthenticationSchemes.DefaultScheme;
            })
                .AddJwtBearer(ApiAuthenticationSchemes.WebsocketScheme, default)
                .AddJwtBearer(ApiAuthenticationSchemes.DefaultScheme, options =>
                {
                    // local identity STS
                    options.Authority = "https://localhost:7030";
                    options.Audience = "api";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidTypes = new[] { "at+jwt" }
                    };

                    options.ForwardDefaultSelector = ApiAuthenticationSchemes.ForwardWebsocket();

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            context.Token = ApiAuthenticationSchemes.TokenRetriever()(context.Request);
                            return Task.CompletedTask;
                        }
                    };
                    options.RequireHttpsMetadata = false;
                }).AddOAuth2Introspection(ApiAuthenticationSchemes.IntrospectionScheme, options =>
                {
                    options.Authority = "https://localhost:7030";
                    options.TokenRetriever = ApiAuthenticationSchemes.TokenRetriever();
                    options.ClientId = "interactive";
                    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
                    options.SkipTokensWithDots = false;
                });
            services.AddErrorFilter<GraphqlErrorFilter>();
            services.AddGraphQLServer()

                //.AddAuthorization() enable if need auth
                .AddQueryType(d => d.Name("Query"))
                .AddTypeExtension<ProductQuery>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddTypeExtension<ProductMutation>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                .AddTypeExtension<ProductSubscription>()
                .BindRuntimeType<string, StringType>()
                //.UseAutomaticPersistedQueryPipeline() enable if you need/want this
                //.AddRedisQueryStorage(provider =>
                //    provider.GetApplicationServices().GetRequiredService<IConnectionMultiplexer>().GetDatabase())
                .UseField(next => async context =>
                {
                    var hca = context.Services.GetRequiredService<IHttpContextAccessor>();
                    context.ContextData["ClaimsPrincipal"] = hca.HttpContext.User;
                    await next(context);
                })
                .ModifyRequestOptions(opt =>
                {
                    opt.IncludeExceptionDetails = true;
                })
                .AddHttpRequestInterceptor((context, executor, builder, ct) =>
                {
                    if (!context.User.Identity.IsAuthenticated) return new ValueTask();
                    var email = context.User.Claims.First(x => x.Type.Equals("email", StringComparison.OrdinalIgnoreCase));
                    builder.AddProperty(
                        "currentUserEmail",
                        email.Value);
                    return new ValueTask();
                })
                .AddInMemorySubscriptions()
                //.AddSocketSessionInterceptor(provider => new CustomSocketSessionInterceptor()) enable if you need this
                .AddDiagnosticEventListener(sp =>
                    new ConsoleQueryLogger(
                        sp.GetApplicationService<ILogger<ConsoleQueryLogger>>()
                    ));
        }

        public class GraphqlErrorFilter : IErrorFilter
        {
            public IError OnError(IError error)
            {
                return error.WithException(error.Exception);
            }
        }
    }
}