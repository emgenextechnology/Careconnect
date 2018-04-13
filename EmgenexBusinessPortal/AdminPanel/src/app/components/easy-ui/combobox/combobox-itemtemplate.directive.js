/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var ComboBoxItemTemplateDirective=function(){function ComboBoxItemTemplateDirective(viewContainer){this.viewContainer=viewContainer}return ComboBoxItemTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.row,rowIndex:this.rowIndex})},ComboBoxItemTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},ComboBoxItemTemplateDirective}();export{ComboBoxItemTemplateDirective};ComboBoxItemTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiComboBoxItemTemplate]"}]}],ComboBoxItemTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},ComboBoxItemTemplateDirective.propDecorators={row:[{type:Input}],rowIndex:[{type:Input}],template:[{type:Input,args:["euiComboBoxItemTemplate"]}]};