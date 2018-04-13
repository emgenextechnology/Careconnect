/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Component,ContentChildren,Input}from"@angular/core";import{GridHeaderRowComponent}from"./grid-header-row.component";import{domHelper}from"../base/domhelper";var GridColumnGroupComponent=function(){function GridColumnGroupComponent(){this.frozen=!1,this.align="left",this.width=null}return GridColumnGroupComponent.prototype.ngOnInit=function(){this.width=domHelper.toStyleValue(this.width)},GridColumnGroupComponent}();export{GridColumnGroupComponent};GridColumnGroupComponent.decorators=[{type:Component,args:[{selector:"eui-grid-column-group",template:""}]}],GridColumnGroupComponent.ctorParameters=function(){return[]},GridColumnGroupComponent.propDecorators={rows:[{type:ContentChildren,args:[GridHeaderRowComponent]}],frozen:[{type:Input}],align:[{type:Input}],width:[{type:Input}]};