/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{FormsModule}from"@angular/forms";import{DraggableModule}from"../draggable/draggable.module";import{SliderComponent}from"./slider.component";var SliderModule=function(){return function(){}}();export{SliderModule};SliderModule.decorators=[{type:NgModule,args:[{declarations:[SliderComponent],imports:[CommonModule,FormsModule,DraggableModule],exports:[SliderComponent]}]}],SliderModule.ctorParameters=function(){return[]};