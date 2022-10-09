using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using DinkToPdf.Contracts;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIGestionFacturas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly GestionFacturasContext _context;
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly IConverter _converter;
        private readonly JwtSettings _jwtSettings;                 

        //Initialize services
        public InvoiceController(GestionFacturasContext context,
                                 IInvoiceService invoiceService,
                                 IInvoiceLineService invoiceLineService,
                                 IConverter converter,
                                 JwtSettings jwtSettings)
        {
            _context = context;
            _invoiceService = invoiceService;
            _invoiceLineService = invoiceLineService;
            _converter = converter;
            _jwtSettings = jwtSettings;
        }

        // GET: api/Invoice/GetInvoicePdf/5
        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult> GetInvoicePdf(int id)
        {
            // User serviec to get the requested id
            var invoice = await _invoiceService.GetAvailableInvoice(_context.Invoices, HttpContext.User, id);

            if (invoice == null)
            {
                // If the invoice in not founded
                return NotFound();
            }
            try
            {
                // Get invoice enterprise data
                invoice.Enterprise = await _context.Enterprises.FindAsync(invoice.EnterpriseId);

                // Get enterprise user data
                invoice.Enterprise.User = await _context.Users.FindAsync(invoice.Enterprise.UserId);

                // Get available invoice lines from the invoice
                var invoiceLines = _invoiceLineService.GetAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User, id).ToArray();

                // Generate invoice pdf
                var pdf = _converter.Convert(_invoiceService.GetInvoicePdf(invoice.Enterprise, invoice, invoiceLines));

                FileStream stream = new FileStream((Path.Combine(Directory.GetCurrentDirectory(), "invoiceFiles", invoice.Id + "_invoice.pdf")),
                                                        FileMode.OpenOrCreate);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(pdf, 0, pdf.Length);
                writer.Close();

                // Return file in a byte format
                return File(pdf, "application/pdf");
            }
            catch(Exception ex)
            {
                // Return BadRequest if a error is catched
                return BadRequest(ex.Message);
            }
        }

        // **** CRUD de la tabla ****

        // GET: api/Invoice
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            // Use service to get available invoices
            var invoices = await _invoiceService.GetAvailableInvoices(_context.Invoices, HttpContext.User).ToListAsync();

            if(invoices != null)
            {
                // Return all available invoices from the database
                return invoices;
            }
            // Return empty list if result of service is null
            return new List<Invoice>();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetEnterpriseInvoices(int id)
        {
            // Use service to get available invoices from a enterprise
            var invoices = await _invoiceService.GetAvailableEnterpriseInvoices(_context.Invoices, HttpContext.User, id).ToListAsync();
            
            if (invoices != null)
            {
                // Return all available invoices from the database
                return invoices;
            }
            // Return empty list if result of service is null
            return new List<Invoice>();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            // Search selected invoice with the id in the database
            var invoice = await _invoiceService.GetAvailableInvoice(_context.Invoices, HttpContext.User, id);

            if (invoice == null)
            {
                // If invoice isn't found send NotFound result
                return NotFound();
            }
            // Return founded invoice
            return invoice;
        }

        // PUT: api/Invoice/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceEditable invoiceData)
        {
            // Create variable to store updated invoice
            Invoice editedInvoice;

            try
            {
                // Use service to update and store invoice
                editedInvoice = await _invoiceService.EditInvoice(_context, HttpContext.User, invoiceData, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the invoice is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                // If not enough data is provided
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // For any other error from the database throw an exception
                throw ex;
            }
            // Return data from the updated invoice
            return CreatedAtAction("PutInvoice", new { id = editedInvoice.Id }, editedInvoice);
        }

        // POST: api/Invoice
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> PostInvoice(InvoiceEditable invoiceData)
        {
            // Create variable to store created invoice
            Invoice createdInvoice;
            
            try
            {
                // Use service to create and store invoice
                createdInvoice = await _invoiceService.CreateInvoice(_context, HttpContext.User, invoiceData);
            }
            catch(KeyNotFoundException ex)
            {
                // If key of the invoice is not found return NotFound result
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // If not enough data is provided
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // For any other error from the database throw an exception
                throw ex;
            }
            // Return data from the created invoice
            return CreatedAtAction("PostInvoice", new { id = createdInvoice.Id }, createdInvoice);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            // Create variable to store deleted invoice
            Invoice deletedInvoice;

            try
            {
                // Use service to obtain deleted invoice and store it
                deletedInvoice = await _invoiceService.DeleteInvoice(_context, HttpContext.User, id);
            }
            catch(KeyNotFoundException ex)
            {
                // If the enterprise is not founded
                return NotFound(ex.Message);
            }
            // Return message to indicate action was successful
            return Ok();
        }
    }
}
