using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IInvoiceLineService
    {
        /// <summary>
        /// Function return the invoice lines available to the user.
        /// </summary>
        /// <returns> IQueryable with the invoice lines available to the user. </returns>
        IQueryable<InvoiceLine>? GetAvailableInvoiceLines();
        /// <summary>
        /// Function return the invoice lines from a invoice that are available to the user.
        /// </summary>
        /// <param name="InvoiceId"> The asociated id of the invoice. </param>
        /// <returns> Task with the requested invoice lines available from the selected invoice. </returns>
        public IQueryable<InvoiceLine> GetAvailableInvoiceLines(int InvoiceId);
        /// <summary>
        /// Function return the invoice line if is available to the user.
        /// </summary>
        /// <param name="id"> The asociated id of the invoice line. </param>
        /// <returns> Task with the requested invoice line selected if allowed or null otherwise. </returns>
        Task<InvoiceLine?> GetAvailableInvoiceLine(int id);
        /// <summary>
        /// Function to create a InvoiceLine class inside a database.
        /// </summary>
        /// <param name="invoiceLineData"> Data to create a new invoice line. </param>
        /// <returns> A Task returns the created invoice line. </returns>
        Task<InvoiceLine> CreateInvoiceLine(InvoiceLineEditable invoiceLineData,
                                            int invoiceId = -1);
        /// <summary>
        /// Function to update a InvoiceLine class inside a database.
        /// </summary>
        /// <param name="invoiceLineData"> Data to edit the selected invoice line. </param>
        /// <param name="invoiceLineId"> Id asociated to the selected invoice line. </param>
        /// <returns> A Task returns the updated invoice line. </returns>
        Task<InvoiceLine> EditInvoiceLine(InvoiceLineEditable invoiceLineData,
                                          int invoiceLineId);
        /// <summary>
        /// Function to delete a InvoiceLine class inside a database.
        /// </summary>
        /// <param name="invoiceLineId"> Id asociated to the selected invoice line. </param>
        /// <returns> A Task returns the deleted invoice line. </returns>
        Task<InvoiceLine> DeleteInvoiceLine(int invoiceLineId);
    }
}
