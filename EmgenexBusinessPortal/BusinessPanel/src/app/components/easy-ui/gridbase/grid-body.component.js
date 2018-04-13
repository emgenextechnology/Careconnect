/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Component,ViewChild,Input,Output,EventEmitter}from"@angular/core";import{domHelper}from"../base/domhelper";export var GRIDBASE_BODY_TEMPLATE='\n\t<div #body class="datagrid-body f-full" (scroll)="onScroll($event)">\n\t\t<div #inner class="datagrid-body-inner">\n\t\t</div>\n\t</div>\n';var GridBodyComponent=function(){function GridBodyComponent(){this.bodyScroll=new EventEmitter,this._scrollTop=0}return Object.defineProperty(GridBodyComponent.prototype,"scrollTop",{get:function(){return this._scrollTop},set:function(value){this._scrollTop=value,this.bodyRef.nativeElement.scrollTop=value},enumerable:!0,configurable:!0}),GridBodyComponent.prototype.onScroll=function(event){this.bodyScroll.emit({left:this.bodyRef.nativeElement.scrollLeft,top:this.bodyRef.nativeElement.scrollTop})},Object.defineProperty(GridBodyComponent.prototype,"scrollbarWidth",{get:function(){return domHelper.outerWidth(this.bodyRef.nativeElement)-domHelper.outerWidth(this.innerRef.nativeElement)},enumerable:!0,configurable:!0}),GridBodyComponent}();export{GridBodyComponent};GridBodyComponent.decorators=[{type:Component,args:[{selector:"eui-grid-body",template:GRIDBASE_BODY_TEMPLATE,host:{class:"f-column f-full"}}]}],GridBodyComponent.ctorParameters=function(){return[]},GridBodyComponent.propDecorators={bodyRef:[{type:ViewChild,args:["body"]}],innerRef:[{type:ViewChild,args:["inner"]}],columns:[{type:Input}],rows:[{type:Input}],bodyScroll:[{type:Output}]};