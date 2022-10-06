import { IBaseEntity } from "./base-entity.interface";

export interface IInvoiceLine extends IBaseEntity
{
    item: string,
    quantity: number,
    itemValue: number
}