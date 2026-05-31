using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VShop.IdentityServer.Data;

namespace VShop.IdentityServer.Services;

public class ProfileAppService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public ProfileAppService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        //id do usuário no IS
        string id = context.Subject.GetSubjectId();
        ApplicationUser user = await _userManager.FindByIdAsync(id);

        //cria as claimsprincipal do usuário
        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory
                                            .CreateAsync(user);

        //define uma colecao de calaims para o usuario
        //e inclui nome e sobrenome do usuário
        List<Claim> claims = userClaims.Claims.ToList();
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

        //se o usermanager do identity suportar roles
        if (_userManager.SupportsUserRole)
        {
            //obtem lista de roles do usuário
            IList<string> roles = await _userManager.GetRolesAsync(user);
            //percorre a lista
            foreach (string role in roles)
            {
                //adiciona a claim de role para o usuário
                claims.Add(new Claim(JwtClaimTypes.Role, role));

                //se rolemanager do identity suportar claims
                if (_roleManager.SupportsRoleClaims)
                {
                    //obtem a role do usuário
                    IdentityRole identityRole = await _roleManager.FindByNameAsync(role);
                  
                    //inclui o perfil
                    if (identityRole != null)
                    {
                        //inclui as claims da role para o usuário
                        claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                    }
                }
            }
         }
        //retorna as claims do usuário para o IS
        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        //id do usuário no IS
        string id = context.Subject.GetSubjectId();

        //localiza o usuário e verifica se ele está ativo
        ApplicationUser user = await _userManager.FindByIdAsync(id);

        //verifica se esta ativo
        context.IsActive = user is not null;
    }
}
