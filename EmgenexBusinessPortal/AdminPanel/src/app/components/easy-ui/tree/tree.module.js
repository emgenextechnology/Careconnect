/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{NgModule}from"@angular/core";import{CommonModule}from"@angular/common";import{TreeComponent}from"./tree.component";import{TreeNodeComponent}from"./tree-node.component";import{TreeItemTemplateDirective}from"./tree-itemtemplate.directive";var TreeModule=function(){return function(){}}();export{TreeModule};TreeModule.decorators=[{type:NgModule,args:[{declarations:[TreeComponent,TreeNodeComponent,TreeItemTemplateDirective],imports:[CommonModule],exports:[TreeComponent,TreeNodeComponent,TreeItemTemplateDirective]}]}],TreeModule.ctorParameters=function(){return[]};