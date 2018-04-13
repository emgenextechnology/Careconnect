import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LeadSourceModels, LeadSourceFilter } from '../../models/lead-source-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-lead-sources',
  templateUrl: './lead-sources.component.html',
  styleUrls: ['./lead-sources.component.css']
})
export class LeadSourcesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() leadSources;
  newLeadSourceModel = new LeadSourceModels();
  searchFilter = new LeadSourceFilter();
  isMouseEnterRegistered: boolean = false;
  private leadSourceId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllLeadSource(this.searchFilter)
  }

  getAllLeadSource(searchFilter: LeadSourceFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/leadSources', searchFilter).subscribe(data => {
      if (this.leadSources = data['Model']) {
        this.leadSources = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllLeadSource(this.searchFilter);
  }

  openCreate() {
    this.newLeadSourceModel = new LeadSourceModels();
    this.newLeadSourceModel.IsActive = true;
  }

  create(leadSourceModels: LeadSourceModels) {
    this.httpClient.post('api/leadSources/save', leadSourceModels).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllLeadSource(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-leadsource-mod').modal('close');
      }
    })
    return false;
  }

  edit(leadSourceId: number) {
    this.httpClient.get('api/leadSources/' + leadSourceId).subscribe(data => {
      this.newLeadSourceModel = data['Model'];
      this.newLeadSourceModel.Id = leadSourceId;
    });
  }

  update(leadSourceModel: LeadSourceModels) {
    this.httpClient.put('api/leadSources/' + leadSourceModel.Id, leadSourceModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllLeadSource(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-leadsource-mod').modal('close');
      }
    });
    return false;
  }

  delete(leadSourceId: number) {
    this.leadSourceId = leadSourceId;
    return false;
  }

  deleteConfirm() {
    if (this.leadSourceId <= 0)
      return true;
    this.httpClient.delete('api/leadSources/' + this.leadSourceId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllLeadSource(this.searchFilter)
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
