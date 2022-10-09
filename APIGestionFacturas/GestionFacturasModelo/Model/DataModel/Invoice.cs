using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Invoice: BaseEntity
    {
        // Generate default Invoice
        public Invoice() { }

        // Generate default Invoice, but replace values with data
        public Invoice(InvoiceEditable data)
        {
            Name = data.Name;
            TaxPercentage = (int)data.TaxPercentage;
            EnterpriseId = (int)data.EnterpriseId;
        }

        [Required]
        public string Name { get; set; } = String.Empty;                // Name of the invoice
        [Required]
        public float TaxPercentage { get; set; } = 20;                  // Tax percentage of the invoice

        public float TotalAmount { get; set; } = 0;                     // Total amount from every invoice line with tax applied

        [Required]
        public Enterprise Enterprise { get; set; } = new Enterprise();  // Enterprise of the invoice
        [ForeignKey("Enterprise")]
        public int EnterpriseId { get; set; }                           // Enterprise Id
    }
}
