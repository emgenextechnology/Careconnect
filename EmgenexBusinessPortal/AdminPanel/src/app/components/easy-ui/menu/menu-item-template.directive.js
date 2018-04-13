/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var MenuItemTemplateDirective=function(){function MenuItemTemplateDirective(viewContainer){this.viewContainer=viewContainer}return MenuItemTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.value,text:this.text})},MenuItemTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},MenuItemTemplateDirective}();export{MenuItemTemplateDirective};MenuItemTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiMenuItemTemplate]"}]}],MenuItemTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},MenuItemTemplateDirective.propDecorators={value:[{type:Input}],text:[{type:Input}],template:[{type:Input,args:["euiMenuItemTemplate"]}]};