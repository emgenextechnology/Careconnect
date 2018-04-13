/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var GridHeaderTemplateDirective=function(){function GridHeaderTemplateDirective(viewContainer){this.viewContainer=viewContainer}return GridHeaderTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.column})},GridHeaderTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},GridHeaderTemplateDirective}();export{GridHeaderTemplateDirective};GridHeaderTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiGridHeaderTemplate]"}]}],GridHeaderTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},GridHeaderTemplateDirective.propDecorators={column:[{type:Input}],template:[{type:Input,args:["euiGridHeaderTemplate"]}]};