
export class LocationModel {
    constructor(
        public Id: number = 0,
        public PracticeName: string = null,
        public Line1: string = null,
        public Line2: string = null,
        public City: string = null,
        public State: string = null,
        public Zip: string = null,
        public AddressType: string = null,
        public LocationId: number = 0,
    ) { }
}

export class LocationFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
