/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var DataGridGroupTemplateDirective=function(){function DataGridGroupTemplateDirective(viewContainer){this.viewContainer=viewContainer}return DataGridGroupTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.value,rows:this.rows})},DataGridGroupTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},DataGridGroupTemplateDirective}();export{DataGridGroupTemplateDirective};DataGridGroupTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiDataGridGroupTemplate]"}]}],DataGridGroupTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},DataGridGroupTemplateDirective.propDecorators={value:[{type:Input}],rows:[{type:Input}],template:[{type:Input,args:["euiDataGridGroupTemplate"]}]};