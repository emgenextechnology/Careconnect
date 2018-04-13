import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PrivilegeFilter, PrivilegeModel } from '../../models/privilege-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-privileges',
  templateUrl: './privileges.component.html',
  styleUrls: ['./privileges.component.css']
})
export class PrivilegesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() privileges
  modeules
  newPrivilegeModel = new PrivilegeModel();
  isMouseEnterRegistered: boolean = false;
  private privilegeId: number = 0;
  searchFilter = new PrivilegeFilter();
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.httpClient.get('api/privilege/getallprivilegemodules').subscribe(data => {
      this.modeules = data['Model'].List
    })
    this.getAllPrivilege(this.searchFilter)
  }

  getAllPrivilege(searchFilter: PrivilegeFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/privilege', searchFilter).subscribe(data => {
      if (this.privileges = data['Model']) {
        this.privileges = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllPrivilege(this.searchFilter);
  }

  openCreate() {
    this.newPrivilegeModel = new PrivilegeModel();
  }

  create(privilegModel: PrivilegeModel) {
    this.httpClient.post('api/privilege/save', privilegModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllPrivilege(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-privileges-mod').modal('close');
      }
    })
    return false;
  }

  edit(privilegeId: number) {
    this.httpClient.get('api/privilege/' + privilegeId).subscribe(data => {
      this.newPrivilegeModel = data['Model'];
      this.newPrivilegeModel.Id = privilegeId;
    });
  }

  update(privilegeModel: PrivilegeModel) {
    this.httpClient.put('api/privilege/' + privilegeModel.Id, privilegeModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllPrivilege(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-privileges-mod').modal('close');
      }
    });
    return false;
  }

  delete(privilegeId: number) {
    this.privilegeId = privilegeId;
    return false;
  }

  deleteConfirm() {
    if (this.privilegeId <= 0)
      return true;
    this.httpClient.delete('api/privilege/' + this.privilegeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllPrivilege(this.searchFilter);
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
