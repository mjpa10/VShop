using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace VShop.IdentityServer.Configuration;

public class IdentityConfiguration
{
    public const string Admin = "Admin";
    public const string Client = "Client";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Email(),
        new IdentityResources.Profile()
    };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
    {
        //vshop é aplicacao web que vai acessar o identity server para obter o token
        new ApiScope("vshop", "VShop Server"),
        new ApiScope(name:"read", "Read data."),
        new ApiScope(name:"write", "Write data."),
        new ApiScope(name:"delete", "Delete data.")
    };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            //cliente Generico
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("dsfdss#fe1234FD".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials, //precisa das credenciais do usuario para obter o token
                AllowedScopes = { "read", "write", "profile" }
            },
            new Client
            {
                ClientId = "vshop",
                ClientSecrets = { new Secret("dsfdss#fe1234FD".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code, //via codigo
                RedirectUris= { "https://localhost:50104/signin-oidc"}, //login
                PostLogoutRedirectUris = { "https://localhost:50104/signout-callback-oidc" }, //logout
                AllowedScopes = new List<string>
                {
                   IdentityServerConstants.StandardScopes.OpenId,
                   IdentityServerConstants.StandardScopes.Profile,
                   IdentityServerConstants.StandardScopes.Email,
                    "vshop"
                }
            }
        };
}
