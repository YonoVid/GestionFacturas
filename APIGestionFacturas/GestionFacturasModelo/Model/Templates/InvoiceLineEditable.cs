using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceLineEditable
    {
        [StringLength(50)]
        public string? Item { get; set; } = string.Empty;
        [StringLength(50)]
        public int? Quantity { get; set; } = 0;
        [StringLength(50)]
        public float? ItemValue { get; set; } = 0.0f;
    }
}
