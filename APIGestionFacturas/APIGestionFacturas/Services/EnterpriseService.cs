using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
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
                   return enterprises.Where((enterprise) => enterprise.User.Id.ToString() == id);

                }
                return null;
            }
            return enterprises;
        }
    }
}
