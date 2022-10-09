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
        /// <param name="invoices"> The data of the invoices to search in. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <returns> IQueryable of the allowed invoices to read or null otherwise </returns>
        IQueryable<Invoice>? GetAvailableInvoices(IQueryable<Invoice> invoices,
                                                 ClaimsPrincipal userClaims);
        /// <summary>
        /// Function may return a invoice, but only if the user is authorized to read it.
        /// </summary>
        /// <param name="invoices"> The data of the invoices to search in. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="id"> Id asociated to the selected invoice. </param>
        /// <returns> Task with the requested invoice or null otherwise </returns>
        Task<Invoice?> GetAvailableInvoice(DbSet<Invoice> invoices,
                                          ClaimsPrincipal userClaims,
                                          int id);
        /// <summary>
        /// Function return the invoices from a enterprise that are available to the user.
        /// </summary>
        /// <param name="invoices"> The data of the invoices to search in. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="enterpriseId"> Id of the enterprise selected. </param>
        /// <returns> IQueryable with all the invoices the user is allowed to read.  </returns>
        IQueryable<Invoice> GetAvailableEnterpriseInvoices(IQueryable<Invoice> invoices,
                                                           ClaimsPrincipal userClaims,
                                                           int enterpriseId);
        /// <summary>
        /// Function to create a Invoice class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the Invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="invoiceData"> Data to create a new invoice. </param>
        /// <returns> A Task returns the created invoice. </returns>
        Task<Invoice> CreateInvoice(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    InvoiceEditable invoiceData);
        /// <summary>
        /// Function to edit the data from a Invoice class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the Invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="invoiceData"> Data to edit the selected invoice. </param>
        /// <param name="invoiceId"> Id asociated to the selected invoice. </param>
        /// <returns> A Task returns the updated invoice. </returns>
        Task<Invoice> EditInvoice(GestionFacturasContext _context,
                                  ClaimsPrincipal userClaims,
                                  InvoiceEditable invoiceData,
                                  int invoiceId);
        /// <summary>
        /// Function to delete a Invoice class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="invoiceId"> Id asociated to the selected invoice. </param>
        /// <returns> A Task returns the deleted invoice. </returns>
        Task<Invoice> DeleteInvoice(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    int invoiceId);
        /// <summary>
        /// Function to generate a pdf from the data of a enterprise, a invoice and it's lines.
        /// </summary>
        /// <param name="enterprise"> The asociated enterprise of the invoice. </param>
        /// <param name="invoice"> The invoice we want to generate the pdf from. </param>
        /// <param name="invoiceLines"> The data of the invoice lines. </param>
        /// <returns> A HtmlToPdfDocument class with the asociated data to generate the pdf (Uses DinkToPdf class). </returns>
        HtmlToPdfDocument GetInvoicePdf(Enterprise enterprise, Invoice invoice, InvoiceLine[] invoiceLines);
    }
}
