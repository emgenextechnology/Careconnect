/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{BaseModule}from"../base/base.module";import{CalendarModule}from"../calendar/calendar.module";import{DateBoxComponent}from"./datebox.component";import{DateBoxCellTemplateDirective}from"./datebox-celltemplate.directive";var DateBoxModule=function(){return function(){}}();export{DateBoxModule};DateBoxModule.decorators=[{type:NgModule,args:[{declarations:[DateBoxComponent,DateBoxCellTemplateDirective],imports:[CommonModule,FormsModule,BaseModule,CalendarModule],exports:[DateBoxComponent,DateBoxCellTemplateDirective]}]}],DateBoxModule.ctorParameters=function(){return[]};