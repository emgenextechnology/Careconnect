export class NotificationTypesModel {
    constructor(
        public Id: number = 0,
        public Title: string = null,
        public NotificationKey: string = null,
        public IsActive: boolean = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}
export class NotificationTypesFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
