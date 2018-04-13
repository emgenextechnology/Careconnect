import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DepartmentFilter, DepartmentModel } from '../../models/department-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-departments',
  templateUrl: './departments.component.html',
  styleUrls: ['./departments.component.css']
})
export class DepartmentsComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() Departments;
  searchFilter = new DepartmentFilter();
  newDepartmentModel = new DepartmentModel();
  isMouseEnterRegistered: boolean = false;
  private departmentId: number = 0;
  Privileges;
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllDepartments(this.searchFilter)
  }

  getAllDepartments(searchFilter: DepartmentFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/departments', searchFilter).subscribe(data => {
      if (this.Departments = data['Model']) {
        this.Departments = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllDepartments(this.searchFilter);
  }

  openCreate() {
    this.newDepartmentModel = new DepartmentModel();
    this.newDepartmentModel.DepartmentPrivilege = [];
    this.httpClient.get('lookup/getallprivileges').subscribe(data => {
      this.Privileges = data['Model'].List
    })
  }

  create(departmentModel: DepartmentModel, privileges) {
    departmentModel.DepartmentPrivilege = privileges;
    this.httpClient.post('api/departments/save', departmentModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllDepartments(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#create-departments-mod').modal('close');
      }
    });
    return false;
  }

  edit(departmentId: number) {
    this.httpClient.get('api/departments/' + departmentId).subscribe(data => {
      this.newDepartmentModel = data['Model'];
      this.newDepartmentModel.Id = departmentId;
    });
  }

  update(departmentModel: DepartmentModel) {
    this.httpClient.put('api/departments/' + departmentModel.Id, departmentModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllDepartments(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-departments-mod').modal('close');
      }
    });
    return false
  }

  delete(departmentId: number) {
    this.departmentId = departmentId;
    return false;
  }

  deleteConfirm() {
    if (this.departmentId <= 0)
      return true;
    this.httpClient.delete('api/departments/' + this.departmentId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllDepartments(this.searchFilter)
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
