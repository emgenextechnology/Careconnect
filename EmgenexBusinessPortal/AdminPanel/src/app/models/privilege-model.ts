export class PrivilegeModel {
    constructor(
        public Id: number = 0,
        public Title: string = null,
        public Description: string = null,
        public PrivilegeKey: string = null,
        public Module: string = null,
        public ModuleId:number=null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}

export class PrivilegeFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
