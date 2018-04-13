export class ReportColumnModel {
    constructor(
        public Id: number = 0,
        public serviceId: number = 0,
        public ColumnName: string = null,
        public DisplayName: string = null,
        public ColumnType: number = 0,
        public InputType: number = 0,
        public IsMandatory: boolean = false,
        public DisplayInFilter: boolean = false,
        public ShowInGrid: boolean = false,
        public RolePrivilegeIds: number[] = [],
        public DepartmentPrivilegeIds: number[] = [],
        public UserPrivilegeIds: number[] = [],
        public ServiceName: string = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedByName: string = "",
        public UpdatedByName: string = "",
    ) { }
}

export class ReportColumnFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}