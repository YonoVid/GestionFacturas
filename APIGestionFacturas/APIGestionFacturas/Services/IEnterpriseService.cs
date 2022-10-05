using GestionFacturasModelo.Model.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IEnterpriseService
    {
        IQueryable<Enterprise> getAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims);
        Task<Enterprise> getAvailableEnterprise(DbSet<Enterprise> enterprises, ClaimsPrincipal userClaims, int id);

    }
}
