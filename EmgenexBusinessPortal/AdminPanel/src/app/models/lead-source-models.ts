export class LeadSourceModels {
    constructor(
        public Id: number = 0,
        public LeadSource: string = null,
        public IsActive: boolean = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}
export class LeadSourceFilter {
    constructor(
        public Keywords: string = "",
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
