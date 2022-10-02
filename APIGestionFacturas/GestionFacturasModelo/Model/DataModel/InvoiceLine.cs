using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    public class InvoiceLine: BaseEntity
    {
        [Required, StringLength(50)]
        public string Item { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public int quantity { get; set; } = 0;
        [Required, StringLength(50)]
        public float ItemValue { get; set; } = 0.0f;
    }
}
