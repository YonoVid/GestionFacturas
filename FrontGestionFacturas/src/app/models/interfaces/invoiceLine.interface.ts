import { IBaseEntity } from "./base-entity.interface";

export interface IInvoiceLine extends IBaseEntity
{
    item: string,
    quantity: string,
    itemValue: number
}