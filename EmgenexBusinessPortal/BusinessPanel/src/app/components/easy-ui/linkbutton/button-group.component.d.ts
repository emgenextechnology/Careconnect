import { QueryList } from '@angular/core';
import { LinkButtonComponent } from './linkbutton.component';
export declare class ButtonGroupComponent {
    buttons: QueryList<LinkButtonComponent>;
    selectionMode: string;
    ngAfterContentInit(): void;
    initButtons(): void;
}
