import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Rolemodel, RoleFilter, RolePrivilege } from '../../models/rolemodel';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var $: any

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() RoleModel;
  searchFilter = new RoleFilter();
  newRoleModel = new Rolemodel();
  isMouseEnterRegistered: boolean = false;
  Privileges;
  pageSize: number;
  pageEvent: PageEvent;
  private roleId: number = 0;
  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllRoles(this.searchFilter)
  }

  getAllRoles(searchFilter: RoleFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/roles', searchFilter).subscribe(data => {
      if (this.RoleModel = data['Model']) {
        this.RoleModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllRoles(this.searchFilter);
  }

  openCreate() {
    this.newRoleModel = new Rolemodel();
    this.newRoleModel.RolePrivilege = [];
    this.httpClient.get('lookup/getallprivileges').subscribe(data => {
      this.Privileges = data['Model'].List
    })
  }

  create(rolemodel: Rolemodel, privileges) {
    rolemodel.RolePrivilege = privileges;
    this.httpClient.post('api/roles/save', rolemodel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllRoles(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-roles-mod').modal('close');
      }
    });
    return false;
  }

  edit(roleId: number) {
    this.httpClient.get('api/roles/' + roleId).subscribe(data => {
      this.newRoleModel = data['Model'];
      this.newRoleModel.Id = roleId;
    });
  }

  update(roleModel: Rolemodel) {
    this.httpClient.put('api/roles/' + roleModel.Id, roleModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllRoles(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-role-mod').modal('close');
      }
    });
    return false
  }

  delete(roleId: number) {
    this.roleId = roleId;
    return false;
  }
  deleteConfirm() {
    if (this.roleId <= 0)
      return true;
    this.httpClient.delete('api/roles/' + this.roleId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllRoles(this.searchFilter)
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
      }
    });
    return false;
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

}
