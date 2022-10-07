import { IEnterprise } from "../interfaces/enterprise.interface";
import { IInvoice } from "../interfaces/invoice.interface";
import { IInvoiceLine } from "../interfaces/invoiceLine.interface";

export interface IInvoiceData
{
  invoice: IInvoice,
  enterprise: IEnterprise,
  invoiceLines: IInvoiceLine[],
  total: number
}
