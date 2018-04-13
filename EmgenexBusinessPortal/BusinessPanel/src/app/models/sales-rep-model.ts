
// export class SalesRepModel {
//     public Id: number
// }

export class SalesRepModel {
    constructor(
        public Id: number = 0,
        public RepGroupName: string = null,
        public SalesDirector: string = null,
        public RepName: string = null,
        public IsActive: boolean = null,
        public OldId: number = null,
        public RepGroupId: number = null,
        public UserId: number = null,
        public SignonDate: DateTimeFormat = null,
        public CreatedBy: number = null,
        public UpdatedBy: number = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",
        public ServiceIds: number[] = [],
        public BusinessId: number = null,
        public SalesRepsComponent: SalesRepsComponent = null,
        public SelectedServiceNames: SelectedServiceNames = null
    ) { }
}

export class SelectedServiceNames {
    constructor(
        public Id: number = null,
        public IsSelected: boolean = false,
        public ParentId: number = null,
        public Selected: string = "",
        public Text: string = "",
        public Value: string = "",
    ) { }
}

export class SalesRepsComponent {
    constructor(
    ) { }
}

export class SalesRepFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}