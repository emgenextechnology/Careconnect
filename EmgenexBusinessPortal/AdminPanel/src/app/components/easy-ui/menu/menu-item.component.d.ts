import { QueryList, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MenuComponent } from './menu.component';
import { SubMenuComponent } from './submenu.component';
import { ItemTemplateDirective } from '../base/template-base';
export declare class MenuItemComponent {
    menu: MenuComponent;
    hostRef: ElementRef;
    cdRef: ChangeDetectorRef;
    itemTemplate: ItemTemplateDirective;
    subMenus: QueryList<SubMenuComponent>;
    value: any;
    text: string;
    iconCls: string;
    disabled: boolean;
    subMenu: SubMenuComponent;
    parentMenu: any;
    isActived: boolean;
    constructor(menu: MenuComponent, hostRef: ElementRef, cdRef: ChangeDetectorRef);
    ngAfterContentInit(): void;
    initMenu(): void;
    onClickItem(event: any): void;
    highlight(): void;
    unhighlight(): void;
}
