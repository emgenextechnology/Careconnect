import { Component, OnInit, AfterViewInit, Input, ViewChild } from '@angular/core';
import { BusinessFilter, BusinessProfile } from '../../models/business-profile';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Http } from '@angular/http';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  moduleId: module.id.toString(),
  selector: 'app-business-profile',
  templateUrl: './business-profile.component.html',
  styleUrls: ['./business-profile.component.css']
})

export class BusinessProfileComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() businesses;
  searchFilter = new BusinessFilter();
  newBusinessProfile = new BusinessProfile();
  isMouseEnterRegistered: boolean = false;
  private businessId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getBusinesses(this.searchFilter);
  }

  getBusinesses(searchFilter: BusinessFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/business', searchFilter).subscribe(data => {
      if (this.businesses = data['Model']) {
        this.businesses = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    });
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getBusinesses(this.searchFilter);
  }

  openCreate() {
    this.newBusinessProfile = new BusinessProfile();
  }

  create(businessProfile: BusinessProfile) {
    this.httpClient.post('api/business/save', businessProfile).subscribe(data => {
      if (data["Status"] = 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.newBusinessProfile = new BusinessProfile();
        this.getBusinesses(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-business-mod').modal('close');
      }
    });
    return false;
  }

  edit(businessId: number) {
    this.httpClient.get('api/business/' + businessId).subscribe(data => {
      this.newBusinessProfile = data['Model'];
      this.newBusinessProfile.Id = businessId;
    });
  }

  update(businessProfile: BusinessProfile) {
    this.httpClient.put('api/business/' + businessProfile.Id, businessProfile).subscribe(data => {
      if (data["Status"] = 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getBusinesses(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-business-mod').modal('close');
      }
    });
    return false;
  }

  delete(businessId: number) {
    this.businessId = businessId;
    return false;
  }

  deleteConfirm() {
    if (this.businessId <= 0)
      return true;
    this.httpClient.delete('api/business/' + this.businessId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getBusinesses(this.searchFilter);
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
        alert()
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