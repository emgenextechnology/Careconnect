/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Component,ViewChild,Host,Inject,forwardRef}from"@angular/core";import{PaginationComponent}from"./pagination.component";var PaginationListComponent=function(){function PaginationListComponent(pagination){this.pagination=pagination}return PaginationListComponent.prototype.ngAfterViewInit=function(){this.listRef.nativeElement.value||(this.listRef.nativeElement.value=this.pagination.pageList[0]),this.pagination.pageSize=this.listRef.nativeElement.value},PaginationListComponent.prototype.onChange=function(event){event.stopPropagation(),this.pagination.pageSize=this.listRef.nativeElement.value},PaginationListComponent}();export{PaginationListComponent};PaginationListComponent.decorators=[{type:Component,args:[{selector:"eui-pagination-list",template:'\n\t\t<select #list class="pagination-page-list" (change)="onChange($event)">\n\t\t\t<option *ngFor="let page of pagination.pageList" [selected]="page==pagination.pageSize">{{page}}</option>\n\t\t</select>\n\t',host:{class:"f-inline-row"}}]}],PaginationListComponent.ctorParameters=function(){return[{type:PaginationComponent,decorators:[{type:Host},{type:Inject,args:[forwardRef(function(){return PaginationComponent})]}]}]},PaginationListComponent.propDecorators={listRef:[{type:ViewChild,args:["list"]}]};