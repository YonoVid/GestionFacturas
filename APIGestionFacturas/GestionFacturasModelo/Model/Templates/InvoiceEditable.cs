using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.Templates
{
    public class InvoiceEditable
    {
        public string? Name { get; set; }                                   // Invoice name
        public float? TaxPercentage { get; set; }                           // Ivoice tax percentage

        public int? EnterpriseId { get; set; }                              // Enterprise of the invoice Id
    }
}
