using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;

namespace IdS3test.Models
{
    /// <summary>
    /// Configuring IdentityServer - Users
    /// 
    /// Next we will add some users to IdentityServer -
    /// again this can be accomplished by providing a simple C# class.
    /// You can retrieve user information from any data store and
    /// we provide out of the box support for ASP.NET Identity and MembershipReboot.
    /// </summary>
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "bob",
                    Password = "secret",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Bob"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Smith"),

                        new Claim(Constants.ClaimTypes.Role, "Geek"),
                        new Claim(Constants.ClaimTypes.Role, "Foo")
                    }
                }
            };
        }
    }


}