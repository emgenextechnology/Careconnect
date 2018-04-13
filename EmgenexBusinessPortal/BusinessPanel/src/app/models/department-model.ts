// export class DepartmentModel {
//     public Id:number
// }

export class DepartmentModel {
    constructor(
        public Id: number = 0,
        public DepartmentName: string = null,
        public Description: string = null,
        public IsActive: boolean = null,
        public StatusId: number = null,
        public OldId: number = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedBy: string = "",
        public UpdatedBy: string = "",
        public CreatedByName: string = "",
        public UpdatedByName: string = "",
        public DepartmentPrivilegeIds: number[][] = [],
        public Users: string[][] = [],
        public BusinessId: number = null,
        // public DepartmentPrivilege: DepartmentPrivilege = null
        public DepartmentPrivilege: DepartmentPrivilege[] = null,
    ) { }
}

// export class DepartmentPrivilege {
//     constructor(
//     ) { }
// }
export class DepartmentPrivilege {
    constructor(
        public ModuleName: string = "",
        public Privileges: Privileges = null) { }
}

export class Privileges {
    constructor(
        public Id: number = null,
        public IsSelected: boolean = false,
        public ParentId: number = null,
        public Selected: string = "",
        public Text: string = "",
        public Value: string = "",
    ) { }
}
export class DepartmentFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
