using GestionFacturasModelo.Model.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGestionFacturas.Tests
{
    public static class MockDbHelper
    {
        public static List<User> GetUserList()
        {
            var user = new User
            {
                Id = 0,
                Name = "Example user",
                Email = "example@mail.com",
                Password = "pwd",
                Rol = UserRol.USER,
                CreatedBy = "Admin",
                CreatedDate = new DateTime(),
                IsDeleted = false
            };
            var users = new List<User>();
            users.Add(user);
            return users;
        }

        public static List<Enterprise> GetEnterpriseList()
        {
            var enterprise = new Enterprise
            {
                Id = 0,
                Name = "Example user",
                UserId = 0,
                CreatedBy = "Admin",
                CreatedDate = new DateTime(),
                IsDeleted = false,
                User = new User
                {
                    Id = 0,
                    Name = "Example user",
                    Email = "example@mail.com",
                    Password = "pwd",
                    Rol = UserRol.USER,
                    CreatedBy = "Admin",
                    CreatedDate = new DateTime(),
                    IsDeleted = false
                }
            };
            var enterprises = new List<Enterprise>();
            enterprises.Add(enterprise);
            return enterprises;
        }

        public static List<Invoice> GetInvoiceList()
        {
            var invoice = new Invoice
            {
                Id = 0,
                Name = "Example user",
                TaxPercentage = 20,
                TotalAmount = 1000,
                EnterpriseId = 0,
                CreatedBy = "Admin",
                CreatedDate = new DateTime(),
                IsDeleted = false,
                Enterprise = new Enterprise
                {
                    Id = 0,
                    Name = "Example user",
                    UserId = 0,
                    CreatedBy = "Admin",
                    CreatedDate = new DateTime(),
                    IsDeleted = false,
                    User = new User
                    {
                        Id = 0,
                        Name = "Example user",
                        Email = "example@mail.com",
                        Password = "pwd",
                        Rol = UserRol.USER,
                        CreatedBy = "Admin",
                        CreatedDate = new DateTime(),
                        IsDeleted = false
                    }
                }
            };
            var invoices = new List<Invoice>();
            invoices.Add(invoice);
            return invoices;
        }

        public static List<InvoiceLine> GetInvoiceLineList()
        {
            var invoiceLine = new InvoiceLine
            {
                Id = 0,
                Item = "Example item",
                Quantity = 1,
                ItemValue = 1000,
                InvoiceId = 0,
                Invoice = new Invoice
                {
                    Id = 0,
                    Name = "Example user",
                    TaxPercentage = 20,
                    TotalAmount = 1000,
                    EnterpriseId = 0,
                    CreatedBy = "Admin",
                    CreatedDate = new DateTime(),
                    IsDeleted = false,
                    Enterprise = new Enterprise
                    {
                        Id = 0,
                        Name = "Example user",
                        UserId = 0,
                        CreatedBy = "Admin",
                        CreatedDate = new DateTime(),
                        IsDeleted = false,
                        User = new User
                        {
                            Id = 0,
                            Name = "Example user",
                            Email = "example@mail.com",
                            Password = "pwd",
                            Rol = UserRol.USER,
                            CreatedBy = "Admin",
                            CreatedDate = new DateTime(),
                            IsDeleted = false
                        }
                    }
                }
            };
            var noInvoice = new InvoiceLine
            {
                Id = 1,
                Item = "No invoice",
                Quantity = 1,
                ItemValue = -1,
                InvoiceId = 1,
            };
            var invoiceLines = new List<InvoiceLine>();
            invoiceLines.Add(invoiceLine);
            invoiceLines.Add(noInvoice);
            return invoiceLines;
        }
    }
}
