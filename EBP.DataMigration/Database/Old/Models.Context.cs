﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EBP.DataMigration.Database.Old
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EPMDevEntities : DbContext
    {
        public EPMDevEntities()
            : base("name=EPMDevEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccessLevel> AccessLevels { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountService> AccountServices { get; set; }
        public virtual DbSet<AccountUploading> AccountUploadings { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }
        public virtual DbSet<Alert_Lead> Alert_Lead { get; set; }
        public virtual DbSet<Alert> Alerts { get; set; }
        public virtual DbSet<BoardCertification> BoardCertifications { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Degree> Degrees { get; set; }
        public virtual DbSet<Dept> Depts { get; set; }
        public virtual DbSet<Dept_Group> Dept_Group { get; set; }
        public virtual DbSet<EnrolledService> EnrolledServices { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<Fellowship> Fellowships { get; set; }
        public virtual DbSet<Formulation> Formulations { get; set; }
        public virtual DbSet<HelpItemOrder> HelpItemOrders { get; set; }
        public virtual DbSet<HelpPriority> HelpPriorities { get; set; }
        public virtual DbSet<HelpPriorityType> HelpPriorityTypes { get; set; }
        public virtual DbSet<HelpRequest> HelpRequests { get; set; }
        public virtual DbSet<HelpType> HelpTypes { get; set; }
        public virtual DbSet<InsurancePlan> InsurancePlans { get; set; }
        public virtual DbSet<InsurancePlanType> InsurancePlanTypes { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Lead> Leads { get; set; }
        public virtual DbSet<LeadSource> LeadSources { get; set; }
        public virtual DbSet<Licence> Licences { get; set; }
        public virtual DbSet<Marketing> Marketings { get; set; }
        public virtual DbSet<Note_Lead> Note_Lead { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<NoteType> NoteTypes { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<PgxReport1> PgxReport1 { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public virtual DbSet<PhoneType> PhoneTypes { get; set; }
        public virtual DbSet<Practice> Practices { get; set; }
        public virtual DbSet<Practice_SpecialityType> Practice_SpecialityType { get; set; }
        public virtual DbSet<PracticeContact> PracticeContacts { get; set; }
        public virtual DbSet<PracticeProvider> PracticeProviders { get; set; }
        public virtual DbSet<PracticeProviderLocation> PracticeProviderLocations { get; set; }
        public virtual DbSet<PracticeSpecialityType> PracticeSpecialityTypes { get; set; }
        public virtual DbSet<PracticeType> PracticeTypes { get; set; }
        public virtual DbSet<Privilege> Privileges { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Rep> Reps { get; set; }
        public virtual DbSet<RepGroup> RepGroups { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportStatu> ReportStatus { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<RepStatu> RepStatus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RxcReport> RxcReports { get; set; }
        public virtual DbSet<RxCScript> RxCScripts { get; set; }
        public virtual DbSet<RxcStatu> RxcStatus { get; set; }
        public virtual DbSet<RxPlanType> RxPlanTypes { get; set; }
        public virtual DbSet<ScriptType> ScriptTypes { get; set; }
        public virtual DbSet<SecurityLevel> SecurityLevels { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceStatusType> ServiceStatusTypes { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<tblGroup> tblGroups { get; set; }
        public virtual DbSet<tblLocation> tblLocations { get; set; }
        public virtual DbSet<tblService> tblServices { get; set; }
        public virtual DbSet<Toxicology> Toxicologies { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Access> Accesses { get; set; }
        public virtual DbSet<AccountAddress> AccountAddresses { get; set; }
        public virtual DbSet<AccountProvider> AccountProviders { get; set; }
        public virtual DbSet<AccountStatusType> AccountStatusTypes { get; set; }
        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<AcctNote> AcctNotes { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<ContactInfo> ContactInfoes { get; set; }
        public virtual DbSet<ContactNote> ContactNotes { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<HelpNote> HelpNotes { get; set; }
        public virtual DbSet<LeadContact> LeadContacts { get; set; }
        public virtual DbSet<LeadNote> LeadNotes { get; set; }
        public virtual DbSet<LeadPracticeProvider> LeadPracticeProviders { get; set; }
        public virtual DbSet<LeadStage> LeadStages { get; set; }
        public virtual DbSet<LeadStatu> LeadStatus { get; set; }
        public virtual DbSet<LeadTask> LeadTasks { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Provider_BoardCert> Provider_BoardCert { get; set; }
        public virtual DbSet<Provider_Fellowship> Provider_Fellowship { get; set; }
        public virtual DbSet<ProviderNote> ProviderNotes { get; set; }
        public virtual DbSet<Rep_Srvc_Map> Rep_Srvc_Map { get; set; }
        public virtual DbSet<Practice_serviceId> Practice_serviceId { get; set; }
        public virtual DbSet<PracticeAddress> PracticeAddresses { get; set; }
        public virtual DbSet<PatientPgxreportMap> PatientPgxreportMaps { get; set; }
    }
}