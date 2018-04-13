/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var TabHeaderTemplateDirective=function(){function TabHeaderTemplateDirective(viewContainer){this.viewContainer=viewContainer}return TabHeaderTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.tab})},TabHeaderTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},TabHeaderTemplateDirective}();export{TabHeaderTemplateDirective};TabHeaderTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiTabHeaderTemplate]"}]}],TabHeaderTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},TabHeaderTemplateDirective.propDecorators={tab:[{type:Input}],template:[{type:Input,args:["euiTabHeaderTemplate"]}]};