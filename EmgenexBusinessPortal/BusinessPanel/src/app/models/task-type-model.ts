
export class TaskTypeModel {
    constructor(
        public Id: number = 0,
        public TaskType: string = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",        
    ) { }
}

export class TaskTypeFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
