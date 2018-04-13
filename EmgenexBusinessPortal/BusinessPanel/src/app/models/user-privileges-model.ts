
export class UserPrivilegesModel {
    constructor(
        // public UserDepartments: UserDepartments = null,
        // public UserRoles: number[] = null,
        public UserDepartments: UserDepartments = null,
        public UserRoles: UserRoles = null,
        public Modules: Modules = null,
    ) { }
}
export class Modules {
    constructor(
        public ModuleName: string = null,
        public PrivilegesModel: PrivilegesModel = null,
    ) { }
}
export class UserDepartments {
    constructor(
        public DepartmentName: string = null,
    ) { }
}
export class UserRoles {
    constructor(
        public DepartmentName: string = null,
    ) { }
}
export class PrivilegesModel {
    constructor(
        public Id: number = 0,
        public TaskType: string = null,
        public Allow: boolean = null,
        public Deny: boolean = null,
        public UserId: number = 0,
    ) { }
}