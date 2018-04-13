import { Component, OnInit, Input, ChangeDetectorRef, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ServiceModel, ServiceFilter, FtpInfo, ServiceToggle } from '../../models/service-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';
declare var $: any

@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.css']
})
export class ServicesComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() Services;
  serviceColor;
  ImportModes;
  Protocols;
  searchFilter = new ServiceFilter();
  newServiceModel = new ServiceModel();
  newServiceToggle = new ServiceToggle();
  isMouseEnterRegistered: boolean = false;
  private ServiceId: number = 0;
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private chRef: ChangeDetectorRef,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllServices(this.searchFilter)
  }

  getAllServices(searchFilter: ServiceFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/service', searchFilter).subscribe(data => {
      if (this.Services = data['Model']) {
        this.Services = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllServices(this.searchFilter);
  }

  openCreate() {
    this.httpClient.get('lookup/getallimportmodes').subscribe(data => {
      this.ImportModes = data['Model'].List
    })

    this.httpClient.get('lookup/getallprotocols').subscribe(data => {
      this.Protocols = data['Model'].List
    })

    this.newServiceModel = new ServiceModel();
    this.newServiceModel.FtpInfo = new FtpInfo();
    this.isMouseEnterRegistered = false;
  }

  create(serviceModel: ServiceModel) {
    serviceModel.ServiceColor = $('#createServiceColor').val();
    this.httpClient.post('api/service/save', serviceModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllServices(this.searchFilter)
        this.isMouseEnterRegistered = false;
        this.newServiceModel = new ServiceModel();
        $('#create-services-mod').modal('close');
      }
    });
    return false;
  }

  edit(ServiceId: number) {
    this.httpClient.get('api/service/' + ServiceId).subscribe(data => {
      this.newServiceModel = data['Model'];
      if (this.newServiceModel.FtpInfo == null) {
        this.newServiceModel.FtpInfo = new FtpInfo();
      }
      this.httpClient.get('lookup/getallimportmodes').subscribe(data => {
        this.ImportModes = data['Model'].List
      })

      this.httpClient.get('lookup/getallprotocols').subscribe(data => {
        this.Protocols = data['Model'].List
      })
      this.newServiceModel.Id = ServiceId;
    });
  }

  update(serviceModel: ServiceModel) {
    console.log(serviceModel)
    serviceModel.ServiceColor = $('#updateServiceColor').val();
    this.httpClient.put('api/service/' + serviceModel.Id, serviceModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllServices(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-service-mod').modal('close');
      }
    });
    return false
  }

  delete(serviceId: number) {
    this.ServiceId = serviceId;
    return false;
  }

  deleteConfirm() {
    if (this.ServiceId <= 0)
      return true;
    this.httpClient.delete('api/service/' + this.ServiceId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllServices(this.searchFilter)
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
    console.log(this.isMouseEnterRegistered)
    if (!this.isMouseEnterRegistered) {
      $('.dropdown-button').dropdown();
      this.isMouseEnterRegistered = true;
    }
  }

  replaceString(param: string, replace: string, replaceWith: string) {
    if (param == null)
      return '';
    return param.replace(replace, replaceWith);
  }

  toggleStatus(serviceId: number, event) {
    this.newServiceToggle.ServiceId = serviceId;
    this.newServiceToggle.Status = event.checked;
    this.httpClient.post('api/service/' + serviceId + '/setdefaultservice', this.newServiceToggle).subscribe(data => {
      this.newServiceToggle = new ServiceToggle();
    });
  }

  changeDetect() {
    console.log(333)
    this.chRef.detectChanges();
  }
}
