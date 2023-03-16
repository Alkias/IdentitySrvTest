using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdS3test.Models
{
    /// <summary>
    /// Configuring IdentityServer - Clients
    /// 
    /// IdentityServer needs some information about the clients it is going to support,
    /// this can be simply supplied using a Client object:
    /// </summary>
    public class Clients
    {
        public static IEnumerable<Client> Get() {
            return new[] {
                new Client {
                    Enabled = true,
                    ClientName = "MVC Client",
                    ClientId = "mvc",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string> {
                        "https://localhost:44356/"
                    },

                    AllowAccessToAllScopes = true
                }
            };
        }
    }
}