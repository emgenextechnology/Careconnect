/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Input,Output,EventEmitter}from"@angular/core";var ValueAccessorBase=function(){function ValueAccessorBase(){this._value=null,this._initialized=!1,this._changed=new Array,this._touched=new Array,this.valueChange=new EventEmitter}return ValueAccessorBase.prototype._transform=function(value){return value},Object.defineProperty(ValueAccessorBase.prototype,"value",{get:function(){return this._value},set:function(value){var previousValue=this._value;this._value=this._transform.call(this,value);var currentValue=this._value;this._initialized&&currentValue!==previousValue&&(this._changed.forEach(function(f){return f(currentValue)}),this.valueChange.emit({currentValue:currentValue,previousValue:previousValue})),this._initialized||(this._initialized=!0)},enumerable:!0,configurable:!0}),ValueAccessorBase.prototype.touched=function(){this._touched.forEach(function(f){return f()})},ValueAccessorBase.prototype.writeValue=function(value){this._initialized?this.value=value:null!=value&&(this._initialized=!0,this._value=this._transform.call(this,value))},ValueAccessorBase.prototype.registerOnChange=function(fn){this._changed.push(fn)},ValueAccessorBase.prototype.registerOnTouched=function(fn){this._touched.push(fn)},ValueAccessorBase}();export{ValueAccessorBase};ValueAccessorBase.propDecorators={valueChange:[{type:Output}],value:[{type:Input}]};