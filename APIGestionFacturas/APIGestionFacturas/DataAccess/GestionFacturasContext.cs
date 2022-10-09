using Microsoft.EntityFrameworkCore;
using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.DataAccess
{
    public class GestionFacturasContext: DbContext
    {
        // Constructor that load options
        public GestionFacturasContext(DbContextOptions<GestionFacturasContext> options): base(options) { }

        // DbSet of every database table
        public DbSet<User>? Users { get; set; }
        public DbSet<Enterprise>? Enterprises { get; set; }
        public DbSet<Invoice>? Invoices { get; set; }
        public DbSet<InvoiceLine>? InvoiceLines { get; set; }
    }
}
