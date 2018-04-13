import { EventEmitter } from '@angular/core';
import { GridColumnComponent } from './grid-column.component';
import { GridColumnGroupComponent } from './grid-column-group.component';
import { GridHeaderComponent } from './grid-header.component';
import { GridFooterComponent } from './grid-footer.component';
import { GridBodyComponent } from './grid-body.component';
export declare class GridViewComponent {
    header: GridHeaderComponent;
    footer: GridFooterComponent;
    body: GridBodyComponent;
    columns: GridColumnComponent[];
    columnGroup: GridColumnGroupComponent;
    viewIndex: number;
    rows: any[];
    footerRows: any[];
    filterable: boolean;
    bodyScroll: EventEmitter<{}>;
    readonly viewCls: string;
    scrollTop: number;
    headerHeight: number;
    readonly headerPaddingWidth: number;
    onBodyScroll(event: any): void;
}
