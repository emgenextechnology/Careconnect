import { QueryList, ElementRef } from '@angular/core';
import { MenuComponent } from './menu.component';
import { MenuItemComponent } from './menu-item.component';
export declare class SubMenuComponent {
    menu: MenuComponent;
    hostRef: ElementRef;
    subItems: QueryList<MenuItemComponent>;
    menuCls: string;
    menuStyle: any;
    menuWidth: any;
    left: number;
    top: number;
    zIndex: number;
    parentItem: MenuItemComponent;
    constructor(menu: MenuComponent, hostRef: ElementRef);
    ngAfterContentInit(): void;
    initItems(): void;
    unhighlight(): void;
    alignMenu(): void;
}
