import { IInvoiceLine } from "./invoiceLine.interface";
import { IBaseEntity } from "./base-entity.interface";

export interface IInvoice extends IBaseEntity
{
    invoiceLines: IInvoiceLine[]
}