/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ViewContainerRef,Input}from"@angular/core";var MessagerBodyTemplateDirective=function(){function MessagerBodyTemplateDirective(viewContainer){this.viewContainer=viewContainer}return MessagerBodyTemplateDirective.prototype.ngOnInit=function(){this.view=this.viewContainer.createEmbeddedView(this.template,{$implicit:this.messager})},MessagerBodyTemplateDirective.prototype.ngOnDestroy=function(){this.view.destroy()},MessagerBodyTemplateDirective}();export{MessagerBodyTemplateDirective};MessagerBodyTemplateDirective.decorators=[{type:Directive,args:[{selector:"[euiMessagerBodyTemplate]"}]}],MessagerBodyTemplateDirective.ctorParameters=function(){return[{type:ViewContainerRef}]},MessagerBodyTemplateDirective.propDecorators={messager:[{type:Input}],template:[{type:Input,args:["euiMessagerBodyTemplate"]}]};