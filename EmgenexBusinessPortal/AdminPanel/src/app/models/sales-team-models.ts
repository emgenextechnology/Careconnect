export class SalesTeamModels {
}

export class SalesFilterModels {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 100,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
