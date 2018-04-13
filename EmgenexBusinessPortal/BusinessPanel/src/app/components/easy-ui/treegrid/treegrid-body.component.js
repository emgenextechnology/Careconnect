/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
var __extends=this&&this.__extends||function(){var extendStatics=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(d,b){d.__proto__=b}||function(d,b){for(var p in b)b.hasOwnProperty(p)&&(d[p]=b[p])};return function(d,b){function __(){this.constructor=d}extendStatics(d,b),d.prototype=null===b?Object.create(b):(__.prototype=b.prototype,new __)}}();import{Component,Inject,Host,forwardRef}from"@angular/core";import{GridBodyComponent}from"../gridbase/grid-body.component";import{TreeGridViewComponent}from"./treegrid-view.component";export var TREEGRID_BODY_TEMPLATE='\n\t<div #body class="datagrid-body f-full" (scroll)="onScroll($event)">\n\t\t<div #inner class="datagrid-body-inner">\n\t\t\t<div euiTreeGridChildren [rows]="rows" [columns]="columns">\n\t\t\t</div>\n\t\t</div>\n\t</div>\n';var TreeGridBodyComponent=function(_super){function TreeGridBodyComponent(view){var _this=_super.call(this)||this;return _this.view=view,_this}return __extends(TreeGridBodyComponent,_super),TreeGridBodyComponent}(GridBodyComponent);export{TreeGridBodyComponent};TreeGridBodyComponent.decorators=[{type:Component,args:[{selector:"eui-treegrid-body",template:TREEGRID_BODY_TEMPLATE,host:{class:"f-column f-full"}}]}],TreeGridBodyComponent.ctorParameters=function(){return[{type:TreeGridViewComponent,decorators:[{type:Host},{type:Inject,args:[forwardRef(function(){return TreeGridViewComponent})]}]}]};