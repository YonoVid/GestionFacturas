using APIGestionFacturas.Controllers;
using APIGestionFacturas.Services;
using DinkToPdf;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace APIGestionFacturas.Tests.Controller
{
    public class InvoiceControllerTest : BaseControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly InvoiceController _controller;

        private readonly Mock<IInvoiceService> _invoiceServiceMock;
        private readonly Mock<IInvoiceLineService> _invoiceLineServiceMock;
        private readonly Mock<DinkToPdf.Contracts.IConverter> _converterServiceMock;

        private readonly int id = 0;
        private Invoice invoice;
        private InvoiceEditable editable;

        public InvoiceControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            id = 0;
            invoice = new Invoice
            {
                Id = id,
                Name = "Example INVOICE",
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            editable = new InvoiceEditable();

            _invoiceServiceMock = new();
            _invoiceLineServiceMock = new();
            _converterServiceMock = new();

            _controller = new InvoiceController(_invoiceServiceMock.Object,
                                                _invoiceLineServiceMock.Object,
                                                _converterServiceMock.Object,
                                                new JwtSettings());

            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public async void GetInvoicePdfSuccessful()
        {
            var pdfDocument = new HtmlToPdfDocument();
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .ReturnsAsync(invoice);
            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetInvoicePdf(id))
                .ReturnsAsync(pdfDocument);

            var response = await _controller.GetInvoicePdf(id);

            //_testOutputHelper.WriteLine((response as BadRequestObjectResult).Value.ToString());

            Assert.NotNull(response);

            Assert.IsType<FileContentResult>(response);
        }

        [Fact]
        public async void GetInvoicePdSearchfNotFounded()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetInvoicePdf(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void GetInvoicePdfNotFounded()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .ReturnsAsync(invoice);
            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetInvoicePdf(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetInvoicePdf(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void GetInvoicePdfSearchDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoicePdf(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult).StatusCode);
        }

        [Fact]
        public async void GetInvoicePdfDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .ReturnsAsync(invoice);
            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetInvoicePdf(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoicePdf(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult).StatusCode);
        }

        //GET ENTERPRISE INVOICES
        [Fact]
        public async void GetEnterpriseInvoicesSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableEnterpriseInvoices(id))
                .Returns((new[] { invoice }).AsQueryable());

            var response = await _controller.GetEnterpriseInvoices(id);

            _testOutputHelper.WriteLine(response.Value.ToString());

            Assert.NotNull(response);

            Assert.Equal(new[] { invoice }, response.Value);
        }

        [Fact]
        public async void GetEnterpriseInvoicesDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableEnterpriseInvoices(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetEnterpriseInvoices(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }

        /*
         * -----------------------
         *  CRUD OF THE ENTITIES
         * -----------------------
         */

        // GET INVOICE
        [Fact]
        public async void GetInvoiceSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .ReturnsAsync(invoice);

            var response = await _controller.GetInvoice(id);

            Assert.NotNull(response);

            Assert.Equal(invoice, response.Value);
        }

        [Fact]
        public async void GetInvoiceNotFounded()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.GetInvoice(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void GetInvoiceDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoice(id))
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoice(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        /// GET ENTERPRISES
        [Fact]
        public async void GetInvoicesSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoices())
                .Returns((new[] { invoice }).AsQueryable());

            var response = await _controller.GetInvoices();

            _testOutputHelper.WriteLine(response.Value.ToString());

            Assert.NotNull(response);

            Assert.Equal(new[] { invoice }, response.Value);
        }

        [Fact]
        public async void GetInvoicesDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.GetAvailableInvoices())
                .Throws(new NullReferenceException());

            var response = await _controller.GetInvoices();

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult).StatusCode);
        }
        //POST INVOICE
        [Fact]
        public async void PostInvoiceSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.CreateInvoice(editable))
                .ReturnsAsync(invoice);

            var response = await _controller.PostInvoice(editable);

            Assert.NotNull(response);

            Assert.Equal(invoice, (response.Result as ObjectResult)?.Value);
        }

        [Fact]
        public async void PostInvoiceNotFound()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.CreateInvoice(editable))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PostInvoice(editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response.Result);
        }

        [Fact]
        public async void PostInvoicesInvalidError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.CreateInvoice(editable))
                .Throws(new InvalidOperationException());

            var response = await _controller.PostInvoice(editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async void PostInvoiceDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.CreateInvoice(editable))
                .Throws(new NullReferenceException());

            var response = await _controller.PostInvoice(editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response.Result);
            Assert.Equal(500, (response.Result as ObjectResult)?.StatusCode);
        }

        //PUT INVOICE
        [Fact]
        public async void PutInvoiceSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.EditInvoice(editable, id))
                .ReturnsAsync(invoice);

            var response = await _controller.PutInvoice(id, editable);

            Assert.NotNull(response);

            Assert.Equal(invoice, (response as ObjectResult)?.Value);
        }

        [Fact]
        public async void PutInvoiceNotFound()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.EditInvoice(editable, id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.PutInvoice(id, editable);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void PutInvoiceInvalidError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.EditInvoice(editable, id))
                .Throws(new InvalidOperationException());

            var response = await _controller.PutInvoice(id, editable);

            Assert.NotNull(response);

            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async void PutInvoiceDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.EditInvoice(editable, id))
                .Throws(new NullReferenceException());

            var response = await _controller.PutInvoice(id, editable);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

        // DELETE INVOICE
        [Fact]
        public async void DeleteInvoiceSuccessful()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.DeleteInvoice(id))
                .ReturnsAsync(invoice);

            var response = await _controller.DeleteInvoice(id);

            Assert.NotNull(response);

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void DeleteInvoiceNotFounded()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.DeleteInvoice(id))
                .Throws(new KeyNotFoundException());

            var response = await _controller.DeleteInvoice(id);

            Assert.NotNull(response);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async void DeleteInvoiceDatabaseError()
        {
            AddAdminToken();

            _invoiceServiceMock.Setup(InvoiceService => InvoiceService.DeleteInvoice(id))
                .Throws(new NullReferenceException());

            var response = await _controller.DeleteInvoice(id);

            Assert.NotNull(response);

            Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, (response as ObjectResult)?.StatusCode);
        }

    }
}