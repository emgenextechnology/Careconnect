/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{LinkButtonModule}from"../linkbutton/linkbutton.module";import{PaginationButtonComponent}from"./pagination-button.component";import{PaginationListComponent}from"./pagination-list.component";import{PaginationLinkComponent}from"./pagination-link.component";import{PaginationTemplateDirective}from"./pagination-template.directive";import{PaginationComponent}from"./pagination.component";var PaginationModule=function(){return function(){}}();export{PaginationModule};PaginationModule.decorators=[{type:NgModule,args:[{declarations:[PaginationListComponent,PaginationLinkComponent,PaginationButtonComponent,PaginationTemplateDirective,PaginationComponent],imports:[CommonModule,FormsModule,LinkButtonModule],exports:[PaginationListComponent,PaginationLinkComponent,PaginationButtonComponent,PaginationTemplateDirective,PaginationComponent]}]}],PaginationModule.ctorParameters=function(){return[]};