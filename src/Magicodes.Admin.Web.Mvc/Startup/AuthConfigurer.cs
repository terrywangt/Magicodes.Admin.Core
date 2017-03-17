using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Magicodes.Admin.Web.Authentication.JwtBearer;
using Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicodes.Admin.Web.Startup
{
    public static class AuthConfigurer
    {
        public const string AuthenticationScheme = "AdminAuthSchema";
        public const string ExternalAuthenticationScheme = AuthenticationScheme + "." + DefaultAuthenticationTypes.ExternalCookie;

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="configuration">The configuration.</param>
        public static void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = false,
                AuthenticationScheme = ExternalAuthenticationScheme,
                CookieName = ExternalAuthenticationScheme
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = AuthenticationScheme,
                LoginPath = new PathString("/Account/Login/"),
                AccessDeniedPath = new PathString("/Error/E403"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = false,
                AuthenticationScheme = DefaultAuthenticationTypes.TwoFactorCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
                CookieName = DefaultAuthenticationTypes.TwoFactorCookie
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = false,
                AuthenticationScheme = DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie,
                CookieName = DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie
            });

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
        }

        private static OpenIdConnectOptions CreateOpenIdConnectAuthOptions(IConfiguration configuration)
        {
            var options = new OpenIdConnectOptions
            {
                ClientId = configuration["Authentication:OpenId:ClientId"],
                Authority = configuration["Authentication:OpenId:Authority"],
                PostLogoutRedirectUri = configuration["App:WebSiteRootAddress"] + "Account/Logout",
                ResponseType = OpenIdConnectResponseType.IdToken,
                SignInScheme = ExternalAuthenticationScheme
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
                SignInScheme = ExternalAuthenticationScheme,
                ClientId = configuration["Authentication:Microsoft:ConsumerKey"],
                ClientSecret = configuration["Authentication:Microsoft:ConsumerSecret"]
            };
        }

        private static GoogleOptions CreateGoogleAuthOptions(IConfiguration configuration)
        {
            return new GoogleOptions
            {
                SignInScheme = ExternalAuthenticationScheme,
                ClientId = configuration["Authentication:Google:ClientId"],
                ClientSecret = configuration["Authentication:Google:ClientSecret"]
            };
        }

        private static TwitterOptions CreateTwitterAuthOptions(IConfiguration configuration)
        {
            return new TwitterOptions
            {
                SignInScheme = ExternalAuthenticationScheme,
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
                AppSecret = configuration["Authentication:Facebook:AppSecret"],
                SignInScheme = ExternalAuthenticationScheme
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
