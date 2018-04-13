/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var CalendarCellTemplateDirective=function(){function CalendarCellTemplateDirective(viewContainer){this.viewContainer=viewContainer}return CalendarCellTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.date})},CalendarCellTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},CalendarCellTemplateDirective}();export{CalendarCellTemplateDirective};CalendarCellTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiCalendarCellTemplate]"}]}],CalendarCellTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},CalendarCellTemplateDirective.propDecorators={date:[{type:Input}],template:[{type:Input,args:["euiCalendarCellTemplate"]}]};