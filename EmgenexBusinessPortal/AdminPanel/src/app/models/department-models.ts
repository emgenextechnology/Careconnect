export class DepartmentModels {
    public Id:number
}

export class DepartmentFilterModel {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 100,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
