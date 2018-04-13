/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{BaseModule}from"../base/base.module";import{PaginationModule}from"../pagination/pagination.module";import{GridBaseModule}from"../gridbase/grid-base.module";import{DataGridComponent}from"./datagrid.component";import{DataGridBodyComponent}from"./datagrid-body.component";import{DataGridViewComponent}from"./datagrid-view.component";import{DataGridTableComponent}from"./datagrid-table.component";import{DataGridGroupTemplateDirective}from"./datagrid-grouptemplate.directive";import{DataGridDetailTemplateDirective}from"./datagrid-detailtemplate.directive";import{DataGridEditTemplateDirective}from"./datagrid-edittemplate.directive";var DataGridModule=function(){return function(){}}();export{DataGridModule};DataGridModule.decorators=[{type:NgModule,args:[{declarations:[DataGridComponent,DataGridBodyComponent,DataGridViewComponent,DataGridTableComponent,DataGridGroupTemplateDirective,DataGridDetailTemplateDirective,DataGridEditTemplateDirective],imports:[CommonModule,FormsModule,BaseModule,PaginationModule,GridBaseModule],exports:[DataGridComponent,DataGridBodyComponent,DataGridViewComponent,DataGridTableComponent,DataGridGroupTemplateDirective,DataGridDetailTemplateDirective,DataGridEditTemplateDirective]}]}],DataGridModule.ctorParameters=function(){return[]};