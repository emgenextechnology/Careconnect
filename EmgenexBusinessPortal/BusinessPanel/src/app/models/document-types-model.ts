export class DocumentTypesModel {
    constructor(
        public Id: number = 0,
        public DocumentType: string = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",        
    ) { }
}

export class DocumentTypesFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
