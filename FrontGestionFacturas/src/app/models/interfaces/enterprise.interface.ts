import { IBaseEntity } from "./base-entity.interface";
import { IUser } from "./user.interface";

export interface IEnterprise extends IBaseEntity
{
    name: string,
    user: IUser,
    userId: number
}