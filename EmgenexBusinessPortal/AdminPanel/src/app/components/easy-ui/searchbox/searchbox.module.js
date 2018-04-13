/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{MenuButtonModule}from"../menubutton/menubutton.module";import{SearchBoxComponent}from"./searchbox.component";var SearchBoxModule=function(){return function(){}}();export{SearchBoxModule};SearchBoxModule.decorators=[{type:NgModule,args:[{declarations:[SearchBoxComponent],imports:[CommonModule,FormsModule,MenuButtonModule],exports:[SearchBoxComponent]}]}],SearchBoxModule.ctorParameters=function(){return[]};