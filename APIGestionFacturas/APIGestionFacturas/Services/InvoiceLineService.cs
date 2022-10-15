using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public class InvoiceLineService: IInvoiceLineService
    {
        private readonly GestionFacturasContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InvoiceLineService(GestionFacturasContext context,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<InvoiceLine> GetAvailableInvoiceLines()
        {
            if (_context.InvoiceLines == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Check if user identity is not null and try to get id
                var identity = userClaims.Identity as ClaimsIdentity;
                var id = identity?.FindFirst("Id")?.Value;

                if (id != null)
                {
                    return _context.InvoiceLines.Where((invoiceLine) => 
                        invoiceLine.Invoice.Enterprise.UserId.ToString().Equals(id)
                    );
                }
                // If identity is not founded return empty IQueryable
                return Enumerable.Empty<InvoiceLine>().AsQueryable();
            }
            // Return obtained invoice lines, may be null
            return _context.InvoiceLines;
        }

        public async Task<InvoiceLine> GetAvailableInvoiceLine(int id)
        {
            if (_context.InvoiceLines == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search for the indicated invoice line
            InvoiceLine? result = await _context.InvoiceLines.FindAsync(id);

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator") && result != null)
            {
                // Get enterprise user id and check if the user making the call is the same
                var identity = userClaims.Identity as ClaimsIdentity;
                if (result.Invoice.Enterprise.UserId.ToString() != identity?.FindFirst("Id")?.Value)
                {
                    // Return null if the user doesn't have permission
                    result = null;
                }
            }
            if (result == null)
            {
                // Throw error if no valid invoice was founded
                throw new KeyNotFoundException("Linea de factura no encontrada");
            }
            // Return obtained invoice line, may be null
            return result;
        }

        public IQueryable<InvoiceLine> GetAvailableInvoiceLines(int InvoiceId)
        {
            if (_context.InvoiceLines == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Check if user is 'Administrator'
            if (!userClaims.IsInRole("Administrator"))
            {
                // Get all the invoice lines from the invoice selected
                var filteredInvoiceLines = _context.InvoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);

                // Check if user identity is not null and try to get id
                var identity = userClaims.Identity as ClaimsIdentity;
                var id = identity?.FindFirst("Id")?.Value;

                if (id != null)
                {
                    // Check if the asociated user of the enterprise is the user making the call
                    if (filteredInvoiceLines.Any((invoiceLine) => invoiceLine.Invoice.Enterprise.UserId.ToString().Equals(id)))
                    { return filteredInvoiceLines; }

                }
                // If a condition is not met return null
                return Enumerable.Empty<InvoiceLine>().AsQueryable();
            }
            // If user is 'Administrator' return all invoice lines from the invoice
            return _context.InvoiceLines.Where((invoiceLine) => invoiceLine.Invoice.Id == InvoiceId);
        }

        public async Task<InvoiceLine> CreateInvoiceLine(InvoiceLineEditable invoiceLineData)
        {
            if (_context.InvoiceLines == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }

            if (invoiceLineData.Item == null ||
                invoiceLineData.Quantity == null ||
                invoiceLineData.ItemValue == null ||
                invoiceLineData.InvoiceId == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("Faltan datos para generar la entidad");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Create new invoice line from data provided
            var invoiceLine = new InvoiceLine(invoiceLineData);

            // Search indicated invoice
            Invoice? invoice = await _context.Invoices.FindAsync(invoiceLineData.InvoiceId);
            if(invoice == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Id de factura no encontrado");
            }

            // Updated data of the invoice related to updation
            if (userClaims.Identity?.Name == null && !userClaims.IsInRole("Administrator"))
            { throw new NullReferenceException("Identidad de petición es nula"); }
            // Identity only is allowed to be empty if user is administrator
            invoice.UpdatedBy = userClaims?.Identity?.Name ?? "Admin";
            invoice.UpdatedDate = DateTime.Now;
            _context.Invoices.Update(invoice);

            // Add data of the invoice line related to invoice
            invoiceLine.Invoice = invoice;

            // Add invoice line
            _context.InvoiceLines.Add(invoiceLine);
            // Update invoice
            invoice.TotalAmount += invoiceLine.ItemValue * invoiceLine.Quantity;
            _context.Invoices.Update(invoice);
            // Save changes
            _context.SetModified(invoice);
            await _context.SaveChangesAsync();

            // Return created invoice line data
            return invoiceLine;
        }

        public async Task<InvoiceLine> EditInvoiceLine(InvoiceLineEditable invoiceLineData,
                                                       int invoiceLineId)
        {
            if (_context.Invoices == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }

            if (invoiceLineData.Item == null &&
               invoiceLineData.Quantity == null &&
               invoiceLineData.ItemValue == null)
            {
                // Throw error if not enough data is provided
                throw new InvalidOperationException("No hay suficientes datos para modificar la entidad");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search the requested invoice line
            InvoiceLine? invoiceLine = await GetAvailableInvoiceLine(invoiceLineId);

            if (invoiceLine == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Linea de factura no encontrada");
            }

            // Modify item if is in data
            if (invoiceLineData.Item != null) { invoiceLine.Item = invoiceLineData.Item; }
            if (invoiceLineData.ItemValue != null ||
                invoiceLineData.Quantity != null ||
                invoiceLineData.InvoiceId != null)
            {
                // Search the invoice asociated 
                Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

                // Save old subtotal
                var oldValue = invoiceLine.Quantity * invoiceLine.ItemValue;

                // Every value included to modify is updated
                if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
                if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue = (float)invoiceLineData.ItemValue; }

                // Check if invoice is null
                if (invoice != null)
                {
                    // If related invoice should be changed
                    if(invoiceLineData.InvoiceId != null)
                    {
                        // Search the new invoice to asociate
                        Invoice? newInvoice = await _context.Invoices.FindAsync(invoiceLineData.InvoiceId);

                        if(newInvoice == null)
                        {
                            // Throw error if no valid invoice was found
                            throw new KeyNotFoundException("Nueva factura no encontrada");
                        }
                        
                        // Add new amount to invoice
                        newInvoice.TotalAmount += invoiceLine.Quantity * invoiceLine.ItemValue;

                        // Updated data of the new related invoice to updation
                        // Identity only is allowed to be empty if user is administrator
                        invoice.UpdatedBy = userClaims?.Identity?.Name ?? "Admin";
                        newInvoice.UpdatedDate = DateTime.Now;
                        _context.Invoices.Update(newInvoice);
                    }
                    else
                    {
                        // Add new amount to invoice related
                        invoice.TotalAmount += invoiceLine.Quantity * invoiceLine.ItemValue;
                    }
                    // Update total amount from invoice
                    invoice.TotalAmount -= oldValue;

                    // Updated data of the invoice related to updation
                    // Identity only is allowed to be empty if user is administrator
                    invoice.UpdatedBy = userClaims?.Identity?.Name ?? "Admin";
                    invoice.UpdatedDate = DateTime.Now;
                    _context.Invoices.Update(invoice);
                }

            }
            // Invoice line is updated and changes are saved
            _context.InvoiceLines.Update(invoiceLine);
            _context.SetModified(invoiceLine);
            await _context.SaveChangesAsync();

            // Return updated invoice line data
            return invoiceLine;
        }

        public async Task<InvoiceLine> DeleteInvoiceLine(int invoiceLineId)
        {
            if (_context.Invoices == null)
            {
                // Throw error if reference to database is null
                throw new NullReferenceException("Referencia a base de datos es nula");
            }
            // Get user claims
            var userClaims = getUserClaims();

            // Search the requested invoice line
            var invoiceLine = await GetAvailableInvoiceLine(invoiceLineId);

            if (invoiceLine == null)
            {
                // Throw error if no valid invoice was found
                throw new KeyNotFoundException("Linea de factura no encontrada");
            }
            // Search the invoice asociated 
            Invoice? invoice = await _context.Invoices.FindAsync(invoiceLine.InvoiceId);

            // Check if invoice is null
            if(invoice != null)
            {
                // Update total amount from invoice
                invoice.TotalAmount -= invoiceLine.Quantity * invoiceLine.ItemValue;

                // Updated data of the invoice related to updation
                // Identity only is allowed to be empty if user is administrator
                invoice.UpdatedBy = userClaims?.Identity?.Name ?? "Admin";
                invoice.UpdatedDate = DateTime.Now;
                _context.Invoices.Update(invoice);
            }
            else
            {
                throw new KeyNotFoundException("Factura asociada no encontrada");
            }
            // Invoice line is deleted and changes are saved
            _context.InvoiceLines.Remove(invoiceLine);
            await _context.SaveChangesAsync();

            // Return deleted invoice line data
            return invoiceLine;
        }
        public ClaimsPrincipal getUserClaims()
        {
            var userClaims = _httpContextAccessor?.HttpContext?.User;
            if (userClaims == null) { throw new BadHttpRequestException("Datos de usuario que realiza la solicitud no encontrados"); }

            return userClaims;
        }
    }
}
