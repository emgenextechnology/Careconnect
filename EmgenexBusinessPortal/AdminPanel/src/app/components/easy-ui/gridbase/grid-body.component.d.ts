import { ElementRef, EventEmitter } from '@angular/core';
import { GridColumnComponent } from './grid-column.component';
export declare const GRIDBASE_BODY_TEMPLATE: string;
export declare class GridBodyComponent {
    bodyRef: ElementRef;
    innerRef: ElementRef;
    columns: GridColumnComponent[];
    rows: any[];
    bodyScroll: EventEmitter<{}>;
    private _scrollTop;
    scrollTop: number;
    onScroll(event: any): void;
    readonly scrollbarWidth: number;
}
