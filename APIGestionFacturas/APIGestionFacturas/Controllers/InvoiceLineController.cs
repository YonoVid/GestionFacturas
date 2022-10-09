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
        private readonly GestionFacturasContext _context;
        private readonly IInvoiceLineService _invoiceLineService;
        private readonly JwtSettings _jwtSettings;

        public InvoiceLineController(GestionFacturasContext context,
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
            var invoiceLines = await _invoiceLineService.GetAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User).ToListAsync();
            if (invoiceLines != null)
            {
                return invoiceLines;
            }

            return new List<InvoiceLine>();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines(int id)
        {
            var invoiceLines = await _invoiceLineService.GetAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User, id).ToListAsync();
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
            var invoiceLine = await _invoiceLineService.GetAvailableInvoiceLine(_context.InvoiceLines, HttpContext.User, id);

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

            InvoiceLine editedInvoiceLine;

            if(invoiceLineData.Item == null &&
               invoiceLineData.Quantity == null &&
               invoiceLineData.ItemValue == null)
            {
                return BadRequest("Faltan datos para modificar la entidad");
            }

            try
            {
                editedInvoiceLine = await _invoiceLineService.EditInvoiceLine(_context, HttpContext.User, invoiceLineData, id);
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

            return CreatedAtAction("PutInvoiceLine", new { id = editedInvoiceLine.Id }, editedInvoiceLine);
        }

        // POST: api/InvoiceLine
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<InvoiceLine>> PostInvoiceLine(InvoiceLineEditable invoiceLineData)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(PostUser)}:: RUNNING FUNCTION CALL");

            InvoiceLine createdInvoiceLine;

            if (invoiceLineData.Item == null ||
                invoiceLineData.Quantity == null ||
                invoiceLineData.ItemValue == null ||
                invoiceLineData.InvoiceId == null)
            {
                return BadRequest("Faltan datos para generar la entidad");
            }

            try
            {
                createdInvoiceLine = await _invoiceLineService.CreateInvoiceLine(_context, HttpContext.User, invoiceLineData, (int)invoiceLineData.InvoiceId);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return CreatedAtAction("PostInvoiceLine", new { id = createdInvoiceLine.Id }, createdInvoiceLine);
        }

        // DELETE: api/InvoiceLine/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteInvoiceLine(int id)
        {
            //_logger.LogInformation($"{nameof(UsersController)} - {nameof(DeleteUser)}:: RUNNING FUNCTION CALL");

            InvoiceLine deletedInvoiceLine;

            try
            {
                deletedInvoiceLine = await _invoiceLineService.DeleteInvoiceLine(_context, HttpContext.User, id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }


            return Ok();
        }
    }
}
