
import { Component, OnInit, Input, AfterViewInit, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CountryModels, CountryFilter } from '../../models/country-models';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any
declare var $: any

@Component({
  selector: 'app-country',
  templateUrl: './country.component.html',
  styleUrls: ['./country.component.css']
})

export class CountryComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() countries;
  newCountryModel = new CountryModels();
  searchFilter = new CountryFilter();
  isMouseEnterRegistered: boolean = false
  private countryId: number = 0;
  pageEvent: PageEvent;
  pageSize: number;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.GetAllCountries(this.searchFilter)
  }

  GetAllCountries(searchFilter: CountryFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/countries', searchFilter).subscribe(data => {
      if (this.countries = data['Model']) {
        this.countries = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.GetAllCountries(this.searchFilter);
  }

  openCreate() {
    this.newCountryModel = new CountryModels();
    this.newCountryModel.IsActive = true;
  }

  create(countryModel: CountryModels) {
    this.httpClient.post('api/countries/save', countryModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.GetAllCountries(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-country-mod').modal('close');
      }
    });
    return false;
  }

  edit(countryId: number) {
    this.httpClient.get('api/countries/' + countryId).subscribe(data => {
      this.newCountryModel = data['Model'];
      this.newCountryModel.Id = countryId;
    });
  }

  update(countryModel: CountryModels) {
    this.httpClient.put('api/countries/' + countryModel.Id, countryModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);

        this.GetAllCountries(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-country-mod').modal('close');
      }
    });
    return false;
  }

  delete(countryId: number) {
    this.countryId = countryId;
    return false;
  }

  deleteConfirm() {
    if (this.countryId <= 0)
      return true;
    this.httpClient.delete('api/countries/' + this.countryId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.GetAllCountries(this.searchFilter)
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

