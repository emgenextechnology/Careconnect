import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router'
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { EasyUIModule } from './components/easy-ui/easyui/easyui.module';

import { AppComponent, MessageServiceComponent } from './app.component';

import { BusinessProfileComponent } from './components/business-profile/business-profile.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { CountryComponent } from './components/country/country.component';
import { DegreeComponent } from './components/degree/degree.component';
import { LeadSourcesComponent } from './components/lead-sources/lead-sources.component';
import { PracticeSpecialityTypesComponent } from './components/practice-speciality-types/practice-speciality-types.component';
import { PracticeTypesComponent } from './components/practice-types/practice-types.component';
import { NotificationTypesComponent } from './components/notification-types/notification-types.component';
import { PrivilegeModulesComponent } from './components/privilege-modules/privilege-modules.component';
import { PrivilegesComponent } from './components/privileges/privileges.component';
import { StatesComponent } from './components/states/states.component';

import { Interceptor } from './services/interceptor';
import { LoaderService } from './services/loader/loader.service';
import { LoaderComponent } from './services/loader/loader.component';
import { MessageService } from './services/message/message.service';
import { MessageComponent } from './services/message/message.component';
import {
  MatSelect, MatSelectModule, MatDatepickerModule, MatDatepicker, MatNativeDateModule,
  MatInputModule, MatPaginatorModule, MatCheckboxModule, MatMenuModule, MatIconModule, MatSnackBarModule,
  MatSortModule
} from '@angular/material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,

    BusinessProfileComponent,
    DashboardComponent,
    CountryComponent,
    DegreeComponent,
    LeadSourcesComponent,
    PracticeSpecialityTypesComponent,
    PracticeTypesComponent,
    NotificationTypesComponent,
    PrivilegeModulesComponent,
    PrivilegesComponent,
    StatesComponent,

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

    NoopAnimationsModule,
    HttpClientModule,
    EasyUIModule,
    RouterModule.forRoot([{
      path: '',
      component: DashboardComponent,
      data: {
        title: "Dashboard"
      }
    }, {
      path: 'business-profile',
      component: BusinessProfileComponent,
      data: {
        title: "Manage Business Profiles",
        shortTitle: "Business Profiles"
      }
    }, {
      path: 'countries',
      component: CountryComponent,
      data: {
        title: "Manage Countries",
        shortTitle: "Countries"
      }
    }, {
      path: 'degrees',
      component: DegreeComponent,
      data: {
        title: "Manage Degrees",
        shortTitle: "Degrees"
      }
    }, {
      path: 'leadsources',
      component: LeadSourcesComponent,
      data: {
        title: "Manage Lead Sources",
        shortTitle: "Lead Sources"
      }
    }, {
      path: 'practice-specialitytypes',
      component: PracticeSpecialityTypesComponent,
      data: {
        title: "Manage Practice Speciality Types",
        shortTitle: "Practice Speciality Types"
      }
    }, {
      path: 'practice-types',
      component: PracticeTypesComponent,
      data: {
        title: "Manage Practice Types",
        shortTitle: "Practice Types"
      }
    }, {
      path: 'notification-types',
      component: NotificationTypesComponent,
      data: {
        title: "Manage Notification Types",
        shortTitle: "Notification Types"
      }
    }, {
      path: 'privilege-modules',
      component: PrivilegeModulesComponent,
      data: {
        title: "Manage Privilege Modules",
        shortTitle: "Privilege Modules"
      }
    }, {
      path: 'privileges',
      component: PrivilegesComponent,
      data: {
        title: "Manage Privileges",
        shortTitle: "Privileges"
      }
    }, {
      path: 'states',
      component: StatesComponent,
      data: {
        title: "Manage State",
        shortTitle: "States"
      }
    }])
  ],
  providers: [
    Title,
    LoaderService,
    MessageService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: Interceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent],
})

export class AppModule { }
