using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceLineEditable
    {
        public string? Item { get; set; } = string.Empty;   // Item name or description
        public int? Quantity { get; set; } = 0;             // Quantity of the item
        public float? ItemValue { get; set; } = 0.0f;       // Value of the item

        public int? InvoiceId { get; set; }                 // Invoice of the lines Id
    }
}
