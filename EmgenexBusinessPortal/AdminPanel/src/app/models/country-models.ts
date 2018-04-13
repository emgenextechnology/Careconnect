export class CountryModels {
    constructor(
        public Id: number = 0,
        public CountryCode: string = null,
        public CountryName: string = null,
        public IsActive: boolean = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}
export class CountryFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
