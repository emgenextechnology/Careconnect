// export class SalesTeamModel {
//     public Id: number
// }

export class SalesTeamModel {
    constructor(
        public Id: number = 0,
        public RepGroupName: string = null,
        public Description: string = null,
        public IsActive: boolean = null,
        // public SalesDirectorId: number = null,
        public SalesDirector: string = null,
        public OldId: number = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedBy: number = null,
        public UpdatedBy: number =null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",
        public RepGroupManagerIds: number[] = [],
        public SalesDirectorIds: number[] = [],
        public Managers: string[] = [],
        public SalesReps: string[] = [],
        public BusinessId: number = null,
    ) { }
}

export class SalesTeamFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
