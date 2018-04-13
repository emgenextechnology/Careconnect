/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{BaseModule}from"../base/base.module";import{PaginationModule}from"../pagination/pagination.module";import{DataListComponent}from"./datalist.component";import{DataListItemTemplateDirective}from"./datalist-itemtemplate.directive";var DataListModule=function(){return function(){}}();export{DataListModule};DataListModule.decorators=[{type:NgModule,args:[{declarations:[DataListComponent,DataListItemTemplateDirective],imports:[CommonModule,FormsModule,BaseModule,PaginationModule],exports:[DataListComponent,DataListItemTemplateDirective]}]}],DataListModule.ctorParameters=function(){return[]};