import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { UserModel, UserFilter, SetPasswordModel } from '../../models/user-model';
import { HttpClient } from '@angular/common/http';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { FormControl } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})

export class UsersComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() Users: UserModel[];
  States;
  UserRoles;
  SalesTeamLookup;
  Departments;
  searchFilter = new UserFilter();
  newUserModel = new UserModel();
  newSetPasswordModel = new SetPasswordModel();
  isMouseEnterRegistered: boolean = false;
  private userId: number = 0;
  public cssClassCountry: string = "";
  pageSize: number;
  pageEvent: PageEvent;
  // date=new FormControl(new Date(2017,12,12));

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllUsers(this.searchFilter)
  }

  getAllUsers(searchFilter: UserFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }

    this.httpClient.post('api/users', searchFilter).subscribe(data => {
      if (this.Users = data['Model']) {
        this.Users = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllUsers(this.searchFilter);
  }

  openCreate() {
    this.newUserModel = new UserModel();
    this.newUserModel.UserRoles = null;
    this.newUserModel.UserDepartments = null;
    this.httpClient.get('lookup/GetAllRoles').subscribe(data => {
      this.UserRoles = data['Model'].List
    })
    this.httpClient.get('lookup/getallstates').subscribe(data => {
      this.States = data['Model'].List
    })
    this.httpClient.get('lookup/GetAllDepartments').subscribe(data => {
      this.Departments = data['Model'].List
    })
  }

  create(uerModel: UserModel, userRoles, userDepartments) {
    uerModel.UserRoles = userRoles;
    uerModel.UserDepartments = userDepartments;
    this.httpClient.post('api/users/save', uerModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllUsers(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#create-user-mod').modal('close');
      }
    });
    return false;
  }

  edit(userId: number) {
    this.newUserModel = new UserModel();
    this.newUserModel.UserRoles = null;
    this.newUserModel.UserDepartments = null;
    this.httpClient.get('lookup/getallstates').subscribe(data => {
      this.States = data['Model'].List
    })

    this.httpClient.get('api/users/' + userId).subscribe(data => {
      let model = data['Model'];
      this.newUserModel = model;
      this.newUserModel.StartDateCal = new FormControl(new Date(model.StartDate));
      if (model.StartDate)
        this.newUserModel.StartDateCal = new FormControl(new Date(model.StartDate));
      else {
        this.newUserModel.StartDateCal.reset();
      }
      this.newUserModel.Id = userId;
    });
  }

  update(userModel: UserModel) {
    // userModel.StartDate = new Date(userModel.StartDateCal.value.toISOString()).toString()
    var datePipe = new DatePipe('en-US');
    userModel.StartDate = datePipe.transform(userModel.StartDateCal.value, 'MM/dd/yyyy');

    // return false;
    this.httpClient.put('api/users/' + userModel.Id, userModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllUsers(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-users-mod').modal('close');
      }
    });
    return false
  }

  setPassword(setPasswordModel: SetPasswordModel) {
    setPasswordModel.UserId = $('#changePasswordUserId').val();
    this.httpClient.post('api/users/' + setPasswordModel.UserId + '/setpassword', setPasswordModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Password Successfully Changed", 200);
        this.getAllUsers(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#user-changepassword-mod').modal('close');
      }
    });
    return false
  }

  delete(userId: number) {
    this.userId = userId;
    return false;
  }

  deleteConfirm() {
    if (this.userId <= 0)
      return true;
    this.httpClient.delete('api/users/' + this.userId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllUsers(this.searchFilter)
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
      }
    });
    return false;
  }

  toggleStatus(userId: number) {
    this.httpClient.post('api/users/' + userId + '/togglestatus', userId).subscribe(data => {
    });
  }

  ngAfterViewInit() {
    $('.modal').modal();
  }

  mouseEnter() {
    if (!this.isMouseEnterRegistered) {
      $('.dropdown-button').dropdown();
      this.isMouseEnterRegistered = true;
    }
  }

  dropChangeValidate(modelValue) {
    if (modelValue > 0)
      this.cssClassCountry = "ng-valid";
    else
      this.cssClassCountry = "ng-invalid";
  }

  bindUsers(fireEvent) {
    if (fireEvent && this.SalesTeamLookup == null) {  
      this.httpClient.get('lookup/getallrepgroups').subscribe(data => {
        this.SalesTeamLookup = data['Model'].List
      })
    }
  }
}
