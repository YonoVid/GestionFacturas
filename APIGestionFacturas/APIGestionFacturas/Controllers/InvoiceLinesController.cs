using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
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
        private readonly IInvoiceLineService _invoiceLinesService;
        private readonly JwtSettings _jwtSettings;

        public InvoiceLinesController(GestionFacturasContext context,
                            IInvoiceLineService invoiceLinesService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _invoiceLinesService = invoiceLinesService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/InvoiceLine
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines()
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            return await _context.InvoiceLines.ToListAsync();
        }

        // GET: api/InvoiceLine/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<InvoiceLine>> GetInvoiceLine(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(GetUsers)}:: RUNNING FUNCTION CALL");
            var invoiceLine = await _context.InvoiceLines.FindAsync(id);

            if (invoiceLine == null)
            {
                return NotFound();
            }

            return invoiceLine;
        }

        // PUT: api/Enterprise/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, InvoiceLineEditable invoiceLineData)
        {
            // _logger.LogInformation($"{nameof(UsersController)} - {nameof(PutUser)}:: RUNNING FUNCTION CALL");

            InvoiceLine? invoiceLine = await _context.InvoiceLines.FindAsync(id);

            if (invoiceLine == null)
            {
                return BadRequest();
            }

            try
            {
                if (invoiceLineData.Item != null) { invoiceLine.Item = invoiceLineData.Item; }
                if (invoiceLineData.Quantity != null) { invoiceLine.Quantity = (int)invoiceLineData.Quantity; }
                if (invoiceLineData.ItemValue != null) { invoiceLine.ItemValue= (float)invoiceLineData.ItemValue; }

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

            return CreatedAtAction("GetUser", new { id = invoiceLine.Id }, invoiceLine);
        }

        // POST: api/InvoiceLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InvoiceLine>> PostUser(InvoiceLine invoiceLine)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            _context.InvoiceLines.Add(invoiceLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = invoiceLine.Id }, invoiceLine);
        }

        // DELETE: api/InvoiceLine/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            var invoiceLine = await _context.InvoiceLines.FindAsync(id);
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
