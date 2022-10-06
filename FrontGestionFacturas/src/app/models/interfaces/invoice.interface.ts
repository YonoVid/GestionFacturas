import { IInvoiceLine } from "./invoiceLine.interface";
import { IBaseEntity } from "./base-entity.interface";
import { IEnterprise } from "./enterprise.interface";

export interface IInvoice extends IBaseEntity
{
    name : string,
    taxPercentage: number,
    invoiceLines: IInvoiceLine[],

    enterprise: IEnterprise,
    enterpriseId: number
}