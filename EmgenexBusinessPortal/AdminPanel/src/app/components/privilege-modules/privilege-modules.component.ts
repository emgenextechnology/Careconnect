import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PrivilegeModulesModels, PrivilegeModuleFilter } from '../../models/privilege-modules-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-privilege-modules',
  templateUrl: './privilege-modules.component.html',
  styleUrls: ['./privilege-modules.component.css']
})
export class PrivilegeModulesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() PrivilegeModuleModel;
  newPrivilegeModulesModel = new PrivilegeModulesModels();
  searchFilter = new PrivilegeModuleFilter();
  isMouseEnterRegistered: boolean = false;
  private privilegeModuleId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getPrivilegeModules(this.searchFilter)
  }

  getPrivilegeModules(searchFilter: PrivilegeModuleFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }

    this.httpClient.post('api/privilegemodules', searchFilter).subscribe(data => {
      if (this.PrivilegeModuleModel = data['Model']) {
        this.PrivilegeModuleModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getPrivilegeModules(this.searchFilter);
  }

  openCreate() {
    this.newPrivilegeModulesModel = new PrivilegeModulesModels();
  }

  create(privilegeModulesModel: PrivilegeModulesModels) {
    this.httpClient.post('api/privilegemodules/save', privilegeModulesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getPrivilegeModules(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-privilege-modules-mod').modal('close');
      }
    });
    return false;
  }


  edit(privilegeModuleId: number) {
    this.httpClient.get('api/privilegemodules/' + privilegeModuleId).subscribe(data => {
      this.newPrivilegeModulesModel = data['Model'];
      this.newPrivilegeModulesModel.Id = privilegeModuleId;
    });
  }

  update(privilegeModulesModel: PrivilegeModulesModels) {
    this.httpClient.put('api/privilegemodules/' + privilegeModulesModel.Id, privilegeModulesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getPrivilegeModules(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-privilege-modules-mod').modal('close');
      }
    });
    return false;
  }

  delete(privilegeModuleId: number) {
    this.privilegeModuleId = privilegeModuleId;
    return false;
  }

  deleteConfirm() {
    if (this.privilegeModuleId <= 0)
      return true;
    this.httpClient.delete('api/privilegemodules/' + this.privilegeModuleId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getPrivilegeModules(this.searchFilter);
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
