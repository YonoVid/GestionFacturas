using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class InvoiceLine: BaseEntity
    {
        public InvoiceLine() { }
        public InvoiceLine(InvoiceLineEditable data)
        {
            Item = data.Item;
            Quantity = (int)data.Quantity;
            ItemValue = (float)data.ItemValue;
        }

        [Required, StringLength(50)]
        public string Item { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public int Quantity { get; set; } = 0;
        [Required, StringLength(50)]
        public float ItemValue { get; set; } = 0.0f;

        [Required]
        public Invoice Invoice { get; set; } = new Invoice();
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
    }
}
