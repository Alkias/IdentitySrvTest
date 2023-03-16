using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdS3test;
using IdS3test.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace IdS3test
{
    /// <summary>
    /// Adding StartupIdentityServer is configured in the startup class.
    /// Here we provide information about the clients, users, scopes,
    /// the signing certificate and some other configuration options.
    /// In production you should load the signing certificate from the
    /// Windows certificate store or some other secured source.
    /// In this sample we simply added it to the project as a file
    /// (you can download a test certificate from here.
    /// Add it to the project and set its build action to Copy to output.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app) {
            //The long claim names come from Microsoft’s JWT handler trying to map some claim types to .NET’s
            //ClaimTypes class types. You can turn off this behavior with the following line of code (in Startup).

            //This also means that you need to adjust the configuration for anti-CSRF protection to the new unique sub claim type:
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.Map("/identity",
                idsrvApp => {
                    idsrvApp.UseIdentityServer(new IdentityServerOptions {
                        SiteName = "Embedded IdentityServer",
                        SigningCertificate = LoadCertificate(),

                        Factory = new IdentityServerServiceFactory()
                            .UseInMemoryUsers(Users.Get())
                            .UseInMemoryClients(Clients.Get())
                            //.UseInMemoryScopes(StandardScopes.All)

                            //Also change the factory in Startup to use the new Scopes:
                            .UseInMemoryScopes(Scopes.Get())
                    });
                });

            //Configure the cookie middleware in StartUp.cs with its default values:
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = "Cookies"
            });

            //Point the OpenID Connect middleware (also in Startup.cs) to our
            //embedded version of IdentityServer and use the previously configured client configuration:
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions {
                Authority = "https://localhost:44356/identity",
                ClientId = "mvc",

                //By default the OIDC middleware asks for two scopes: openid and profile - this is why
                //IdentityServer includes the subject and name claims. Now we add a request to the roles scope:
                Scope = "openid profile roles",

                RedirectUri = "https://localhost:44356/",
                ResponseType = "id_token",

                SignInAsAuthenticationType = "Cookies",

                UseTokenLifetime = false,

                //The OIDC middleware has a notification that you can use to do claims transformation -
                //the resulting claims will be stored in the cookie:
                Notifications = new OpenIdConnectAuthenticationNotifications {
                    SecurityTokenValidated = n => {
                        var id = n.AuthenticationTicket.Identity;

                        // we want to keep first name, last name, subject and roles
                        var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        var roles = id.FindAll(Constants.ClaimTypes.Role);

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        nid.AddClaim(givenName);
                        nid.AddClaim(familyName);
                        nid.AddClaim(sub);
                        nid.AddClaims(roles);

                        // add some other app specific claim
                        nid.AddClaim(new Claim("app_specific", "some data"));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);

                        return Task.FromResult(0);
                    }
                }
            });
        }

        private X509Certificate2 LoadCertificate() {
            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory),
                "idsrv3test");
        }
    }
}