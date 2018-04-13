import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LocationModel, LocationFilter } from '../../models/location-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-location-settings',
  templateUrl: './location-settings.component.html',
  styleUrls: ['./location-settings.component.css']
})
export class LocationSettingsComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() Locations;
  searchFilter = new LocationFilter();
  newLocationModel = new LocationModel();
  isMouseEnterRegistered: boolean = false;
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllLocations(this.searchFilter)
  }

  getAllLocations(searchFilter: LocationFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/location', searchFilter).subscribe(data => {
      if (this.Locations = data['Model']) {
        this.Locations = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllLocations(this.searchFilter);
  }

  edit(locationId: number) {
    this.httpClient.get('api/location/' + locationId).subscribe(data => {
      this.newLocationModel = data['Model'];
      this.newLocationModel.Id = locationId;
    });
  }

  update(locationModel: LocationModel) {
    this.httpClient.put('api/location/' + locationModel.Id, locationModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllLocations(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-location-mod').modal('close');
      }
    });
    return false
  }

  delete(locationId: number) {
    this.httpClient.delete('api/location/' + locationId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllLocations(this.searchFilter)
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
