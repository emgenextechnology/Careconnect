export class Rolemodel {
    constructor(
        public Id: number = 0,
        public Name: string = null,
        public Description: string = null,
        public RolePrivilege: RolePrivilege[] = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public IsActive: boolean = false
    ) { }
}

export class RolePrivilege {
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

export class RoleFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}