import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReportColumnModel, ReportColumnFilter } from '../../models/report-column-model';
import { ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-report-columns',
  templateUrl: './report-columns.component.html',
  styleUrls: ['./report-columns.component.css']
})
export class ReportColumnsComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() ReportColumns;
  ReportRolePrivilegeLookup;
  ReportDepartmentPrivilegeLookup;
  ReportUserPrivilegeLookup;
  SalesColumnTypeLookup;
  SalesInputTypeLookup;
  serviceId: number;
  searchFilter = new ReportColumnFilter();
  newReportColumnModel = new ReportColumnModel();
  isMouseEnterRegistered: boolean = false;
  private columnId: number = 0;
  public cssClassRepGroups: string = "";
  reportRolePrivilegeLookup = new FormControl();
  reportDepartmentPrivilegeLookup = new FormControl();
  reportUserPrivilegeLookup = new FormControl();
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllReportColumns(this.searchFilter)
  }

  getAllReportColumns(searchFilter: ReportColumnFilter, resetPager: boolean = true) {
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
    });

    this.httpClient.post('api/service/' + this.serviceId + '/reportcolumn', searchFilter).subscribe(data => {
      if (this.ReportColumns = data['Model']) {
        this.ReportColumns = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllReportColumns(this.searchFilter);
  }

  openCreate() {
    this.httpClient.get('lookup/getallroles').subscribe(data => {
      this.ReportRolePrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getalldepartments').subscribe(data => {
      this.ReportDepartmentPrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallusersforservicecolumn').subscribe(data => {
      this.ReportUserPrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallsalescolumntype').subscribe(data => {
      this.SalesColumnTypeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallsalesinputtype').subscribe(data => {
      this.SalesInputTypeLookup = data['Model'].List
    })
    this.newReportColumnModel = new ReportColumnModel();
  }

  create(reportColumnModel: ReportColumnModel) {
    this.route.params.subscribe(params => {
      reportColumnModel.serviceId = params['id'];
    });

    this.httpClient.post('api/service/reportcolumn/save', reportColumnModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllReportColumns(this.searchFilter);
        $('#create-column-mod').modal('close');
      }
    });
    return false;
  }

  edit(columnId: number) {
    this.httpClient.get('lookup/getallroles').subscribe(data => {
      this.ReportRolePrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getalldepartments').subscribe(data => {
      this.ReportDepartmentPrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallusersforservicecolumn').subscribe(data => {
      this.ReportUserPrivilegeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallsalescolumntype').subscribe(data => {
      this.SalesColumnTypeLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallsalesinputtype').subscribe(data => {
      this.SalesInputTypeLookup = data['Model'].List
    })
    this.httpClient.get('api/service/reportcolumn/' + columnId).subscribe(data => {
      this.newReportColumnModel = data['Model'];
      this.newReportColumnModel.Id = columnId;
    });
  }

  update(reportColumnModel: ReportColumnModel) {
    this.httpClient.put('api/service/reportcolumn/' + reportColumnModel.Id, reportColumnModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllReportColumns(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-column-mod').modal('close');
      }
    });
    return false
  }

  delete(columnId: number) {
    this.columnId = columnId;
    return false;
  }

  deleteConfirm() {
    if (this.columnId <= 0)
      return true;
    this.httpClient.delete('api/service/reportcolumn/' + this.columnId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllReportColumns(this.searchFilter)
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

  repGroupDropChangeValidate(modelValue) {
    if (modelValue > 0)
      this.cssClassRepGroups = "ng-valid";
    else
      this.cssClassRepGroups = "ng-invalid";
  }
}
