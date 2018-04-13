import { EventEmitter } from '@angular/core';
import { GridBaseComponent } from '../gridbase/grid-base.component';
export declare const TREEGRID_TEMPLATE: string;
export declare class TreeGridComponent extends GridBaseComponent {
    idField: string;
    treeField: string;
    selectionMode: string;
    checkbox: boolean;
    cascadeCheck: boolean;
    rowClick: EventEmitter<{}>;
    rowExpand: EventEmitter<{}>;
    rowCollapse: EventEmitter<{}>;
    rowCheck: EventEmitter<{}>;
    rowUncheck: EventEmitter<{}>;
    readonly checkedRows: any[];
    getCheckedRows(state?: string): any[];
    selectRow(row: any): void;
    checkRow(row: any): void;
    uncheckRow(row: any): void;
    adjustCheck(row: any): void;
    sortData(): void;
}
