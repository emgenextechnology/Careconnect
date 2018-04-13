import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { EasyUIModule } from './components/easy-ui/easyui/easyui.module';
import {NgxMaskModule} from 'ngx-mask';

import { AppComponent, MessageServiceComponent } from './app.component';

import { BusinessProfileComponent } from './components/business-profile/business-profile.component';
import { ColumnLookupsComponent } from './components/column-lookups/column-lookups.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DepartmentsComponent } from './components/departments/departments.component';
import { DocumentTypesComponent } from './components/document-types/document-types.component';
import { LocationSettingsComponent } from './components/location-settings/location-settings.component';
import { TaskTypesComponent } from './components/task-types/task-types.component';
import { SalesRepsComponent } from './components/sales-reps/sales-reps.component';
import { SalesTeamComponent } from './components/sales-team/sales-team.component';
import { ServicesComponent } from './components/services/services.component';
import { ReportColumnsComponent } from './components/report-columns/report-columns.component';
import { RolesComponent } from './components/roles/roles.component';
import { UserPrivilegesComponent } from './components/user-privileges/user-privileges.component';
import { UsersComponent } from './components/users/users.component';

import { Interceptor } from './services/interceptor';
import { LoaderService } from './services/loader/loader.service';
import { LoaderComponent } from './services/loader/loader.component';
import { MessageService } from './services/message/message.service';
import { MessageComponent } from './services/message/message.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatSelect, MatSelectModule, MatDatepickerModule, MatDatepicker, MatNativeDateModule,
  MatInputModule, MatPaginatorModule, MatCheckboxModule, MatMenuModule, MatIconModule, MatSnackBarModule,
  MatSortModule
} from '@angular/material';
// import { MatDialogModule} from '@angular/material/dialog';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,

    DashboardComponent,
    DocumentTypesComponent,
    TaskTypesComponent,
    RolesComponent,
    DepartmentsComponent,
    SalesTeamComponent,
    SalesRepsComponent,
    LocationSettingsComponent,
    ServicesComponent,
    BusinessProfileComponent,
    ReportColumnsComponent,
    ColumnLookupsComponent,
    UsersComponent,
    UserPrivilegesComponent,

    LoaderComponent,
    MessageComponent,
    MessageServiceComponent,
  ],
  entryComponents: [
    MessageServiceComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    EasyUIModule,

    MatSelectModule,
    MatNativeDateModule,
    MatInputModule,
    MatDatepickerModule,
    MatPaginatorModule,
    MatCheckboxModule,
    MatMenuModule,
    MatIconModule,
    MatSnackBarModule,
    MatSortModule,
    // MatDialogModule,
    NgxMaskModule.forRoot(),

    NoopAnimationsModule,
    RouterModule.forRoot([{
      path: '',
      component: DashboardComponent,
      data: {
        title: "Dashboard"
      }
    }, {
      path: 'document-types',
      component: DocumentTypesComponent,
      data: {
        title: "Manage Document Types",
        shortTitle: "Document Types"
      }
    }, {
      path: 'task-types',
      component: TaskTypesComponent,
      data: {
        title: "Manage Task Types",
        shortTitle: "Task Types"
      }
    }, {
      path: 'roles',
      component: RolesComponent,
      data: {
        title: "Manage Roles",
        shortTitle: "Roles"
      }
    }, {
      path: 'sales-team',
      component: SalesTeamComponent,
      data: {
        title: "Manage Sales Teams",
        shortTitle: "Sales Teams"
      }
    }, {
      path: 'users',
      component: UsersComponent,
      data: {
        title: "Manage Users",
        shortTitle: "Users"
      }
    }, {
      path: 'users/:id/privileges',
      component: UserPrivilegesComponent,
      data: {
        title: "Manage Privileges",
        shortTitle: "Users > Privileges"
      }
    }, {
      path: 'sales-reps',
      component: SalesRepsComponent,
      data: {
        title: "Manage Sales Reps",
        shortTitle: "Sales Reps"
      }
    }, {
      path: 'services',
      component: ServicesComponent,
      data: {
        title: "Manage Services",
        shortTitle: "Services"
      }
    }, {
      path: 'services/:id/columns',
      component: ReportColumnsComponent,
      data: {
        title: "Manage Columns",
        shortTitle: "Services > Columns"
      }
    }, {
      path: 'services/:id/columns/:columnid/column-lookup',
      component: ColumnLookupsComponent,
      data: {
        title: "Manage ColumnLookup",
        shortTitle: "Services > Columns > ColumnLookups"
      }
    }, {
      path: 'business-profile',
      component: BusinessProfileComponent,
      data: {
        title: "Manage Business Profile",
        shortTitle: "Business Profile"
      }
    }, {
      path: 'departments',
      component: DepartmentsComponent,
      data: {
        title: "Manage Departments",
        shortTitle: "Departments"
      }
    }, {
      path: 'location-settings',
      component: LocationSettingsComponent,
      data: {
        title: "Location Settings",
      }
    }
    ])
  ],
  providers: [
    Title,
    LoaderService,
    MessageService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: Interceptor,
      multi: true
    },
    NgxMaskModule,
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
