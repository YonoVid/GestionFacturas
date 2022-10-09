using GestionFacturasModelo.Model.DataModel;
using System.Text;

namespace APIGestionFacturas.Helpers
{
    public static class TemplateGenerator
    {
        /// <summary>
        /// Generates a html string of a invoice document with the data of
        /// the enterprise. 
        /// </summary>
        /// <param name="enterprise"> Enterprise the invoice is from. </param>
        /// <param name="invoice"> Invoice used to create the document. </param>
        /// <param name="invoiceLines"> Data of the invoice items. </param>
        /// <returns> String with HTML structure showing all invoice. data
        /// inside a table </returns>
        public static string GetHTMLString(Enterprise enterprise,
                                           Invoice invoice,
                                           InvoiceLine[] invoiceLines)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
            <html>
              <head></head>
              <body>
                <div class='invoice-box'>
                  <table>
                    <tr class='top'>
                      <td colspan='4'>
                        <table>
                          <tr>
                            <td class='title' colspan='4'>
                              <img src = './images/logo.png' alt='FACTURA' style='width: 100%; max-width: 300px' />
							</td>
					      </tr>
					    </table>
				      </td>
				    </tr>
                    <tr class='information'>
                      <td colspan = '4'>
                        <table>
                          <tr>
                            <td>
						      <strong>Factura #:</strong> {0} <br />
						      <strong>Creado:</strong> {1} <br />
						      <strong>Emitido:</strong> {2}
						    </td>
                            <td colspan='2'></td>
                            <td>
                              {3}<br />
                              {4}<br />
                              {5}
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr class='heading'>
                      <td> Cantidad </td>
                      <td> Descripción </td>
                      <td> Valor </td>
                      <td> Subtotal </td>
                    </tr>
            ",
            invoice.Id,
            invoice.CreatedDate.ToShortDateString(),
            DateTime.Now.ToShortDateString(),
            enterprise.Name,
            enterprise.User?.Name,
            enterprise.User?.Email
            );

            float total = 0;

            foreach (var invoiceLine in invoiceLines)
            {
                sb.AppendFormat(@"
                      <tr class='item'>
                        <td> {0} </td>
                        <td> {1} </td>
                        <td > {2} </td>
                        <td> {3} </td>
                      </tr>
                    ", 
                    invoiceLine.Quantity,
                    invoiceLine.Item,
                    invoiceLine.ItemValue.ToString(".00"),
                    (invoiceLine.ItemValue * invoiceLine.Quantity).ToString(".00")
                );
                total += invoiceLine.Quantity * invoiceLine.ItemValue;
            }

            float taxes = total * invoice.TaxPercentage / 100;

            sb.AppendFormat(@"
                      <tr class='total'>
                        <td colspan='2'> </td>
                        <td> Subtotal </td>
                        <td> {0} </td>
                      </tr>
                      <tr class='total'>
                        <td colspan='2'> </td>
                        <td> Impuestos {1}% </td>
                        <td> {2} </td>
                      </tr>
                      <tr class='total'>
                        <td colspan='2'> </td>
                        <td> Total </td>
                        <td><h4> ${3} </h4></td>
                      </tr>
                   </table>
                 </div>
               </body>
             </html>
            ",
                total.ToString(".00"),
                invoice.TaxPercentage,
                taxes.ToString(".00"),
                (total + taxes).ToString(".00")
            );

            return sb.ToString();
        }
    }
}
