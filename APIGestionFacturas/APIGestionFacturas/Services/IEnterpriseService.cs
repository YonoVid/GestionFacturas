using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IEnterpriseService
    {
        IQueryable<Enterprise> getAvailableEnterprises(IQueryable<Enterprise> enterprises, ClaimsPrincipal userClaims);
        Task<Enterprise> getAvailableEnterprise(DbSet<Enterprise> enterprises, ClaimsPrincipal userClaims, int id);

        Task<Enterprise> createEnterprise(GestionFacturasContext _context,
                                      ClaimsPrincipal userClaims,
                                      EnterpriseEditable enterpriseData);

        Task<Enterprise> deleteEnterprise(GestionFacturasContext _context,
                                       ClaimsPrincipal userClaims,
                                       int enterpriseId);

        Task<Enterprise> editEnterprise(GestionFacturasContext _context,
                                     ClaimsPrincipal userClaims,
                                     EnterpriseEditable enterpriseData,
                                     int enterpriseId);
    }
}
