export class Rolemodel {
    constructor(
        public Id: number = 0,
        public Name: string = null,
    ) { }
}

export class RoleFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 100,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}