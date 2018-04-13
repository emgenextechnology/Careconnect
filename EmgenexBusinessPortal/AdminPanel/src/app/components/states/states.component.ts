
import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StatesModels, StateFilter } from '../../models/states-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-states',
  templateUrl: './states.component.html',
  styleUrls: ['./states.component.css']
})

export class StatesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;

  @Input() StateModel;
  Countries;
  newStatesModels = new StatesModels();
  searchFilter = new StateFilter();
  isMouseEnterRegistered: boolean = false;
  private stateId: number = 0;
  public cssClassCountry: string = "";
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService,
  ) { }

  ngOnInit() {
    this.httpClient.get('lookup/getallcountries').subscribe(data => {
      this.Countries = data['Model'].List
    })
    this.getAllStates(this.searchFilter);
  }

  getAllStates(searchFilter: StateFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }

    this.httpClient.post('api/states', searchFilter).subscribe(data => {
      if (this.StateModel = data['Model']) {
        this.StateModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    });
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllStates(this.searchFilter);
  }

  openCreate() {
    this.newStatesModels = new StatesModels();
    this.newStatesModels.IsActive = true;
  }

  create(statesModels: StatesModels) {
    this.httpClient.post('api/states/save', statesModels).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllStates(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-states-mod').modal('close');
      }
    })
    return false;
  }

  edit(statesId: number) {
    this.httpClient.get('api/states/' + statesId).subscribe(data => {
      this.newStatesModels = data['Model'];
      this.newStatesModels.Id = statesId;
    });
  }

  dropChangeValidate(modelValue) {
    if (modelValue > 0)
      this.cssClassCountry = "ng-valid";
    else
      this.cssClassCountry = "ng-invalid";
  }

  update(statesModel: StatesModels) {
    this.httpClient.put('api/states/' + statesModel.Id, statesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllStates(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-states-mod').modal('close');
      }
    });
    return false;
  }

  delete(stateId: number) {
    this.stateId = stateId;
    return false;
  }

  deleteConfirm() {
    if (this.stateId <= 0)
      return true;

    this.httpClient.delete('api/states/' + this.stateId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllStates(this.searchFilter);
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
    // $('select').material_select();
  }

  mouseEnter() {
    if (!this.isMouseEnterRegistered) {
      $('.dropdown-button').dropdown();
      this.isMouseEnterRegistered = true;
    }
  }

}
