
import { FormControl } from '@angular/forms';

export class UserModel {
    constructor(
        public Id: number = 0,
        public FirstName: string = null,
        public MiddleName: string = null,
        public LastName: string = null,
        public UserName: string = null,
        public Email: string = null,
        public PhoneNumber: string = null,
        public WorkEmail: string = null,
        public HomePhone: string = null,
        public AdditionalPhone: string = null,
        public AddressLine1: string = null,
        public AddressLine2: string = null,
        public City: string = null,
        public StateId: number = null,
        public Zip: string = null,
        public RepGroupId: number = null,
        public UserDepartmentName: string = null,
        public StartDateCal: FormControl = null,
        public StartDate: string = null,
        public LastLoggedInTime: DateTimeFormat = null,
        public UserRoleNames: string[] = null,
        public UserRoles: UserRoles = null,
        public UserDepartments: UserDepartments = null,
    ) { }
}
export class UserRoles {
    constructor(
        public Disabled: boolean = false,
        public Group: string = null,
        public Selected: boolean = false,
        public Text: string = "",
        public Value: string = "",
    ) { }
}

export class UserDepartments {
    constructor(
        public Disabled: boolean = false,
        public Group: string = null,
        public Selected: boolean = false,
        public Text: string = "",
        public Value: string = "",
    ) { }
}

export class UserFilter {
    constructor(
        public Keywords: string = null,
        public pageSize: number = 25,
        public currentPage: number = 1,
        public SortKey: string = null,
        public SortOrder: string = null,
    ) { }
}
export class SetPasswordModel {
    constructor(
        public UserId: number = 0,
        public NewPassword: string = null,
        public ConfirmPassword: string = null
    ) { }
}