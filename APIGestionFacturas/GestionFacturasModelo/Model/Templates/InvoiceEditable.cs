using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceEditable
    {
        public string? Name { get; set; }
        public int? TaxPercentage { get; set; }

        public int? EnterpriseId { get; set; }
        public ICollection<InvoiceLineEditable>? InvoiceLines { get; set; }
    }
}
