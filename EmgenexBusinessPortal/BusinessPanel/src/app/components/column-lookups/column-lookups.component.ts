import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { ColumnLookupModel, ColumnLookupFiter } from '../../models/column-lookup-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-column-lookups',
  templateUrl: './column-lookups.component.html',
  styleUrls: ['./column-lookups.component.css']
})
export class ColumnLookupsComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() ColumnLookups;
  searchFilter = new ColumnLookupFiter();
  newColumnLookupModel = new ColumnLookupModel();
  isMouseEnterRegistered: boolean = false;
  private columnLookupId: number = 0;
  serviceId: number;
  columnId: number;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getColumnLookup(this.searchFilter)
  }

  getColumnLookup(searchFilter: ColumnLookupFiter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.route.params.subscribe(params => {
      this.serviceId = params['id'];
      this.columnId = params['columnid'];
    });
    this.httpClient.post('api/service/' + this.columnId + '/columnlookup', searchFilter).subscribe(data => {
      if (this.ColumnLookups = data['Model']) {
        this.ColumnLookups = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getColumnLookup(this.searchFilter);
  }

  openCreate() {
    this.newColumnLookupModel = new ColumnLookupModel();
  }

  create(columnLookupModel: ColumnLookupModel) {
    this.route.params.subscribe(params => {
      columnLookupModel.ColumnId = params['columnid'];
    });

    this.httpClient.post('api/service/columnlookup/save', columnLookupModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getColumnLookup(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-column-lookup-mod').modal('close');
      }
    });
    return false;
  }


  edit(columnLookupId: number) {
    this.httpClient.get('api/service/columnlookup/' + columnLookupId).subscribe(data => {
      this.newColumnLookupModel = data['Model'];
      this.newColumnLookupModel.Id = columnLookupId;
    });
  }

  update(columnLookupModel: ColumnLookupModel) {
    this.httpClient.put('api/service/columnlookup/' + columnLookupModel.Id, columnLookupModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getColumnLookup(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-column-lookup-mod').modal('close');
      }
    });
    return false
  }

  delete(columnLookupId: number) {
    this.columnLookupId = columnLookupId;
    return false;
  }

  deleteConfirm() {
    if (this.columnLookupId <= 0)
      return true;
    this.httpClient.delete('api/service/columnlookup/' + this.columnLookupId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getColumnLookup(this.searchFilter)
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
