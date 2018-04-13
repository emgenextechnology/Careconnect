import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NotificationTypesModel, NotificationTypesFilter } from '../../models/notification-types-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-notification-types',
  templateUrl: './notification-types.component.html',
  styleUrls: ['./notification-types.component.css']
})
export class NotificationTypesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() NotificationTypeModel
  newNotificationTypesModel = new NotificationTypesModel();
  searchFilter = new NotificationTypesFilter();
  isMouseEnterRegistered: boolean = false;
  private notificationTypeId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getNotificationType(this.searchFilter)
  }

  getNotificationType(searchFilter: NotificationTypesFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/notificationtype', searchFilter).subscribe(data => {
      if (this.NotificationTypeModel = data['Model']) {
        this.NotificationTypeModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getNotificationType(this.searchFilter);
  }

  openCreate() {
    this.newNotificationTypesModel = new NotificationTypesModel();
  }

  create(notificationTypesModel: NotificationTypesModel) {
    this.httpClient.post('api/notificationtype/save', notificationTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getNotificationType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-notification-type-mod').modal('close');
      }
    });
    return false;
  }

  edit(notificationTypeId: number) {
    this.httpClient.get('api/notificationtype/' + notificationTypeId).subscribe(data => {
      this.newNotificationTypesModel = data['Model'];
      this.newNotificationTypesModel.Id = notificationTypeId;
    });
  }

  update(notificationTypesModel: NotificationTypesModel) {
    this.httpClient.put('api/notificationtype/' + notificationTypesModel.Id, notificationTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getNotificationType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-notification-type-mod').modal('close');
      }
    });
    return false
  }

  delete(notificationTypeId: number) {
    this.notificationTypeId = notificationTypeId;
    return false;
  }

  deleteConfirm() {
    if (this.notificationTypeId <= 0)
      return true;
    this.httpClient.delete('api/notificationtype/' + this.notificationTypeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getNotificationType(this.searchFilter);
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
