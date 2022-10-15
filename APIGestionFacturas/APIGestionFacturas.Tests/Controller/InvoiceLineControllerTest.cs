using APIGestionFacturas.Controllers;
using APIGestionFacturas.Services;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Controller
{
    public class InvoiceLineControllerTest : BaseControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly InvoiceLineController _controller;

        private readonly Mock<IInvoiceLineService> _invoiceLineServiceMock;

        private readonly int id = 0;
        private InvoiceLine invoiceLine;
        private InvoiceLineEditable editable;

        public InvoiceLineControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            id = 0;
            invoiceLine = new InvoiceLine
            {
                Id = id,
                Item = "example",
                Quantity = 1,
                ItemValue = 100,
                InvoiceId = 0
            };

            editable = new InvoiceLineEditable();

            _invoiceLineServiceMock = new();

            _controller = new InvoiceLineController(_invoiceLineServiceMock.Object, new JwtSettings());

            _testOutputHelper = testOutputHelper;
        }

        /*
         * -----------------------
         *  CRUD OF THE ENTITIES
         * -----------------------
         */

        // GET ENTERPRISE
        [Fact]
        public async void GetInvoiceLineSuccessful()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.GetAvailableInvoiceLine(id))
                .ReturnsAsync(invoiceLine);

            var response = await _controller.GetInvoiceLine(id);

            Assert.NotNull(response);

            Assert.Equal(invoiceLine, response.Value);
        }

        [Fact]
        public async void GetInvoiceLineNotFounded()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.GetAvailableInvoiceLine(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetInvoiceLine(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void GetInvoiceLineDatabaseError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.GetAvailableInvoiceLine(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoiceLine(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        /// GET ENTERPRISES
        [Fact]
        public async void GetInvoiceLinesSuccessful()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.GetAvailableInvoiceLines())
                .Returns((new[] { invoiceLine }).AsQueryable());

            var response = await _controller.GetInvoiceLines();

            _testOutputHelper.WriteLine(response.Value.ToString());

            Assert.NotNull(response);

            Assert.Equal(new[] { invoiceLine }, response.Value);
        }

        [Fact]
        public async void GetInvoiceLinesDatabaseError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.GetAvailableInvoiceLines())
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoiceLines();

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        //POST ENTERPRISE
        [Fact]
        public async void PostInvoiceLineSuccessful()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.CreateInvoiceLine(editable))
                .ReturnsAsync(invoiceLine);

            var response = await _controller.PostInvoiceLine(editable);

            Assert.NotNull(response);

            Assert.Equal(invoiceLine, (response.Result as ObjectResult)?.Value);
        }

        [Fact]
        public async void PostInvoiceLineNotFound()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.CreateInvoiceLine(editable))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PostInvoiceLine(editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void PostInvoiceLineInvalidError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.CreateInvoiceLine(editable))
                .Throws(new InvalidOperationException());

            var response = await _controller.PostInvoiceLine(editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async void PostInvoiceLineDatabaseError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.CreateInvoiceLine(editable))
                .Throws(new NullReferenceException());

            var response = await _controller.PostInvoiceLine(editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult)?.StatusCode);
        }

        //PUT ENTERPRISE
        [Fact]
        public async void PutInvoiceLineSuccessful()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.EditInvoiceLine(editable, id))
                .ReturnsAsync(invoiceLine);

            var response = await _controller.PutInvoiceLine(id, editable);

            Assert.NotNull(response);

            Assert.Equal(invoiceLine, (response as ObjectResult)?.Value);
        }

        [Fact]
        public async void PutInvoiceLineNotFound()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.EditInvoiceLine(editable, id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PutInvoiceLine(id, editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void PutInvoiceLineInvalidError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.EditInvoiceLine(editable, id))
                .Throws(new InvalidOperationException());

            var response = await _controller.PutInvoiceLine(id, editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async void PutInvoiceLineDatabaseError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.EditInvoiceLine(editable, id))
                .Throws(new NullReferenceException());

            var response = await _controller.PutInvoiceLine(id, editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

        // DELETE ENTERPRISE
        [Fact]
        public async void DeleteInvoiceLineSuccessful()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.DeleteInvoiceLine(id))
                .ReturnsAsync(invoiceLine);

            var response = await _controller.DeleteInvoiceLine(id);

            Assert.NotNull(response);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteInvoiceLineNotFounded()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.DeleteInvoiceLine(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.DeleteInvoiceLine(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void DeleteInvoiceLineDatabaseError()
        {
            AddAdminToken();

            _invoiceLineServiceMock.Setup(invoiceLineService => invoiceLineService.DeleteInvoiceLine(id))
                .Throws(new NullReferenceException());

            var response = await _controller.DeleteInvoiceLine(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

    }
}