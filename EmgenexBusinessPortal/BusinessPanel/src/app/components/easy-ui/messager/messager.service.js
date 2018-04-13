/**
 * EasyUI for Angular 0.7
 * 
 * Copyright (c) 2009-2017 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: https://www.jeasyui.com/license_freeware2.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
import{Injectable}from"@angular/core";import{Subject}from"rxjs/Subject";var MessagerService=function(){function MessagerService(){this.alertSubject=new Subject,this.confirmSubject=new Subject,this.promptSubject=new Subject}return MessagerService.prototype.alert=function(options){this.alertSubject.next(options)},MessagerService.prototype.confirm=function(options){this.confirmSubject.next(options)},MessagerService.prototype.prompt=function(options){this.promptSubject.next(options)},MessagerService}();export{MessagerService};MessagerService.decorators=[{type:Injectable}],MessagerService.ctorParameters=function(){return[]};