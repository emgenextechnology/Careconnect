/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{BaseModule}from"../base/base.module";import{GridBaseModule}from"../gridbase/grid-base.module";import{PaginationModule}from"../pagination/pagination.module";import{TreeGridComponent}from"./treegrid.component";import{TreeGridBodyComponent}from"./treegrid-body.component";import{TreeGridViewComponent}from"./treegrid-view.component";import{TreeGridRowComponent}from"./treegrid-row.component";import{TreeGridChildrenComponent}from"./treegrid-children.component";var TreeGridModule=function(){return function(){}}();export{TreeGridModule};TreeGridModule.decorators=[{type:NgModule,args:[{declarations:[TreeGridComponent,TreeGridBodyComponent,TreeGridViewComponent,TreeGridRowComponent,TreeGridChildrenComponent],imports:[CommonModule,FormsModule,BaseModule,GridBaseModule,PaginationModule],exports:[TreeGridComponent,TreeGridBodyComponent,TreeGridViewComponent,TreeGridRowComponent,TreeGridChildrenComponent]}]}],TreeGridModule.ctorParameters=function(){return[]};