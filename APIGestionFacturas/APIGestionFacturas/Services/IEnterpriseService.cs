using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IEnterpriseService
    {
        /// <summary>
        /// Function return the enterprises available to the user.
        /// </summary>
        /// <returns> IQueryable with the enterprises available to the user. </returns>
        IQueryable<Enterprise>? GetAvailableEnterprises();
        /// <summary>
        /// Function may return a enterprise, but only if the user is authorized to read it.
        /// </summary>
        /// <param name="id"> Id asociated to the selected enterprise. </param>
        /// <returns> Task with the requested invoice or null otherwise </returns>
        Task<Enterprise?> GetAvailableEnterprise(int id);

        /// <summary>
        /// Function to create a Enterprise class inside a database.
        /// </summary>
        /// <param name="enterpriseData"> Data to edit the selected enterprise. </param>
        /// <returns> A Task returns the created enterprise. </returns>
        Task<Enterprise> CreateEnterprise(EnterpriseEditable enterpriseData);

        /// <summary>
        /// Function to update a Enterprise class inside a database.
        /// </summary>
        /// <param name="enterpriseData"> Data to edit the selected enterprise. </param>
        /// <param name="enterpriseId"> Id asociated to the selected enterprise. </param>
        /// <returns> A Task returns the updated enterprise. </returns>
        Task<Enterprise> EditEnterprise(EnterpriseEditable enterpriseData,
                                     int enterpriseId);

        /// <summary>
        /// Function to delete a Enterprise class inside a database.
        /// </summary>
        /// <param name="enterpriseId"> Id asociated to the selected enterprise. </param>
        /// <returns> A Task returns the deleted enterprise. </returns>
        Task<Enterprise> DeleteEnterprise(int enterpriseId);
    }
}
