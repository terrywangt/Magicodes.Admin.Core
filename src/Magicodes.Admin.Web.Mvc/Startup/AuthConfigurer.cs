using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Magicodes.Admin.Web.Authentication.JwtBearer;

namespace Magicodes.Admin.Web.Startup
{
    public static class AuthConfigurer
    {
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configuration">The configuration.</param>
        public static void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseIdentity();

            if (bool.Parse(configuration["IdentityServer:IsEnabled"]))
            {
                app.UseIdentityServer();
            }

            if (bool.Parse(configuration["Authentication:OpenId:IsEnabled"]))
            {
                app.UseOpenIdConnectAuthentication(CreateOpenIdConnectAuthOptions(configuration));
            }

            if (bool.Parse(configuration["Authentication:Microsoft:IsEnabled"]))
            {
                app.UseMicrosoftAccountAuthentication(CreateMicrosoftAuthOptions(configuration));
            }

            if (bool.Parse(configuration["Authentication:Google:IsEnabled"]))
            {
                app.UseGoogleAuthentication(CreateGoogleAuthOptions(configuration));
            }

            if (bool.Parse(configuration["Authentication:Twitter:IsEnabled"]))
            {
                app.UseTwitterAuthentication(CreateTwitterAuthOptions(configuration));
            }

            if (bool.Parse(configuration["Authentication:Facebook:IsEnabled"]))
            {
                app.UseFacebookAuthentication(CreateFacebookAuthOptions(configuration));
            }

            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                app.UseJwtBearerAuthentication(CreateJwtBearerAuthenticationOptions(app));
            }

            if (bool.Parse(configuration["IdentityServer:IsEnabled"]))
            {
                app.UseIdentityServerAuthentication(
                    new IdentityServerAuthenticationOptions
                    {
                        Authority = configuration["App:WebSiteRootAddress"],
                        RequireHttpsMetadata = false,
                        AutomaticAuthenticate = true,
                        AutomaticChallenge = true
                    });
            }
        }

        private static OpenIdConnectOptions CreateOpenIdConnectAuthOptions(IConfiguration configuration)
        {
            var options = new OpenIdConnectOptions
            {
                ClientId = configuration["Authentication:OpenId:ClientId"],
                Authority = configuration["Authentication:OpenId:Authority"],
                PostLogoutRedirectUri = configuration["App:WebSiteRootAddress"] + "Account/Logout",
                ResponseType = OpenIdConnectResponseType.IdToken
            };

            var clientSecret = configuration["Authentication:OpenId:ClientSecret"];
            if (!clientSecret.IsNullOrEmpty())
            {
                options.ClientSecret = clientSecret;
            }

            return options;
        }

        private static MicrosoftAccountOptions CreateMicrosoftAuthOptions(IConfiguration configuration)
        {
            return new MicrosoftAccountOptions
            {
                ClientId = configuration["Authentication:Microsoft:ConsumerKey"],
                ClientSecret = configuration["Authentication:Microsoft:ConsumerSecret"]
            };
        }

        private static GoogleOptions CreateGoogleAuthOptions(IConfiguration configuration)
        {
            return new GoogleOptions
            {
                ClientId = configuration["Authentication:Google:ClientId"],
                ClientSecret = configuration["Authentication:Google:ClientSecret"]
            };
        }

        private static TwitterOptions CreateTwitterAuthOptions(IConfiguration configuration)
        {
            return new TwitterOptions
            {
                ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"],
                ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"],
                RetrieveUserDetails = true
            };
        }

        private static FacebookOptions CreateFacebookAuthOptions(IConfiguration configuration)
        {
            var options = new FacebookOptions
            {
                AppId = configuration["Authentication:Facebook:AppId"],
                AppSecret = configuration["Authentication:Facebook:AppSecret"]
            };

            options.Scope.Add("email");
            options.Scope.Add("public_profile");

            return options;
        }

        private static JwtBearerOptions CreateJwtBearerAuthenticationOptions(IApplicationBuilder app)
        {
            var tokenAuthConfig = app.ApplicationServices.GetRequiredService<TokenAuthConfiguration>();

            return new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = tokenAuthConfig.SecurityKey,

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = tokenAuthConfig.Issuer,

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = tokenAuthConfig.Audience,

                    // Validate the token expiry
                    ValidateLifetime = true,

                    // If you want to allow a certain amount of clock drift, set that here
                    ClockSkew = TimeSpan.Zero
                }
            };
        }
    }
}
