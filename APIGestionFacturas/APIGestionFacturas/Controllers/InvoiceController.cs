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
        private readonly GestionFacturasContext _context;           //Contexto de las bases de datos
        private readonly IInvoiceService _invoiceService;           //Servicios relacionados a las facturas
        private readonly IInvoiceLineService _invoiceLineService;   //Servicios relacionados a lineas de las facturas
        private readonly IConverter _converter;                     //Funciones de generación de pdf
        private readonly JwtSettings _jwtSettings;                  //Configuraciones de JWT

        //Inicialización del controlador asignando instancias de los servicios asociados
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
        /*
         * Función para realizar la generación de pdfs y entregarlos 
         * @param Se requiere {id} de la factura
        */
        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult> GetInvoicePdf(int id)
        {

            var invoice = await _invoiceService.GetAvailableInvoice(_context.Invoices, HttpContext.User, id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceLines = _invoiceLineService.GetAvailableInvoiceLines(_context.InvoiceLines, HttpContext.User, id).ToArray();

            var pdf = _invoiceService.GetInvoicePdf(invoice.Enterprise, invoice, invoiceLines);


            return File(_converter.Convert(pdf), "application/pdf");
        }

        // **** CRUD de la tabla ****

        // GET: api/Invoice
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {

            var invoices = await _invoiceService.GetAvailableInvoices(_context.Invoices, HttpContext.User).ToListAsync();

            if(invoices != null)
            {
                return invoices;
            }


            return new List<Invoice>();
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetEnterpriseInvoices(int id)
        {
            var invoices = await _invoiceService.GetAvailableEnterpriseInvoices(_context.Invoices, HttpContext.User, id).ToListAsync();
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
            var invoice = await _invoiceService.GetAvailableInvoice(_context.Invoices, HttpContext.User, id);

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

            Invoice editedInvoice;

            if (invoiceData.Name == null &&
                invoiceData.EnterpriseId == null &&
                invoiceData.TaxPercentage == null)
            {
                return BadRequest("Faltan datos para modificar la entidad");
            }

            try
            {
                editedInvoice = await _invoiceService.EditInvoice(_context, HttpContext.User, invoiceData, id);
                if(invoiceData.InvoiceLines?.Count > 0)
                {
                    List<InvoiceLine> invoiceLines = await _context.InvoiceLines.Where((InvoiceLine row) => row.InvoiceId == id).ToListAsync();

                    int invoiceLineIndex = 0;

                    foreach(InvoiceLineEditable invoiceLineData in invoiceData.InvoiceLines)
                    {
                        if (invoiceLines.Count > invoiceLineIndex)
                        {
                            await _invoiceLineService.EditInvoiceLine(_context, HttpContext.User, invoiceLineData, id);
                        }
                        else
                        {
                            await _invoiceLineService.CreateInvoiceLine(_context, HttpContext.User, invoiceLineData, id);
                        }

                        invoiceLineIndex++;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
            Invoice createdInvoice;

            if(invoiceData.Name == null &&
                invoiceData.EnterpriseId == null &&
                invoiceData.TaxPercentage == null)
            {
                return BadRequest("Faltan datos para generar la entidad");
            }
            try
            {
                createdInvoice = await _invoiceService.CreateInvoice(_context, HttpContext.User, invoiceData);
                if (invoiceData.InvoiceLines?.Count > 0)
                {
                    foreach (InvoiceLineEditable invoiceLineData in invoiceData.InvoiceLines)
                    {
                        await _invoiceLineService.CreateInvoiceLine(_context, HttpContext.User, invoiceLineData, createdInvoice.Id);
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
            Invoice deletedInvoice;
            try
            {
                deletedInvoice = await _invoiceService.DeleteInvoice(_context, HttpContext.User, id);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
