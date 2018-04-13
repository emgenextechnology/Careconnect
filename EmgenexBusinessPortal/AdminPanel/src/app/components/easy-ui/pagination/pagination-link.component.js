/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Component,Host,Inject,forwardRef}from"@angular/core";import{PaginationComponent}from"./pagination.component";var PaginationLinkComponent=function(){function PaginationLinkComponent(pagination){this.pagination=pagination}return Object.defineProperty(PaginationLinkComponent.prototype,"pages",{get:function(){var begin=this.pagination.pageNumber-Math.floor(this.pagination.links/2);begin<1&&(begin=1);var end=begin+this.pagination.links-1;end>this.pagination.pageCount&&(end=this.pagination.pageCount),(begin=end-this.pagination.links+1)<1&&(begin=1);for(var pp=[],i=begin;i<=end;i++)pp.push(i);return pp},enumerable:!0,configurable:!0}),PaginationLinkComponent.prototype.onClick=function(page){this.pagination.selectPage(page)},PaginationLinkComponent}();export{PaginationLinkComponent};PaginationLinkComponent.decorators=[{type:Component,args:[{selector:"eui-pagination-link",template:'\n\t\t<div class="pagination-links">\n\t\t\t<eui-linkbutton *ngFor="let page of pages" class="pagination-link" [selected]="page==pagination.pageNumber" [plain]="true" (click)="onClick(page)">{{page}}</eui-linkbutton>\n\t\t</div>\n\t',host:{class:"f-inline-row"}}]}],PaginationLinkComponent.ctorParameters=function(){return[{type:PaginationComponent,decorators:[{type:Host},{type:Inject,args:[forwardRef(function(){return PaginationComponent})]}]}]};