import { LinkButtonComponent } from '../linkbutton/linkbutton.component';
import { MenuComponent } from '../menu/menu.component';
export declare const MENUBUTTON_TEMPLATE: string;
export declare class MenuButtonComponent extends LinkButtonComponent {
    menu: MenuComponent;
    menuAlign: string;
    duration: number;
    timer: any;
    getInnerCls(): string;
    onMouseEnter(event: any): void;
    onMouseLeave(event: any): void;
    showMenu(): void;
}
