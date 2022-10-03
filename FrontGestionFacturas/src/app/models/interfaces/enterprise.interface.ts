import { IBaseEntity } from "./base-entity.interface";
import { IInvoice } from "./invoice.interface";

export interface IEnterprise extends IBaseEntity
{
    name: string,
    invoices: IInvoice[]
}