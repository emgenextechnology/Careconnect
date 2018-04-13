/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{ComboBoxComponent}from"./combobox.component";import{ComboBoxItemTemplateDirective}from"./combobox-itemtemplate.directive";import{BaseModule}from"../base/base.module";import{DataListModule}from"../datalist/datalist.module";var ComboBoxModule=function(){return function(){}}();export{ComboBoxModule};ComboBoxModule.decorators=[{type:NgModule,args:[{declarations:[ComboBoxComponent,ComboBoxItemTemplateDirective],imports:[CommonModule,FormsModule,BaseModule,DataListModule],exports:[ComboBoxComponent,ComboBoxItemTemplateDirective]}]}],ComboBoxModule.ctorParameters=function(){return[]};