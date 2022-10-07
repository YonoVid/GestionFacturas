export interface IInvoiceLine
{
    id: number,
    item: string,
    quantity: number,
    itemValue: number,
    invoiceId: number,
    isEdit: boolean
}