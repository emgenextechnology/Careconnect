import { EventEmitter } from '@angular/core';
import { GridBodyComponent } from '../gridbase/grid-body.component';
import { DataGridViewComponent } from './datagrid-view.component';
import { VirtualScrollComponent } from '../base/virtual-scroll.component';
export declare const DATAGRID_BODY_TEMPLATE: string;
export declare class DataGridBodyComponent extends GridBodyComponent {
    view: DataGridViewComponent;
    vscroll: VirtualScrollComponent;
    virtualPageChange: EventEmitter<{}>;
    virtualPageUpdate: EventEmitter<{}>;
    marginTop: number;
    currRows: any[];
    _rows: any[];
    rows: any[];
    readonly isVirtualScroll: boolean;
    scrollTop: number;
    onVirtualScroll(event: any): void;
    onVirtualPageChange(event: any): void;
    onVirtualPageUpdate(event: any): void;
    readonly scrollbarWidth: number;
    constructor(view: DataGridViewComponent);
}
