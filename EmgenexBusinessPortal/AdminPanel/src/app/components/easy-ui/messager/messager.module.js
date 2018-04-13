/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{LinkButtonModule}from"../linkbutton/linkbutton.module";import{MessagerService}from"./messager.service";import{MessagerComponent}from"./messager.component";import{MessagerBodyTemplateDirective}from"./messager-bodytemplate.directive";var MessagerModule=function(){return function(){}}();export{MessagerModule};MessagerModule.decorators=[{type:NgModule,args:[{declarations:[MessagerComponent,MessagerBodyTemplateDirective],imports:[CommonModule,FormsModule,LinkButtonModule],exports:[MessagerComponent,MessagerBodyTemplateDirective],providers:[MessagerService]}]}],MessagerModule.ctorParameters=function(){return[]};