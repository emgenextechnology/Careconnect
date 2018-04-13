export class BusinessProfile {
    constructor(
        public Id: number = 0,
        public BusinessName: string = "",
        public Description: string = "",
        public FirstName: string = "",
        public MiddleName: string = "",
        public LastName: string = "",
        public Email: string = "",
        public Phone: string = "",
        public IsActive: boolean = false,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}

export class BusinessFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
