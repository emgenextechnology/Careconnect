/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var TreeItemTemplateDirective=function(){function TreeItemTemplateDirective(viewContainer){this.viewContainer=viewContainer}return TreeItemTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.node})},TreeItemTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},TreeItemTemplateDirective}();export{TreeItemTemplateDirective};TreeItemTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiTreeItemTemplate]"}]}],TreeItemTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},TreeItemTemplateDirective.propDecorators={node:[{type:Input}],template:[{type:Input,args:["euiTreeItemTemplate"]}]};