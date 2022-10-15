using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
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
    public class InvoiceLineController : ControllerBase
    {
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly JwtSettings _jwtSettings;

        public InvoiceLineController(IInvoiceLineService invoiceLinesService,
                                     JwtSettings jwtSettings)
        {
            _invoiceLineService = invoiceLinesService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/InvoiceLine
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines()
        {
            // Variable to store invoices
            IQueryable<InvoiceLine> invoiceLines;
            try
            {
                // Use service to get available invoice lines
                invoiceLines = _invoiceLineService.GetAvailableInvoiceLines();

            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            // Return list of result from the service
            return invoiceLines.ToList();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines(int id)
        {
            // Use service to get available invoice lines from invoice of the id
            var invoiceLines = await _invoiceLineService.GetAvailableInvoiceLines(id).ToListAsync();

            if (invoiceLines != null)
            {
                // Return all invoice lines of the invoice from the database
                return invoiceLines;
            }
            // Return empty list if result of service is null
            return new List<InvoiceLine>();
        }

        // GET: api/InvoiceLine/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<InvoiceLine>> GetInvoiceLine(int id)
        {
            // Variable to store invoice line
            InvoiceLine invoiceLine;
            try
            {
                // Search selected invoice line with the id in the database
                invoiceLine = await _invoiceLineService.GetAvailableInvoiceLine(id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the enterprise is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                // If not database is founded
                return StatusCode(500, ex.Message);
            }
            // Return founded invoice line
            return invoiceLine;
        }

        // PUT: api/Enterprise/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutInvoiceLine(int id, InvoiceLineEditable invoiceLineData)
        {
            // Create variable to store updated invoice line
            InvoiceLine editedInvoiceLine;

            try
            {
                // Use service to update and store invoice line
                editedInvoiceLine = await _invoiceLineService.EditInvoiceLine(invoiceLineData, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of invoice line is not found return NotFound result
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // For any other error from the database throw an exception
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
            // Return data from the updated invoice line
            return CreatedAtAction("PutInvoiceLine", new { id = editedInvoiceLine.Id }, editedInvoiceLine);
        }

        // POST: api/InvoiceLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<InvoiceLine>> PostInvoiceLine(InvoiceLineEditable invoiceLineData)
        {
            // Create variable to store created invoice line
            InvoiceLine createdInvoiceLine;
            try
            {
                // Use service to create and store invoice line
                createdInvoiceLine = await _invoiceLineService.CreateInvoiceLine(invoiceLineData);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of invoice is not found return NotFound result
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
            // Return data from the created invoice line
            return CreatedAtAction("PostInvoiceLine", new { id = createdInvoiceLine.Id }, createdInvoiceLine);
        }

        // DELETE: api/InvoiceLine/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoiceLine(int id)
        {
            // Create variable to store deleted invoice line
            InvoiceLine deletedInvoiceLine;

            try
            {
                // Use service to obtain deleted invoice line and store it
                deletedInvoiceLine = await _invoiceLineService.DeleteInvoiceLine(id);
            }
            catch (KeyNotFoundException ex)
            {
                // If the invoice is not founded
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
