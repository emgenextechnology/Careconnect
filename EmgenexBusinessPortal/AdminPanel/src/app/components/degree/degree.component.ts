import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DegreeModels, DegreeFilter } from '../../models/degree-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-degree',
  templateUrl: './degree.component.html',
  styleUrls: ['./degree.component.css']
})
export class DegreeComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() degrees;
  newDegreeModel = new DegreeModels();
  searchFilter = new DegreeFilter();
  isMouseEnterRegistered: boolean = false;
  private degreeId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAlldegrees(this.searchFilter)
  }

  getAlldegrees(searchFilter: DegreeFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/degrees', searchFilter).subscribe(data => {
      if (this.degrees = data['Model']) {
        this.degrees = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAlldegrees(this.searchFilter);
  }

  openCreate() {
    this.newDegreeModel = new DegreeModels();
    this.newDegreeModel.IsActive = true;
  }

  create(degreemodel: DegreeModels) {
    this.httpClient.post('api/degrees/save', degreemodel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAlldegrees(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-degree-mod').modal('close');
      }
    });
    return false;
  }

  edit(degreeId: number) {
    this.httpClient.get('api/degrees/' + degreeId).subscribe(data => {
      this.newDegreeModel = data['Model'];
      this.newDegreeModel.Id = degreeId;
    });
  }

  update(degreeModel: DegreeModels) {
    this.httpClient.put('api/degrees/' + degreeModel.Id, degreeModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAlldegrees(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-degree-mod').modal('close');
      }
    });
    return false;
  }

  delete(degreeId: number) {
    this.degreeId = degreeId;
    return false;
  }

  deleteConfirm() {
    if (this.degreeId <= 0)
      return true;
    this.httpClient.delete('api/degrees/' + this.degreeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAlldegrees(this.searchFilter)
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
