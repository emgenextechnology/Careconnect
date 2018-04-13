import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { TaskTypeModel, TaskTypeFilter } from '../../models/task-type-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-task-types',
  templateUrl: './task-types.component.html',
  styleUrls: ['./task-types.component.css']
})
export class TaskTypesComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() TaskTypeModel;
  searchFilter = new TaskTypeFilter();
  newTaskTypeModel = new TaskTypeModel();
  isMouseEnterRegistered: boolean = false;
  private taskTypeId: number = 0;
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getTaskType(this.searchFilter)
  }

  openCreate() {
    this.newTaskTypeModel = new TaskTypeModel();
  }

  create(taskTypeModel: TaskTypeModel) {
    this.httpClient.post('api/tasktype/save', taskTypeModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getTaskType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-task-type-mod').modal('close');
      }
    });
    return false;
  }

  getTaskType(searchFilter: TaskTypeFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }

    this.httpClient.post('api/tasktype', searchFilter).subscribe(data => {
      if (this.TaskTypeModel = data['Model']) {
        this.TaskTypeModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getTaskType(this.searchFilter);
  }

  edit(taskTypeId: number) {
    this.httpClient.get('api/tasktype/' + taskTypeId).subscribe(data => {
      this.newTaskTypeModel = data['Model'];
      this.newTaskTypeModel.Id = taskTypeId;
    });
  }

  update(taskTypeModel: TaskTypeModel) {
    this.httpClient.put('api/tasktype/' + taskTypeModel.Id, taskTypeModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getTaskType(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-task-type-mod').modal('close');
      }
    });
    return false
  }

  delete(taskTypeId: number) {
    this.taskTypeId = taskTypeId;
    return false;
  }

  deleteConfirm() {
    if (this.taskTypeId <= 0)
      return true;
    this.httpClient.delete('api/tasktype/' + this.taskTypeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getTaskType(this.searchFilter)
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
