import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SalesRepModel, SalesRepFilter } from '../../models/sales-rep-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { FormControl } from '@angular/forms';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-sales-reps',
  templateUrl: './sales-reps.component.html',
  styleUrls: ['./sales-reps.component.css']
})
export class SalesRepsComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() SalesReps;
  SalesRepLookup;
  SalesTeamLookup;
  ServicesLookup;
  searchFilter = new SalesRepFilter();
  newSalesRepsModel = new SalesRepModel();
  isMouseEnterRegistered: boolean = false;
  managers = new FormControl();
  private salesRepId: number = 0;
  public cssClassUsers: string = "";
  public cssClassRepGroups: string = "";
  pageSize: number;
  pageEvent: PageEvent;
  Managers;
  public repGroupManagers: string = "";

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllSalesReps(this.searchFilter)
  }

  getAllSalesReps(searchFilter: SalesRepFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/reps', searchFilter).subscribe(data => {
      if (this.SalesReps = data['Model']) {
        this.SalesReps = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllSalesReps(this.searchFilter);
  }

  openCreate() {
    this.newSalesRepsModel = new SalesRepModel();
    this.newSalesRepsModel.SelectedServiceNames = null;
    this.repGroupManagers = null;
    this.httpClient.get('lookup/getallrepservices').subscribe(data => {
      this.ServicesLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallmanagers').subscribe(data => {
      this.Managers = data['Model'].List
    })

    this.httpClient.get('lookup/getallusersforrep/0').subscribe(data => {
      this.SalesRepLookup = data['Model'].List
    })

    this.httpClient.get('lookup/getallrepgroups').subscribe(data => {
      this.SalesTeamLookup = data['Model'].List
    })
  }

  create(salesRepModel: SalesRepModel, services) {
    salesRepModel.SelectedServiceNames = services;
    this.httpClient.post('api/reps/save', salesRepModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllSalesReps(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-sales-rep-mod').modal('close');
      }
    });
    return false;
  }

  edit(salesRepId: number) {
    this.httpClient.get('api/reps/' + salesRepId).subscribe(data => {
      this.newSalesRepsModel = data['Model'];
      this.newSalesRepsModel.Id = salesRepId;
      this.repGroupManagers = null;

      if (this.newSalesRepsModel.RepGroupId != null)
        this.getManagerByGroup(this.newSalesRepsModel.RepGroupId)

      this.httpClient.get('lookup/getallusersforrep/' + this.newSalesRepsModel.UserId).subscribe(data => {
        this.SalesRepLookup = data['Model'].List
      })

      this.httpClient.get('lookup/getallrepservices').subscribe(data => {
        this.ServicesLookup = data['Model'].List
      })

      this.httpClient.get('lookup/getallmanagers').subscribe(data => {
        this.Managers = data['Model'].List
      })

      this.httpClient.get('lookup/getallrepgroups').subscribe(data => {
        this.SalesTeamLookup = data['Model'].List
      })
    });
  }

  update(salesRepModel: SalesRepModel) {
    salesRepModel.ServiceIds = null;
    this.httpClient.put('api/reps/' + salesRepModel.Id, salesRepModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllSalesReps(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-sales-rep-mod').modal('close');
      }
    });
    return false
  }

  delete(salesRepId: number) {
    this.salesRepId = salesRepId;
    return false;
  }

  deleteConfirm() {
    if (this.salesRepId <= 0)
      return true;
    this.httpClient.delete('api/reps/' + this.salesRepId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllSalesReps(this.searchFilter)
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
      }
    });
    return false;
  }

  getManagerByGroup(salesRepId: number) {
    this.httpClient.get('lookup/getallmanagersbyrepgroupid/' + salesRepId).subscribe(data => {
      this.repGroupManagers = data['Model'];
    })
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

  userDropChangeValidate(modelValue) {
    if (modelValue > 0)
      this.cssClassUsers = "ng-valid";
    else
      this.cssClassUsers = "ng-invalid";
  }

  repGroupDropChangeValidate(modelValue) {
    if (modelValue > 0)
      this.cssClassRepGroups = "ng-valid";
    else
      this.cssClassRepGroups = "ng-invalid";
  }
}
