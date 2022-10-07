using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Invoice: BaseEntity
    {
        public Invoice() { }
        public Invoice(InvoiceEditable data)
        {
            Name = data.Name;
            TaxPercentage = (int)data.TaxPercentage;
            EnterpriseId = (int)data.EnterpriseId;
        }

        [Required]
        public string Name { get; set; } = String.Empty;
        [Required]
        public float TaxPercentage { get; set; } = 20;

        public float TotalAmount { get; set; } = 0;

        [Required]
        public Enterprise Enterprise { get; set; } = new Enterprise();
        [ForeignKey("Enterprise")]
        public int EnterpriseId { get; set; }
    }
}
