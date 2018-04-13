import { ElementRef } from '@angular/core';
import { ListBaseComponent } from '../base/list-base.component';
import { VirtualScrollComponent } from '../base/virtual-scroll.component';
import { ItemTemplateDirective } from '../base/template-base';
export declare const DATALIST_TEMPLATE: string;
export declare class DataListComponent extends ListBaseComponent {
    vscroll: VirtualScrollComponent;
    innerRef: ElementRef;
    itemTemplate: ItemTemplateDirective;
    itemStyle: Object;
    itemCls: string;
    hoverCls: string;
    selectedCls: string;
    getItemClass(row: any): string;
    getRowIndex(index: number): number;
    scrollTop: number;
    navRow(step: number): void;
    highlightFirstRow(): void;
    scrollToSelectedRow(): void;
}
