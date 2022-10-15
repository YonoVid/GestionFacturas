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
    public class InvoiceLineServiceTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly InvoiceLineService _service;

        private List<Invoice> invoices;
        private List<InvoiceLine> invoiceLines;
        private List<Enterprise> enterprises;
        private List<User> users;
        private InvoiceLineEditable editable;
        private string httpUser;

        private Mock<GestionFacturasContext> _context;
        private Mock<IHttpContextAccessor> _mockHttp;

        public InvoiceLineServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            invoices = MockDbHelper.GetInvoiceList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            enterprises = MockDbHelper.GetEnterpriseList();
            users = MockDbHelper.GetUserList();


            editable = new InvoiceLineEditable
            {
                Item = invoiceLines[0].Item,
                Quantity = invoiceLines[0].Quantity,
                ItemValue = invoiceLines[0].ItemValue,
                InvoiceId = invoiceLines[0].InvoiceId
            };

            httpUser = "Test identity";

            _context = new Mock<GestionFacturasContext>();

            _context.SetupGet(p => p.InvoiceLines).Returns(MockDbContext.GetQueryableMockDbSet<InvoiceLine>(invoiceLines));
            _context.SetupGet(p => p.Invoices).Returns(MockDbContext.GetQueryableMockDbSet<Invoice>(invoices));
            _context.SetupGet(p => p.Enterprises).Returns(MockDbContext.GetQueryableMockDbSet<Enterprise>(enterprises));
            _context.SetupGet(p => p.Users).Returns(MockDbContext.GetQueryableMockDbSet<User>(users));

            _context.Setup(p => p.SetModified(It.IsAny<InvoiceLine>()));
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

            _service = new InvoiceLineService(_context.Object, _mockHttp.Object);

            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            invoiceLines = MockDbHelper.GetInvoiceLineList();
            users = MockDbHelper.GetUserList();
        }

        // GET INVOICE LINES
        [Fact]
        public async void GetInvoiceLinesSuccessful()
        {
            var stored = _context.Object.InvoiceLines.FirstOrDefault();
            _testOutputHelper.WriteLine(stored.Id.ToString());

            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;
            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = _service.GetAvailableInvoiceLines();

            Assert.NotNull(response);

            _testOutputHelper.WriteLine(response.ToString());
            _testOutputHelper.WriteLine(response.ToList().ToString());
            _testOutputHelper.WriteLine(response.ToList()[0].ToString());

            Assert.Equal(invoiceLines.Where(i => i.InvoiceId == invoices[0].Id), response.ToList());
        }
        [Fact]
        public async void GetInvoiceLinesNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoiceLines();

            _testOutputHelper.WriteLine(response.Expression.ToString());
            _testOutputHelper.WriteLine(response.Count().ToString());
            Assert.Empty(response.ToList());
        }
        [Fact]
        public async void GetInvoiceLinesAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoiceLines();

            Assert.NotNull(response);
            Assert.Equal(invoiceLines, response.ToList());
        }
        [Fact]
        public async void GetInvoiceLinesNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);

            var response = Assert.Throws<NullReferenceException>(() => _service.GetAvailableInvoiceLines());

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }

        // GET INVOICE LINES
        [Fact]
        public async void GetAvailableInvoiceLinesSuccessful()
        {
            var stored = _context.Object.InvoiceLines.FirstOrDefault();
            _testOutputHelper.WriteLine(stored.Id.ToString());

            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;
            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = _service.GetAvailableInvoiceLines(enterprises[0].Id);

            Assert.NotNull(response);

            _testOutputHelper.WriteLine(response.ToString());
            _testOutputHelper.WriteLine(response.ToList().ToString());
            _testOutputHelper.WriteLine(response.ToList()[0].ToString());

            Assert.Equal(invoiceLines, response.ToList());
        }
        [Fact]
        public async void GetAvailableInvoiceLinesNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoiceLines(enterprises[0].Id);

            _testOutputHelper.WriteLine(response.Expression.ToString());
            _testOutputHelper.WriteLine(response.Count().ToString());
            Assert.Empty(response.ToList());
        }
        [Fact]
        public async void GetEnterpriseInvoiceLinesAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = _service.GetAvailableInvoiceLines(enterprises[0].Id);

            Assert.NotNull(response);
            Assert.Equal(invoiceLines, response.ToList());
        }
        [Fact]
        public async void GetEnterpriseInvoiceLinesNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);

            var response = Assert.Throws<NullReferenceException>(() => _service.GetAvailableInvoiceLines(enterprises[0].Id));

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }

        // GET INVOICE LINE
        [Fact]
        public async void GetInvoiceLineSuccessful()
        {
            var claims = _mockHttp.Object.HttpContext.User.Identity as ClaimsIdentity;

            Assert.Equal(users[0].Id.ToString(), claims.FindFirst("Id").Value);

            var response = await _service.GetAvailableInvoiceLine(invoiceLines[0].Id);

            Assert.NotNull(response);

            Assert.Equal(invoiceLines[0], response);
        }
        [Fact]
        public async void GetInvoiceLineNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableInvoiceLine(invoiceLines[0].Id));

            Assert.Equal("Linea de factura no encontrada", response.Message);
        }
        [Fact]
        public async void GetInvoiceLineAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await _service.GetAvailableInvoiceLine(invoiceLines[0].Id);

            Assert.NotNull(response);

            Assert.Equal(invoiceLines[0], response);
        }
        [Fact]
        public async void GetInvoiceLineNotFound()
        {
            _testOutputHelper.WriteLine(_context.Object.InvoiceLines!.Count().ToString());

            _testOutputHelper.WriteLine((await _context.Object.InvoiceLines!.FindAsync(invoiceLines[0].Id))!.Id.ToString());

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetAvailableInvoiceLine(-1));

            Assert.Equal("Linea de factura no encontrada", response.Message);
        }
        [Fact]
        public async void GetInvoiceLineNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.GetAvailableInvoiceLine(0));

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }

        // CREATE INVOICE LINE
        [Fact]
        public async void CreateInvoiceLineSuccessful()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity= 2, ItemValue = 100, InvoiceId = 0 };

            var response = await _service.CreateInvoiceLine(newInvoiceLine);

            Assert.Equal(newInvoiceLine.Item, response.Item);
            Assert.Equal(newInvoiceLine.Quantity, response.Quantity);
            Assert.Equal(newInvoiceLine.ItemValue, response.ItemValue);
            Assert.Equal(newInvoiceLine.InvoiceId, response.InvoiceId);
        }
        [Fact]
        public async void CreateInvoiceLineNullInvoiceLineDatabase()
        {
            _context.Setup(c => c.InvoiceLines).Returns<DbSet>(null);
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateInvoiceLine(newInvoiceLine));

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }
        [Fact]
        public async void CreateInvoiceLineNotEnoughData()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = null, Quantity = null, ItemValue = null, InvoiceId = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateInvoiceLine(newInvoiceLine));

            Assert.Equal("Faltan datos para generar la entidad", response.Message);
        }
        [Fact]
        public async void CreateInvoiceLineNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(null, "Basic"));

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.CreateInvoiceLine(newInvoiceLine));

            Assert.Equal("Identidad de petición es nula", response.Message);
        }
        [Fact]
        public async void CreateInvoiceLineAdministatorNoIdentity()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await _service.CreateInvoiceLine(newInvoiceLine);

            Assert.Equal(newInvoiceLine.Item, response.Item);
            Assert.Equal(newInvoiceLine.Quantity, response.Quantity);
            Assert.Equal(newInvoiceLine.ItemValue, response.ItemValue);
            Assert.Equal(newInvoiceLine.InvoiceId, response.InvoiceId);
        }
        [Fact]
        public async void CreateInvoiceLineEnterpriseNotFound()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.CreateInvoiceLine(newInvoiceLine));

            Assert.Equal("Id de factura no encontrado", response.Message);

        }

        // UPDATE INVOICE LINE
        [Fact]
        public async void UpdateInvoiceLineSuccessful()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await _service.EditInvoiceLine(newInvoiceLine, invoiceLines[0].Id);

            Assert.Equal(newInvoiceLine.Item, response.Item);
            Assert.Equal(newInvoiceLine.Quantity, response.Quantity);
            Assert.Equal(newInvoiceLine.ItemValue, response.ItemValue);
            Assert.Equal(newInvoiceLine.InvoiceId, response.InvoiceId);

            Assert.Equal(invoices[0].TotalAmount, response.Quantity * response.ItemValue);
        }
        [Fact]
        public async void UpdateInvoiceLineNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.EditInvoiceLine(newInvoiceLine, 0));

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }

        [Fact]
        public async void UpdateInvoiceLineNotEnoughData()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = null, Quantity = null, ItemValue = null, InvoiceId = null };

            var response = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.EditInvoiceLine(newInvoiceLine, invoiceLines[0].Id));

            Assert.Equal("No hay suficientes datos para modificar la entidad", response.Message);
        }
        [Fact]
        public async void UpdateInvoiceLineInvoiceNotFound()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = -1 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditInvoiceLine(newInvoiceLine, invoiceLines[0].Id));

            Assert.Equal("Nueva factura no encontrada", response.Message);
        }

        [Fact]
        public async void UpdateInvoiceLineNotFound()
        {
            var newInvoiceLine = new InvoiceLineEditable { Item = "New", Quantity = 2, ItemValue = 100, InvoiceId = 0 };

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.EditInvoiceLine(newInvoiceLine, -1));

            Assert.Equal("Linea de factura no encontrada", response.Message);
        }

        // DELETE INVOICE LINE
        [Fact]
        public async void DeleteInvoiceLineSuccessful()
        {
            var response = await _service.DeleteInvoiceLine(invoiceLines[0].Id);

            // editable has the stored values of invoice line item
            Assert.Equal(editable.Item, response.Item);
            Assert.Equal(editable.Quantity, response.Quantity);
            Assert.Equal(editable.ItemValue, response.ItemValue);
            Assert.Equal(editable.InvoiceId, response.InvoiceId);

            Assert.DoesNotContain(response, invoiceLines);

            Assert.True(invoices[0].TotalAmount < response.Quantity * response.ItemValue);
        }
        [Fact]
        public async void DeleteInvoiceLineNullInvoiceDatabase()
        {
            _context.Setup(c => c.Invoices).Returns<DbSet>(null);

            var response = await Assert.ThrowsAsync<NullReferenceException>(async () => await _service.DeleteInvoiceLine(0));

            Assert.Equal("Referencia a base de datos es nula", response.Message);
        }
        [Fact]
        public async void DeleteInvoiceLineNotFounded()
        {
            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteInvoiceLine(-1));

            Assert.Equal("Linea de factura no encontrada", response.Message);
        }
        [Fact]
        public async void DeleteInvoiceNotFounded()
        {
            var httpContext = new DefaultHttpContext();
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
            httpContext.User = new ClaimsPrincipal(claims);

            _mockHttp.SetupGet(_ => _.HttpContext).Returns(httpContext);

            var response = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.DeleteInvoiceLine(1));

            Assert.Equal("Factura asociada no encontrada", response.Message);
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
