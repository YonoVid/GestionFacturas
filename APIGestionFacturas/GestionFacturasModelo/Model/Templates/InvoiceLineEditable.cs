using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceLineEditable
    {
        public string? Item { get; set; } = string.Empty;
        public int? Quantity { get; set; } = 0;
        public float? ItemValue { get; set; } = 0.0f;

        public int? InvoiceId { get; set; }
    }
}
