/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Directive,ElementRef,Input,Output,EventEmitter}from"@angular/core";import{DraggableDirective}from"../draggable/draggable.directive";var DroppableDirective=function(){function DroppableDirective(hostRef){this.hostRef=hostRef,this.scope=null,this.disabled=!1,this.dragEnter=new EventEmitter,this.dragOver=new EventEmitter,this.dragLeave=new EventEmitter,this.drop=new EventEmitter}return DroppableDirective.prototype.ngOnInit=function(){DraggableDirective.droppables.push(this)},DroppableDirective.prototype.ngOnDestroy=function(){var index=DraggableDirective.droppables.indexOf(this);index>=0&&DraggableDirective.droppables.splice(index,1)},DroppableDirective.prototype.checkDrop=function(scope){if(void 0===scope&&(scope=null),!scope||!this.scope)return!0;if("string"==typeof this.scope&&this.scope==scope)return!0;if(this.scope instanceof Array)for(var i=0;i<this.scope.length;i++)if(this.scope[i]==scope)return!0;return!1},DroppableDirective}();export{DroppableDirective};DroppableDirective.decorators=[{type:Directive,args:[{selector:"[euiDroppable]"}]}],DroppableDirective.ctorParameters=function(){return[{type:ElementRef}]},DroppableDirective.propDecorators={scope:[{type:Input,args:["euiDroppable"]}],disabled:[{type:Input}],dragEnter:[{type:Output}],dragOver:[{type:Output}],dragLeave:[{type:Output}],drop:[{type:Output}]};