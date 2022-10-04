using GestionFacturasModelo.Model.DataModel;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IEnterpriseService
    {
        IQueryable<Enterprise> getAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims);
    }
}
