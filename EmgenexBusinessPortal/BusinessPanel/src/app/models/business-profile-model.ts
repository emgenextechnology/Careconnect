export class BusinessProfileModel {
    constructor(
        public BusinessName: string = null,
        public FilePath: string = null,
        public FullName: string = "",
        public Email: string = null,
        public Description: string = null,
        public Address: string = null,
        public About: string = null,
        public DomainUrl: string = null,
        public RelativeUrl: string = null,
        public City: string = null,
        public State: string = null,
        public Country: string = null,
        public OtherEmails: string = null,
        public Logo: string = null,
        public IsActive: boolean = false,
        public SalesGroupBy: number = null,
        public DateRange: number[] = [],
        public SalesGroup: number[] = [],
        public LogoUrl: string = null,
        public LogoPath: string = null,
    ) { }
}
