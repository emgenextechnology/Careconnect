/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{DraggableModule}from"../draggable/draggable.module";import{DroppableDirective}from"./droppable.directive";var DroppableModule=function(){return function(){}}();export{DroppableModule};DroppableModule.decorators=[{type:NgModule,args:[{declarations:[DroppableDirective],imports:[CommonModule,DraggableModule],exports:[DroppableDirective]}]}],DroppableModule.ctorParameters=function(){return[]};