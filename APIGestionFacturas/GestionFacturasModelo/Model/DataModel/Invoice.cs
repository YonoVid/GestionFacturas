using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Invoice: BaseEntity
    {
        [Required]
        public Enterprise Enterprise { get; set; } = new Enterprise();
        [ForeignKey("Enterprise")]
        public int EnterpriseId { get; set; }
        [Required]
        public int TaxPercentage { get; set; } = 20;
        [Required]
        public ICollection<InvoiceLine>? InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
