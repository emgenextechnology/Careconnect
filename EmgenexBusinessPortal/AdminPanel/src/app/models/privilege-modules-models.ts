export class PrivilegeModulesModels {
    constructor(
        public Id: number = 0,
        public Title: string = null,
        public Description: string = null,
        public IsActive: boolean = false,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}
export class PrivilegeModuleFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
