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
        private readonly JwtSettings _jwtSettings;

        public InvoiceController(GestionFacturasContext context,
                            IInvoiceService invoiceService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _invoiceService = invoiceService;
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
        public async Task<ActionResult<IEnumerable<Invoice>>> GetEnterpriseInvoices(int enterpriseId)
        {
            var invoices = await _invoiceService.getAvailableEnterpriseInvoices(_context.Invoices, HttpContext.User, enterpriseId).ToListAsync();
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

            Invoice? invoice = await _invoiceService.getAvailableInvoice(_context.Invoices, HttpContext.User, id);

            if (invoice == null)
            {
                return BadRequest();
            }

            try
            {
                if (invoiceData.EnterpriseId != null) { invoice.EnterpriseId = (int)invoiceData.EnterpriseId; }
                if (invoiceData.TaxPercentage!= null) { invoice.TaxPercentage= (int)invoiceData.TaxPercentage; }

                _context.Invoices.Update(invoice);

                _context.Entry(invoice).State = EntityState.Modified;

                await _context.SaveChangesAsync();
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

            return CreatedAtAction("PutInvoice", new { id = invoice.Id }, invoice);
        }

        // POST: api/Invoice
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Invoice>> PostInvoice(InvoiceEditable invoiceData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            if(invoiceData.Name == null &&
                invoiceData.EnterpriseId == null &&
                invoiceData.TaxPercentage == null)
            {
                return BadRequest("Faltan datos para generar la entidad");
            }

            var invoice = new Invoice(invoiceData);

            Enterprise? enterprise= await _context.Enterprises.FindAsync(invoiceData.EnterpriseId);
            if (enterprise == null) { return BadRequest("Id de usuario no encontrado"); }
            invoice.Enterprise = enterprise;

            invoice.CreatedBy = HttpContext.User.Identity.Name;

            invoice.Id = _context.Invoices.Add(invoice).Entity.Id;
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostInvoice", new { id = invoice.Id }, invoice);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            var invoice = await _invoiceService.getAvailableInvoice(_context.Invoices, HttpContext.User, id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.DeletedBy = HttpContext.User.Identity.Name;
            invoice.DeletedDate = DateTime.Now;
            invoice.IsDeleted = true;


            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
