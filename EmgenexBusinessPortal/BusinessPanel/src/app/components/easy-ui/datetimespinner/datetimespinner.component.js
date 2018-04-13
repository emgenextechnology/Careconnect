/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
var __extends=this&&this.__extends||function(){var extendStatics=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(d,b){d.__proto__=b}||function(d,b){for(var p in b)b.hasOwnProperty(p)&&(d[p]=b[p])};return function(d,b){function __(){this.constructor=d}extendStatics(d,b),d.prototype=null===b?Object.create(b):(__.prototype=b.prototype,new __)}}();import{Component,forwardRef,Input}from"@angular/core";import{NG_VALUE_ACCESSOR}from"@angular/forms";import{SPINNER_BASE_TEMPLATE}from"../base/spinner-base.component";import{TimeSpinnerComponent}from"../timespinner/timespinner.component";var DateTimeSpinnerComponent=function(_super){function DateTimeSpinnerComponent(){var _this=null!==_super&&_super.apply(this,arguments)||this;return _this.selections=[[0,2],[3,5],[6,10],[11,13],[14,16],[17,19]],_this._format="MM/dd/yyyy HH:mm",_this}return __extends(DateTimeSpinnerComponent,_super),DateTimeSpinnerComponent}(TimeSpinnerComponent);export{DateTimeSpinnerComponent};DateTimeSpinnerComponent.decorators=[{type:Component,args:[{selector:"eui-datetimespinner",template:SPINNER_BASE_TEMPLATE,providers:[{provide:NG_VALUE_ACCESSOR,useExisting:forwardRef(function(){return DateTimeSpinnerComponent}),multi:!0}],host:{"[class.f-inline-row]":"true"}}]}],DateTimeSpinnerComponent.ctorParameters=function(){return[]},DateTimeSpinnerComponent.propDecorators={selections:[{type:Input}]};