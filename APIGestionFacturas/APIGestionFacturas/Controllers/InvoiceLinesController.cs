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
    public class InvoiceLinesController : ControllerBase
    {
        private readonly GestionFacturasContext _context;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly JwtSettings _jwtSettings;

        public InvoiceLinesController(GestionFacturasContext context,
                            IInvoiceLineService invoiceLinesService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _invoiceLineService = invoiceLinesService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/InvoiceLine
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines()
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            var invoiceLines = await _invoiceLineService.getAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User).ToListAsync();
            if (invoiceLines != null)
            {
                return invoiceLines;
            }

            return new List<InvoiceLine>();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines(int invoiceId)
        {
            var invoiceLines = await _invoiceLineService.getAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User, invoiceId).ToListAsync();
            if (invoiceLines != null)
            {
                return invoiceLines;
            }

            return new List<InvoiceLine>();
        }

        // GET: api/InvoiceLine/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<InvoiceLine>> GetInvoiceLine(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            var invoiceLine = await _invoiceLineService.getAvailableInvoiceLine(_context.InvoiceLines, HttpContext.User, id);

            if (invoiceLine == null)
            {
                return NotFound();
            }

            return invoiceLine;
        }

        // PUT: api/Enterprise/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutInvoiceLine(int id, InvoiceLineEditable invoiceLineData)
        {
            // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            InvoiceLine? invoiceLine = await _invoiceLineService.getAvailableInvoiceLine(_context.InvoiceLines, HttpContext.User, id);

            if (invoiceLine == null)
            {
                return BadRequest();
            }

            try
            {
                if (invoiceLineData.Item != null) { invoiceLine.Item = invoiceLineData.Item; }
                if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
                if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue= (float)invoiceLineData.ItemValue; }

                invoiceLine.UpdatedBy = HttpContext.User.Identity.Name;
                invoiceLine.UpdatedDate = DateTime.Now;

                _context.InvoiceLines.Update(invoiceLine);

                _context.Entry(invoiceLine).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (true)//!_enterpriseService.userExists(_context.Users, user))
                {
                    return NotFound();
                }
                else
                {
                    //_logger.LogWarning($"{nameof(UsersController)} - {nameof(PutUser)}:: UNEXPECTED BEHAVIOUR IN FUNCTION CALL");

                    throw ex;
                }
            }

            return CreatedAtAction("PutInvoiceLine", new { id = invoiceLine.Id }, invoiceLine);
        }

        // POST: api/InvoiceLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<InvoiceLine>> PostInvoiceLine(InvoiceLineEditable invoiceLineData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");
            if (invoiceLineData.Item == null &&
                invoiceLineData.Quantity == null &&
                invoiceLineData.ItemValue == null)
            {
                return BadRequest("Faltan datos para generar la entidad");
            }

            var invoiceLine = new InvoiceLine(invoiceLineData);

            if (invoiceLine == null) { return BadRequest("Id de usuario no encontrado"); }

            invoiceLine.CreatedBy = HttpContext.User.Identity.Name;

            _context.InvoiceLines.Add(invoiceLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostInvoiceLine", new { id = invoiceLine.Id }, invoiceLine);
        }

        // DELETE: api/InvoiceLine/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoiceLine(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            var invoiceLine = await _invoiceLineService.getAvailableInvoiceLine(_context.InvoiceLines, HttpContext.User, id);
            if (invoiceLine == null)
            {
                return NotFound();
            }

            _context.InvoiceLines.Remove(invoiceLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
