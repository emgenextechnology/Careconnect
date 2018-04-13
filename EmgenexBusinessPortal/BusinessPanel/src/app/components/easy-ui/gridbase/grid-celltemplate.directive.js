/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var GridCellTemplateDirective=function(){function GridCellTemplateDirective(viewContainer){this.viewContainer=viewContainer}return GridCellTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.row,rowIndex:this.rowIndex,column:this.column})},GridCellTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},GridCellTemplateDirective}();export{GridCellTemplateDirective};GridCellTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiGridCellTemplate]"}]}],GridCellTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},GridCellTemplateDirective.propDecorators={row:[{type:Input}],rowIndex:[{type:Input}],column:[{type:Input}],template:[{type:Input,args:["euiGridCellTemplate"]}]};