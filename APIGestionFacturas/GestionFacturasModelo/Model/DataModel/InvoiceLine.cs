using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class InvoiceLine
    {
        // Generate default InvoiceLine
        public InvoiceLine() { }

        // Generate default InvoiceLine, but replace values with data
        public InvoiceLine(InvoiceLineEditable data)
        {
            Item = data.Item;
            Quantity = (int)data.Quantity;
            ItemValue = (float)data.ItemValue;
        }

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }                             // Primary key of the invoice line

        [Required, StringLength(50)]
        public string Item { get; set; } = string.Empty;        // Item name or description
        [Required, StringLength(50)]
        public int Quantity { get; set; } = 0;                  // Quanity of items
        [Required, StringLength(50)]
        public float ItemValue { get; set; } = 0.0f;            // Value of the item

        [Required]
        public Invoice Invoice { get; set; } = new Invoice();   // Invoice the line is from
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }                      // Invoice Id
    }
}
