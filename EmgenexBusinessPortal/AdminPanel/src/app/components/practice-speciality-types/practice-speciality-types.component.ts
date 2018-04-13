import { Component, OnInit, Input, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PracticeSpecialityTypesModels, PracticeSpecialityTypesFilter } from '../../models/practice-speciality-types-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-practice-speciality-types',
  templateUrl: './practice-speciality-types.component.html',
  styleUrls: ['./practice-speciality-types.component.css']
})

export class PracticeSpecialityTypesComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() PracticeSpecialityTypes
  newPracticeSpecialityTypes = new PracticeSpecialityTypesModels();
  searchFilter = new PracticeSpecialityTypesFilter();
  isMouseEnterRegistered: boolean = false;
  private specialityTypeId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllPracticeSpecialityTypes(this.searchFilter)
  }

  getAllPracticeSpecialityTypes(searchFilter: PracticeSpecialityTypesFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/practiceSpeciality', searchFilter).subscribe(data => {
      if (this.PracticeSpecialityTypes = data['Model']) {
        this.PracticeSpecialityTypes = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllPracticeSpecialityTypes(this.searchFilter);
  }

  openCreate() {
    this.newPracticeSpecialityTypes = new PracticeSpecialityTypesModels();
    this.newPracticeSpecialityTypes.IsActive = true;
  }

  create(practiceSpecialityTypesModel: PracticeSpecialityTypesModels) {
    this.httpClient.post('api/practiceSpeciality/save', practiceSpecialityTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllPracticeSpecialityTypes(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-practicespecialitytypes-mod').modal('close');
      }
    });
    return false;
  }

  edit(specialityTypeId: number) {
    this.httpClient.get('api/practiceSpeciality/' + specialityTypeId).subscribe(data => {
      this.newPracticeSpecialityTypes = data['Model'];
      this.newPracticeSpecialityTypes.Id = specialityTypeId;
    });
  }

  update(practiceSpecialityTypesModel: PracticeSpecialityTypesModels) {
    this.httpClient.put('api/practiceSpeciality/' + practiceSpecialityTypesModel.Id, practiceSpecialityTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllPracticeSpecialityTypes(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-practicespecialitytypes-mod').modal('close');
      }
    });
    return false;
  }

  delete(specialityTypeId: number) {
    this.specialityTypeId = specialityTypeId;
    return false;
  }

  deleteConfirm() {
    if (this.specialityTypeId <= 0)
      return true;
    this.httpClient.delete('api/practiceSpeciality/' + this.specialityTypeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllPracticeSpecialityTypes(this.searchFilter);
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
        alert(data["Message"])
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
