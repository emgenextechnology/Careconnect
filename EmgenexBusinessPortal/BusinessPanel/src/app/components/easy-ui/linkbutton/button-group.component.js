/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Component,ContentChildren,forwardRef,Input}from"@angular/core";import{LinkButtonComponent}from"./linkbutton.component";var ButtonGroupComponent=function(){function ButtonGroupComponent(){this.selectionMode="multiple"}return ButtonGroupComponent.prototype.ngAfterContentInit=function(){var _this=this;this.initButtons(),this.buttons.changes.subscribe(function(){return _this.initButtons()})},ButtonGroupComponent.prototype.initButtons=function(){var _this=this;this.buttons.length&&this.buttons.forEach(function(btn){btn.click.subscribe(function(){"single"==_this.selectionMode&&_this.buttons.filter(function(b){return b!=btn}).forEach(function(b){b.selected=!1})})})},ButtonGroupComponent}();export{ButtonGroupComponent};ButtonGroupComponent.decorators=[{type:Component,args:[{selector:"eui-button-group",template:"<ng-content></ng-content>",host:{class:"f-inline-row"}}]}],ButtonGroupComponent.ctorParameters=function(){return[]},ButtonGroupComponent.propDecorators={buttons:[{type:ContentChildren,args:[forwardRef(function(){return LinkButtonComponent})]}],selectionMode:[{type:Input}]};