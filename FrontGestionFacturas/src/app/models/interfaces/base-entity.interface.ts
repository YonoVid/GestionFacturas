export interface IBaseEntity{
    id: number,
    createdBy: string,
    createdDate: Date,
    updatedBy: string,
    updatedDate: Date,
    deletedBy: string,
    deletedDate: Date,
    isDeleted: boolean
}