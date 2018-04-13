import { EventEmitter } from '@angular/core';
import { InputBaseComponent } from '../base/input-base.component';
import { MenuComponent } from '../menu/menu.component';
export declare const SEARCHBOX_TEMPLATE: string;
export declare class SearchBoxComponent extends InputBaseComponent<string> {
    menu: MenuComponent;
    menuAlign: string;
    category: any;
    buttonAlign: string;
    buttonIconCls: string;
    search: EventEmitter<{}>;
    text: string;
    readonly menuBtnCls: string;
    cls: string;
    menuBtnText: string;
    menuBtnIcon: string;
    ngAfterViewInit(): void;
    onKeyDown(event: any): void;
    doSearch(): void;
    initMenu(): void;
    setCategory(value: any): void;
}
