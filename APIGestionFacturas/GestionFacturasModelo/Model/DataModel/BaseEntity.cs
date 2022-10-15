using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class BaseEntity
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }                                 // Primary key
        public string CreatedBy { get; set; } = string.Empty;       // Name of user that created
        public DateTime CreatedDate { get; set; } = DateTime.Now;   // Date of creation
        public string? UpdatedBy { get; set; } = string.Empty;      // Name of last user that updated
        public DateTime? UpdatedDate { get; set; }                  // Date of last update
        public string? DeletedBy { get; set; } = string.Empty;      // Name of user that deleted
        public DateTime? DeletedDate { get; set; }                  // Date of deletion
        public bool IsDeleted { get; set; } = false;                // Boolean indicating if is deleted
    }
}
