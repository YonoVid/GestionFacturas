using APIGestionFacturas.DataAccess;
using DinkToPdf;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IInvoiceService
    {
        /// <summary>
        /// Function that returns invoices, but only the ones the user is
        /// authorized to read.
        /// </summary>
        /// <returns> IQueryable of the allowed invoices to read or null otherwise </returns>
        IQueryable<Invoice> GetAvailableInvoices();
        /// <summary>
        /// Function may return a invoice, but only if the user is authorized to read it.
        /// </summary>
        /// <param name="id"> Id asociated to the selected invoice. </param>
        /// <returns> Task with the requested invoice or null otherwise </returns>
        Task<Invoice> GetAvailableInvoice(int id);
        /// <summary>
        /// Function return the invoices from a enterprise that are available to the user.
        /// </summary>
        /// <param name="enterpriseId"> Id of the enterprise selected. </param>
        /// <returns> IQueryable with all the invoices the user is allowed to read.  </returns>
        IQueryable<Invoice> GetAvailableEnterpriseInvoices(int enterpriseId);
        /// <summary>
        /// Function to create a Invoice class inside a database.
        /// </summary>
        /// <param name="invoiceData"> Data to create a new invoice. </param>
        /// <returns> A Task returns the created invoice. </returns>
        Task<Invoice> CreateInvoice(InvoiceEditable invoiceData);
        /// <summary>
        /// Function to edit the data from a Invoice class inside a database.
        /// </summary>
        /// <param name="invoiceData"> Data to edit the selected invoice. </param>
        /// <param name="invoiceId"> Id asociated to the selected invoice. </param>
        /// <returns> A Task returns the updated invoice. </returns>
        Task<Invoice> EditInvoice(InvoiceEditable invoiceData,
                                  int invoiceId);
        /// <summary>
        /// Function to delete a Invoice class inside a database.
        /// </summary>
        /// <param name="invoiceId"> Id asociated to the selected invoice. </param>
        /// <returns> A Task returns the deleted invoice. </returns>
        Task<Invoice> DeleteInvoice(int invoiceId);
        /// <summary>
        /// Function to generate a pdf from the data of a enterprise, a invoice and it's lines.
        /// </summary>
        /// <param name="id"> The asociated id of the invoice. </param>
        /// <returns> A HtmlToPdfDocument class with the asociated data to generate the pdf (Uses DinkToPdf class). </returns>
        Task<HtmlToPdfDocument> GetInvoicePdf(int id);
    }
}
