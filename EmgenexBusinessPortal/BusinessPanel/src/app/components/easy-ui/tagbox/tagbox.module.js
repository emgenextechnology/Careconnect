/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{BaseModule}from"../base/base.module";import{DataListModule}from"../datalist/datalist.module";import{ComboBoxModule}from"../combobox/combobox.module";import{TagBoxComponent}from"./tagbox.component";var TagBoxModule=function(){return function(){}}();export{TagBoxModule};TagBoxModule.decorators=[{type:NgModule,args:[{declarations:[TagBoxComponent],imports:[CommonModule,FormsModule,BaseModule,DataListModule,ComboBoxModule],exports:[TagBoxComponent]}]}],TagBoxModule.ctorParameters=function(){return[]};