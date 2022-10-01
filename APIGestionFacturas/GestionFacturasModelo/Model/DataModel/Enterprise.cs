using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Enterprise : BaseEntity
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
