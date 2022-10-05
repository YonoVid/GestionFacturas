using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        public IQueryable<Enterprise> getAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims)
        {
            if(!userClaims.IsInRole("Administrator"))
            {
                var identity = userClaims.Identity as ClaimsIdentity;
                if (identity != null)
                {
                   var id = identity.FindFirst("Id").Value;
                   return enterprises.Where((enterprise) => enterprise.User.Id.ToString() == id && !enterprise.IsDeleted);

                }
                return null;
            }
            return enterprises;
        }
        public async Task<Enterprise> getAvailableEnterprise(DbSet<Enterprise> enterprises, ClaimsPrincipal userClaims, int id)
        {

            Enterprise result = await enterprises.FindAsync(id);

            if(!userClaims.IsInRole("Administrator") && result != null)
            {
                var identity = userClaims.Identity as ClaimsIdentity;

                int idToken = int.Parse(identity.FindFirst("Id").Value);

                if (result.UserId != idToken)
                {
                    return null;
                }
            }

            return result;
        }
    }
}
