
export class TaskTypeModel {
    constructor(
        public Id: number = 0,
        public TaskType: string = null,
    ) { }
}

export class TaskTypeFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 100,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
