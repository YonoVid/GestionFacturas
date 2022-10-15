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
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly IConverter _converter;
        private readonly JwtSettings _jwtSettings;                 

        //Initialize services
        public InvoiceController(IInvoiceService invoiceService,
                                 IInvoiceLineService invoiceLineService,
                                 IConverter converter,
                                 JwtSettings jwtSettings)
        {
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
            try
            {
                // User serviec to get the requested id
                var invoice = await _invoiceService.GetAvailableInvoice(id);

                // Generate invoice pdf
                var pdf = _converter.Convert(await _invoiceService.GetInvoicePdf(id));

                FileStream stream = new FileStream((Path.Combine(Directory.GetCurrentDirectory(), "invoiceFiles", invoice.Id + "_invoice.pdf")),
                                                        FileMode.OpenOrCreate);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(pdf, 0, pdf.Length);
                writer.Close();

                // Return file in a byte format
                return File(pdf, "application/pdf");
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the invoice is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
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
            // Variable to store invoices
            IQueryable<Invoice> invoices;
            try
            {
                // Use service to get available invoices
                invoices = _invoiceService.GetAvailableInvoices();

            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            // Return list of result from the service
            return invoices.ToList();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetEnterpriseInvoices(int id)
        {
            IQueryable<Invoice> invoices;
            try
            {
                // Use service to get available invoices from a enterprise
                invoices = _invoiceService.GetAvailableEnterpriseInvoices(id);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            // Return result of service as a IEnumerable
            return invoices.ToList();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {   
            // Variable to store invoice
            Invoice invoice;
            try
            {
                // Search selected invoice with the id in the database
                invoice = await _invoiceService.GetAvailableInvoice(id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the invoice is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
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
                editedInvoice = await _invoiceService.EditInvoice(invoiceData, id);
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
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
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
                createdInvoice = await _invoiceService.CreateInvoice(invoiceData);
            }
            catch(KeyNotFoundException ex)
            {
                // If key of the invoice is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // If not enough data is provided
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
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
                deletedInvoice = await _invoiceService.DeleteInvoice(id);
            }
            catch(KeyNotFoundException ex)
            {
                // If the enterprise is not founded
                return NotFound(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            // Return message to indicate action was successful
            return Ok();
        }
    }
}
