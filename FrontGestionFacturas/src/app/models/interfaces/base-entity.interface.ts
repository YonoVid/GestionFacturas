export interface IBaseEntity{
    id: number,
    createdBy: string,
    createdDate: Date,
    updateddBy: string,
    updatedDate: Date,
    deleteddBy: string,
    deletedDate: Date,
    isDeleted: boolean
}