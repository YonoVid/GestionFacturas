using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIGestionFacturas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly GestionFacturasContext _context;           //Contexto con las referencias a tablas de la base de datos
        private readonly IEnterpriseService _enterpriseService;     //Servicio relacionado a las empresas
        private readonly JwtSettings _jwtSettings;                  //Configuración de JWT
        
        //Initialize services
        public EnterpriseController(GestionFacturasContext context,
                            IEnterpriseService enterpriseService,
                            JwtSettings jwtSettings)
        {
            _context = context;
            _enterpriseService = enterpriseService;
            _jwtSettings = jwtSettings;
        }

        // **** CRUD de la tabla ****

        // GET: api/Enterprise
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterprises()
        {
            // Use service to get available enterprises
            var enterprises = await _enterpriseService.GetAvailableEnterprises(_context.Enterprises, HttpContext.User).ToListAsync();
            
            if (enterprises != null)
            {
                // Return all available enterprises from the database
                return enterprises;
            }
            // Return empty list if result of service is null
            return new List<Enterprise>();
        }

        // GET: api/Enterprise/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
        {
            // Search selected enterprise with the id in the database
            var enterprise = await _enterpriseService.GetAvailableEnterprise(_context.Enterprises, HttpContext.User, id);

            if (enterprise == null)
            {
                // If enterprise isn't found send NotFound result
                return NotFound();
            }
            // Return founded enterprise
            return enterprise;
        }

        // PUT: api/Enterprise/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> PutEnterprise(int id, EnterpriseEditable enterpriseData)
        {
            // Create variable to store updated enterprise
            Enterprise editedEnterprise;
            try
            {
                // Use service to update and store enterprise
                editedEnterprise = await _enterpriseService.EditEnterprise(_context, HttpContext.User, enterpriseData, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the enterprise is not found return NotFound result
                return NotFound(ex.Message);
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
            // Return data from the updated enterprise
            return CreatedAtAction("PutEnterprise", new { id = editedEnterprise.Id }, editedEnterprise);
        }

        // POST: api/Enterprise
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<Enterprise>> PostEnterprise(EnterpriseEditable enterpriseData)
        {
            // Create variable to store created enterprise
            Enterprise createdEnterprise;

            try
            {
                // Use service to create and store enterprise
                createdEnterprise = await _enterpriseService.CreateEnterprise(_context, HttpContext.User, enterpriseData);
            }
            catch (KeyNotFoundException ex)
            {
                // If key of the enterprise is not found return NotFound result
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
            // Return data from the created enterprise
            return CreatedAtAction("PostEnterprise", new { id = createdEnterprise.Id }, createdEnterprise);
        }

        // DELETE: api/Enterprise/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> DeleteEnterprise(int id)
        {
            // Create variable to store deleted enterprise
            Enterprise deletedEnterprise;

            try
            {
                // Use service to obtain deleted enterprise and store it
                deletedEnterprise = await _enterpriseService.DeleteEnterprise(_context, HttpContext.User, id);
            }
            catch (KeyNotFoundException ex)
            {
                // If the enterprise is not founded
                return NotFound(ex.Message);
            }
            // Return message to indicate action was successful
            return Ok();
        }
    }
}
