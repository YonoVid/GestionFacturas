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

            sb.Append(@"
            <html>
              <head></head>
              <body>
                <div class='invoice-box'>
                  <table>
                    <tr class='top'>
                      <td colspan='2'>
                        <table>
                          <tr>
                            <td class='title'>
                              <img src = './images/logo.png' alt='Company logo' style='width: 100%; max-width: 300px' />
							</td>
                            <td>
						      Invoice #: 123<br />
						      Created: January 1, 2015<br />
						      Due: February 1, 2015
						    </td>
					      </tr>
					    </table>
				      </td>
				    </tr>
                    <tr class='information'>
                      <td colspan = '2'>
                        <table>
                          <tr>
                            <td>
                              Sparksuite, Inc.<br />
                              12345 Sunny Road <br />
                              Sunnyville, TX 12345
                            </td>
                            <td>
                              Acme Corp.<br />
                              John Doe <br />
                              john@example.com
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr class='heading'>
                      <td> Payment Method </td>
                      <td> Check #</td>
				    </tr>
                    <tr class='details'>
					  <td>Check</td>
					  <td>1000</td>
				    </tr>
                    <tr class='heading'>
                      <td> Cantidad </td>
                      <td> Descripción </td>
                      <td> Valor </td>
                      <td> Subtotal </td>
                    </tr>
            ");

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
                    invoiceLine.ItemValue,
                    invoiceLine.ItemValue * invoiceLine.Quantity
                );
            }


            sb.Append(@"
                   </table>
                 </div>
               </body>
             </html>

            ");

            return sb.ToString();
        }
    }
}
