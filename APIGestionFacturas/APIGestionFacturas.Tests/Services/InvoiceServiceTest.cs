using APIGestionFacturas.DataAccess;
using APIGestionFacturas.Helpers;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Services
{
    public class InvoiceServiceTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly InvoiceLineService _invoiceLineService;
        private readonly InvoiceService _service;
        private readonly EnterpriseService _enterpriseService;
        private readonly UserService _userService;

        private List<Invoice> invoices;
        private List<InvoiceLine> invoiceLines;
        private List<Enterprise> enterprises;
        private List<User> users;
        private InvoiceEditable editable;
        private string httpUser;

        private Mock<GestionFacturasContext> _context;
        private Mock<IHttpContextAccessor> _mockHttp;

        public InvoiceServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            invoices = MockDbHelper.GetInvoiceList();
            enterprises = MockDbHelper.GetEnterpriseList();
            users = MockDbHelper.GetUserList();


            editable = new InvoiceEditable
            {
                Name = invoices[0].Name,
                TaxPercentage = invoices[0].TaxPercentage,
                EnterpriseId = invoices[0].EnterpriseId
            };

            httpUser = "Test identity";

            _context = new Mock<GestionFacturasContext>();

            _context.SetupGet(p => p.Invoices).Returns(MockDbContext.GetQueryableMockDbSet<Invoice>(invoices));
            _context.SetupGet(p => p.InvoiceLines).Returns(MockDbContext.GetQueryableMockDbSet<InvoiceLine>(invoiceLines));
            _context.SetupGet(p => p.Enterprises).Returns(MockDbContext.GetQueryableMockDbSet<Enterprise>(enterprises));
            _context.SetupGet(p => p.Users).Returns(MockDbContext.GetQueryableMockDbSet<User>(users));

            _context.Setup(p => p.SetModified(It.IsAny<Invoice>()));
            _context.Setup(p => p.SaveChanges()).Returns(1);
            _context.Setup(p => p.SaveChangesAsync(default)).Returns(Task.FromResult(1));


            _mockHttp = new();
            var httpContext = new DefaultHttpContext();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, httpUser));
            identity.AddClaim(new Claim("Id", users[0].Id.ToString()));

            httpContext.User = new ClaimsPrincipal(identity);

            httpContext.User.AddIdentity(identity);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            _userService = new UserService(_context.Object, _mockHttp.Object);
            _enterpriseService = new EnterpriseService(_context.Object, _mockHttp.Object);
            _invoiceLineService = new InvoiceLineService(_context.Object, _mockHttp.Object);
            _service = new InvoiceService(_context.Object, _mockHttp.Object, _userService, _enterpriseService, _invoiceLineService);

            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            invoices = MockDbHelper.GetInvoiceList();
            invoices = MockDbHelper.GetInvoiceList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            users = MockDbHelper.GetUserList();
        }

        // GET INVOICES
        [Fact]
        public async void GetInvoicesSuccessful()
        {
            var stored = _context.Object.Invoices.FirstOrDefault();
            _testOutputHelper.WriteLine(stored.IsDeleted.ToString());
            _testOutputHelper.WriteLine(stored.Id.ToString());

            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;
            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = _service.GetAvailableInvoices();


            Assert.NotNull(response);

            _testOutputHelper.WriteLine(response.ToString());
            _testOutputHelper.WriteLine(response.ToList().ToString());
            _testOutputHelper.WriteLine(response.ToList()[0].ToString());

            Assert.Equal(invoices, response.ToList());
        }
        [Fact]
        public async void GetInvoicesNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoices();

            _testOutputHelper.WriteLine(response.Expression.ToString());
            _testOutputHelper.WriteLine(response.Count().ToString());
            Assert.Empty(response.ToList());
        }
        [Fact]
        public async void GetInvoicesAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoices();

            Assert.NotNull(response);
            Assert.Equal(invoices, response.ToList());
        }
        [Fact]
        public async void GetInvoicesNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);

            var response = Assert.Throws<NullReferenceException>(() => _service.GetAvailableInvoices());

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // GET ENTERPRISE INVOICES
        [Fact]
        public async void GetEnterpriseInvoicesSuccessful()
        {
            var stored = _context.Object.Invoices.FirstOrDefault();
            _testOutputHelper.WriteLine(stored.IsDeleted.ToString());
            _testOutputHelper.WriteLine(stored.Id.ToString());

            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;
            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = _service.GetAvailableEnterpriseInvoices(enterprises[0].Id);

            Assert.NotNull(response);

            _testOutputHelper.WriteLine(response.ToString());
            _testOutputHelper.WriteLine(response.ToList().ToString());
            _testOutputHelper.WriteLine(response.ToList()[0].ToString());

            Assert.Equal(invoices, response.ToList());
        }
        [Fact]
        public async void GetEnterpriseInvoicesNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableEnterpriseInvoices(enterprises[0].Id);

            _testOutputHelper.WriteLine(response.Expression.ToString());
            _testOutputHelper.WriteLine(response.Count().ToString());
            Assert.Empty(response.ToList());
        }
        [Fact]
        public async void GetEnterpriseInvoicesAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableEnterpriseInvoices(enterprises[0].Id);

            Assert.NotNull(response);
            Assert.Equal(invoices, response.ToList());
        }
        [Fact]
        public async void GetEnterpriseInvoicesNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);

            var response = Assert.Throws<NullReferenceException>(() => _service.GetAvailableEnterpriseInvoices(enterprises[0].Id));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // GET INVOICE
        [Fact]
        public async void GetInvoiceSuccessful()
        {
            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;

            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = await _service.GetAvailableInvoice(invoices[0].Id);

            Assert.NotNull(response);

            Assert.Equal(invoices[0], response);
        }
        [Fact]
        public async void GetInvoiceNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableInvoice(invoices[0].Id));

            Assert.Equal("Factura no encontrada", response.Message);
        }
        [Fact]
        public async void GetInvoiceAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await _service.GetAvailableInvoice(invoices[0].Id);

            Assert.NotNull(response);

            Assert.Equal(invoices[0], response);
        }
        [Fact]
        public async void GetInvoiceNotFound()
        {
            _testOutputHelper.WriteLine(_context.Object.Invoices!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.Invoices!.FindAsync(invoices[0].Id))!.Id.ToString());

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableInvoice(-1));

            Assert.Equal("Factura no encontrada", response.Message);
        }
        [Fact]
        public async void GetInvoiceNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetAvailableInvoice(0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // CREATE INVOICE
        [Fact]
        public async void CreateInvoiceSuccessful()
        {
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await _service.CreateInvoice(newInvoice);

            Assert.Equal(newInvoice.Name, response.Name);
            Assert.Equal(newInvoice.TaxPercentage, response.TaxPercentage);
            Assert.Equal(newInvoice.EnterpriseId, response.EnterpriseId);
        }
        [Fact]
        public async void CreateInvoiceNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateInvoice(newInvoice));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void CreateInvoiceNullEnterprisesDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateInvoice(newInvoice));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void CreateInvoiceNotEnoughData()
        {
            var newInvoice = new InvoiceEditable { Name = null, TaxPercentage = null, EnterpriseId = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateInvoice(newInvoice));

            Assert.Equal("Faltan datos para generar la entidad", response.Message);
        }
        [Fact]
        public async void CreateInvoiceNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateInvoice(newInvoice));

            Assert.Equal("Identidad de petición es nula", response.Message);
        }
        [Fact]
        public async void CreateInvoiceAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await _service.CreateInvoice(newInvoice);

            Assert.Equal(newInvoice.Name, response.Name);
            Assert.Equal(newInvoice.TaxPercentage, response.TaxPercentage);
            Assert.Equal(newInvoice.EnterpriseId, response.EnterpriseId);
        }
        [Fact]
        public async void CreateInvoiceEnterpriseNotFound()
        {
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.CreateInvoice(newInvoice));

            Assert.Equal("Id de empresa no encontrado", response.Message);
        }

        // UPDATE INVOICE
        [Fact]
        public async void UpdateInvoiceSuccessful()
        {
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await _service.EditInvoice(newInvoice, invoices[0].Id);

            Assert.Equal(newInvoice.Name, response.Name);
            Assert.Equal(newInvoice.TaxPercentage, response.TaxPercentage);
            Assert.Equal(newInvoice.EnterpriseId, response.EnterpriseId);
        }
        [Fact]
        public async void UpdateInvoiceNullEnterpriseDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.EditInvoice(newInvoice, 0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        [Fact]
        public async void UpdateInvoiceNotEnoughData()
        {
            var newInvoice = new InvoiceEditable { Name = null, TaxPercentage = null, EnterpriseId = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.EditInvoice(newInvoice, invoices[0].Id));

            Assert.Equal("No hay suficientes datos para modificar la entidad", response.Message);
        }
        [Fact]
        public async void UpdateInvoiceEnterpriseNotFound()
        {
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditInvoice(newInvoice, invoices[0].Id));

            Assert.Equal("Empresa no encontrada", response.Message);
        }

        [Fact]
        public async void UpdateInvoiceNotFound()
        {
            var newInvoice = new InvoiceEditable { Name = "New", TaxPercentage = 19, EnterpriseId = 0 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditInvoice(newInvoice, -1));

            Assert.Equal("Factura no encontrada", response.Message);
        }

        // DELETE INVOICE
        [Fact]
        public async void DeleteInvoiceSuccessful()
        {
            var response = await _service.DeleteInvoice(invoices[0].Id);

            Assert.Equal(invoices[0].Name, response.Name);
            Assert.Equal(invoices[0].TaxPercentage, response.TaxPercentage);
            Assert.Equal(invoices[0].EnterpriseId, response.EnterpriseId);

            Assert.Equal(httpUser, response.DeletedBy);
            Assert.Equal(httpUser, invoices[0].DeletedBy);
            Assert.True(response.IsDeleted);
            Assert.True(invoices[0].IsDeleted);
        }
        [Fact]
        public async void DeleteInvoiceNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.DeleteInvoice(0));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void DeleteInvoiceNotFounded()
        {
            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteInvoice(-1));

            Assert.Equal("Factura no encontrada", response.Message);
        }

        // GET INVOICE PDF
        [Fact]
        public async void GetInvoicePdfSuccessful()
        {
            var response = await _service.GetInvoicePdf(invoices[0].Id);

            Assert.Equal(TemplateGenerator.GetHTMLString(enterprises[0], invoices[0], invoiceLines.ToArray()), response.Objects[0].HtmlContent);

        }
        [Fact]
        public async void GetInvoicePdfNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetInvoicePdf(invoices[0].Id));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void GetInvoicePdfNullEnterpriseDatabase()
        {
            _context.Setup(c => c.Enterprises).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetInvoicePdf(invoices[0].Id));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }
        [Fact]
        public async void GetInvoicePdfNullUserDatabase()
        {
            _context.Setup(c => c.Users).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetInvoicePdf(invoices[0].Id));

            Assert.Equal("Referencia a base de datos en nula", response.Message);
        }

        // BASE CHECKS
        [Fact]
        public void GetClaimsNull()
        {
            _mockHttp.SetupGet(_ => _.HttpContext.User).Returns((ClaimsPrincipal)null);

            Assert.Null(_mockHttp.Object.HttpContext.User);

            var response = Assert.Throws<BadHttpRequestException>(() => _service.getUserClaims());

            Assert.Equal("Datos de usuario que realiza la solicitud no encontrados", response.Message);
        }
    }
}
