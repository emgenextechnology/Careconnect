export class StatesModels {
    constructor(
        public Id: number = 0,
        public StateName: string = null,
        public StateCode: string = null,
        public IsActive: boolean = null,
        public CountryCode: string = null,
        public CountryId: number = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: DateTimeFormat = null,
        public CreatedUser: string = "",
        public Updateduser: string = "",
    ) { }
}

// export class CountryLookupModel {
//     constructor(
//         public Id: number = 0,
//         public CountryName: string = null,
//     ) { }
// }

export class StateFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}