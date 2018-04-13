export class DocumentTypesModel {
    constructor(
        public Id: number = 0,
        public DocumentType: string = null,
    ) { }
}

export class DocumentTypesFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 100,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
