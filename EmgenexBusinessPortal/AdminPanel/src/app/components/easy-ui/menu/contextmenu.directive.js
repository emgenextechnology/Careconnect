/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,Input,HostListener}from"@angular/core";var ContextMenuDirective=function(){function ContextMenuDirective(){}return ContextMenuDirective.prototype.onContextMenu=function(event){event.stopPropagation(),event.preventDefault(),this.menu&&this.menu.showContextMenu(event.pageX,event.pageY)},ContextMenuDirective}();export{ContextMenuDirective};ContextMenuDirective.decorators=[{type:Directive,args:[{selector:"[euiContextMenu]"}]}],ContextMenuDirective.ctorParameters=function(){return[]},ContextMenuDirective.propDecorators={menu:[{type:Input,args:["euiContextMenu"]}],onContextMenu:[{type:HostListener,args:["contextmenu",["$event"]]}]};