import { IBaseEntity } from "./base-entity.interface";
import { IEnterprise } from "./enterprise.interface";

export enum UserRol
{
    ADMINISTRATOR = 0,
    USER = 10
}

export interface IUser extends IBaseEntity{
    name: string,
    email: string,
    password: string
    userRol: UserRol
    enterprises: IEnterprise[]
}