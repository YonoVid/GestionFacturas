export interface ITableFilter {
    enterprise: IFilter;
    options:string[];
    defaultValue:string;
}

export interface IFilter {
    name:string;
    options:string[];
    defaultValue:string;
}

export interface IFilterOption{
    name:string;
    value:string;
    isdefault:boolean;
}
