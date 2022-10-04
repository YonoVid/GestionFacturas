using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceEditable: BaseEntity
    {
        public ICollection<InvoiceLine>? InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
