
export class ColumnLookupModel {
    constructor(
        public Id: number = null,
        public ColumnId: number = null,
        public Text: string = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",
    ) { }
}

export class ColumnLookupFiter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}