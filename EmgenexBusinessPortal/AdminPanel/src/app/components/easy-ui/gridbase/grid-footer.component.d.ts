import { ElementRef } from '@angular/core';
import { GridColumnComponent } from './grid-column.component';
export declare const GRID_FOOTER_TEMPLATE: string;
export declare class GridFooterComponent {
    footerRef: ElementRef;
    columns: GridColumnComponent[];
    rows: any[];
    paddingWidth: number;
    private _scrollLeft;
    scrollLeft: number;
}
