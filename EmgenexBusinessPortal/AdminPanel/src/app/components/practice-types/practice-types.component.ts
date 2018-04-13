import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PracticeTypesModel, PracticeTypeFilter } from '../../models/practice-types-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-practice-types',
  templateUrl: './practice-types.component.html',
  styleUrls: ['./practice-types.component.css']
})
export class PracticeTypesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() PracticeTypeModel
  newPracticeTypesModel = new PracticeTypesModel();
  searchFilter = new PracticeTypeFilter();
  isMouseEnterRegistered: boolean = false;
  private practiceTypeId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllPracticeType(this.searchFilter)
  }

  getAllPracticeType(searchFilter: PracticeTypeFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/practicetype', searchFilter).subscribe(data => {
      if (this.PracticeTypeModel = data['Model']) {
        this.PracticeTypeModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllPracticeType(this.searchFilter);
  }

  openCreate() {
    this.newPracticeTypesModel = new PracticeTypesModel();
    this.newPracticeTypesModel.IsActive = true;
  }

  create(practiceTypesModel: PracticeTypesModel) {
    this.httpClient.post('api/practicetype/save', practiceTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllPracticeType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-Practice-Types-mod').modal('close');
      }
    });
    return false;
  }

  edit(practiceTypeId: number) {
    this.httpClient.get('api/practicetype/' + practiceTypeId).subscribe(data => {
      this.newPracticeTypesModel = data['Model'];
      this.newPracticeTypesModel.Id = practiceTypeId;
    });
  }

  update(practiceTypesModel: PracticeTypesModel) {
    this.httpClient.put('api/practicetype/' + practiceTypesModel.Id, practiceTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllPracticeType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-Practice-Types-mod').modal('close');
      }
    });
    return false;
  }

  delete(practiceTypeId: number) {
    this.practiceTypeId = practiceTypeId;
    return false;
  }

  deleteConfirm() {
    if (this.practiceTypeId <= 0)
      return true;
    this.httpClient.delete('api/practicetype/' + this.practiceTypeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllPracticeType(this.searchFilter);
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
