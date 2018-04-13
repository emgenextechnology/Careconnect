export class ServiceModel {
    constructor(
        public Id: number = 0,
        public ServiceColor: string = "",
        public FtpInfo: FtpInfo = null,
        public ServiceName: string = null,
        public ServiceDecription: string = null,
        public ImportMode: number = null,
        public ImportName: string = null,
        public CreatedOn: DateTimeFormat = null,
        public UpdatedOn: string = null,
        public CreatedByName: string = null,
        public UpdatedByName: string = null,
    ) { }
}

export class FtpInfo {
    public Host: string = null
    public Protocol: string = null
    public Username: string = null
    public Password: string = null
    public PortNumber: number = null
    public RemotePath: string = null
}

export class ServiceFilter {
    constructor(
        public KeyWords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}

export class ServiceToggle {
    constructor(
        public ServiceId: number = 0,
        public Status: boolean = false
    ) { }
}