using Microsoft.EntityFrameworkCore;
using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.DataAccess
{
    public class GestionFacturasContext: DbContext
    {
        public GestionFacturasContext() { }
        // Constructor that load options
        public GestionFacturasContext(DbContextOptions<GestionFacturasContext> options): base(options) { }

        // DbSet of every database table
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Enterprise>? Enterprises { get; set; }
        public virtual DbSet<Invoice>? Invoices { get; set; }
        public virtual DbSet<InvoiceLine>? InvoiceLines { get; set; }

        // Aditional indirection layer for testing
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
