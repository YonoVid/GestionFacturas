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
    public class InvoiceController : ControllerBase
    {
        private readonly GestionFacturasContext _context;
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly JwtSettings _jwtSettings;

        public InvoiceController(GestionFacturasContext context,
                            IInvoiceService invoiceService,
                            IInvoiceLineService invoiceLineService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _invoiceService = invoiceService;
            _invoiceLineService = invoiceLineService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/Invoice
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {

            var invoices = await _invoiceService.getAvailableInvoices(_context.Invoices, HttpContext.User).ToListAsync();

            if(invoices != null)
            {
                return invoices;
            }

            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");

            return new List<Invoice>();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetEnterpriseInvoices(int id)
        {
            var invoices = await _invoiceService.getAvailableEnterpriseInvoices(_context.Invoices, HttpContext.User, id).ToListAsync();
            if (invoices != null)
            {
                return invoices;
            }

            return new List<Invoice>();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            var invoice = await _invoiceService.getAvailableInvoice(_context.Invoices, HttpContext.User, id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoice/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceEditable invoiceData)
        {
            // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            Invoice editedInvoice;

            if (invoiceData.Name == null &&
                invoiceData.EnterpriseId == null &&
                invoiceData.TaxPercentage == null)
            {
                return BadRequest("Faltan datos para modificar la entidad");
            }

            try
            {
                editedInvoice = await _invoiceService.editInvoice(_context, HttpContext.User, invoiceData, id);
                if(invoiceData.InvoiceLines?.Count > 0)
                {
                    List<InvoiceLine> invoiceLines = await _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == id).ToListAsync();

                    int invoiceLineIndex = 0;

                    foreach(InvoiceLineEditable invoiceLineData in invoiceData.InvoiceLines)
                    {
                        if (invoiceLines.Count > invoiceLineIndex)
                        {
                            await _invoiceLineService.editInvoiceLine(_context, HttpContext.User, invoiceLineData, id);
                        }
                        else
                        {
                            await _invoiceLineService.createInvoiceLine(_context, HttpContext.User, invoiceLineData, id);
                        }

                        invoiceLineIndex++;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (_context.Invoices.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");

                    throw ex;
                }
            }

            return CreatedAtAction("PutInvoice", new { id = editedInvoice.Id }, editedInvoice);
        }

        // POST: api/Invoice
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> PostInvoice(InvoiceEditable invoiceData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            Invoice createdInvoice;

            if(invoiceData.Name == null &&
                invoiceData.EnterpriseId == null &&
                invoiceData.TaxPercentage == null)
            {
                return BadRequest("Faltan datos para generar la entidad");
            }
            try
            {
                createdInvoice = await _invoiceService.createInvoice(_context, HttpContext.User, invoiceData);
                if (invoiceData.InvoiceLines?.Count > 0)
                {
                    foreach (InvoiceLineEditable invoiceLineData in invoiceData.InvoiceLines)
                    {
                        await _invoiceLineService.createInvoiceLine(_context, HttpContext.User, invoiceLineData, createdInvoice.Id);
                    }
                }
            }
            catch(KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return CreatedAtAction("PostInvoice", new { id = createdInvoice.Id }, createdInvoice);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            Invoice deletedInvoice;
            try
            {
                deletedInvoice = await _invoiceService.deleteInvoice(_context, HttpContext.User, id);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
