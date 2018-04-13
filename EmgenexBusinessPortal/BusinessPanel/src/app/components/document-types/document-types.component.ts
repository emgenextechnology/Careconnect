import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { DocumentTypesModel, DocumentTypesFilter } from '../../models/document-types-model';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var $: any
@Component({
  selector: 'app-document-types',
  templateUrl: './document-types.component.html',
  styleUrls: ['./document-types.component.css']
})
export class DocumentTypesComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() DocumentTypeModel;
  searchFilter = new DocumentTypesFilter();
  newDocumentTypesModel = new DocumentTypesModel();
  isMouseEnterRegistered: boolean = false;
  private documentTypeId: number = 0;
  pageSize: number;
  pageEvent: PageEvent;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getDocumentType(this.searchFilter)
  }

  getDocumentType(searchFilter: DocumentTypesFilter, resetPager: boolean = true) {

    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/documentType', searchFilter).subscribe(data => {
      if (this.DocumentTypeModel = data['Model']) {
        this.DocumentTypeModel = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getDocumentType(this.searchFilter);
  }

  openCreate() {
    this.newDocumentTypesModel = new DocumentTypesModel();
  }

  create(documentTypesModel: DocumentTypesModel) {
    this.httpClient.post('api/documentType/save', documentTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getDocumentType(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-document-type-mod').modal('close');
      }
    });
    return false;
  }

  edit(documentTypeId: number) {
    this.httpClient.get('api/documentType/' + documentTypeId).subscribe(data => {
      this.newDocumentTypesModel = data['Model'];
      this.newDocumentTypesModel.Id = documentTypeId;
    });
  }

  update(documentTypesModel: DocumentTypesModel) {
    this.httpClient.put('api/documentType/' + documentTypesModel.Id, documentTypesModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getDocumentType(this.searchFilter)
        this.isMouseEnterRegistered = false;
        $('#edit-document-type-mod').modal('close');
      }
    });
    return false
  }

  delete(documentTypeId: number) {
    this.documentTypeId = documentTypeId;
    return false;
  }

  deleteConfirm() {
    if (this.documentTypeId <= 0)
      return true;
    this.httpClient.delete('api/documenttype/' + this.documentTypeId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getDocumentType(this.searchFilter)
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
