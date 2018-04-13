import { QueryList, ElementRef } from '@angular/core';
import { LayoutPanelComponent } from './layout-panel.component';
export declare const LAYOUT_TEMPLATE: string;
export declare class LayoutComponent {
    layoutRef: ElementRef;
    panels: QueryList<LayoutPanelComponent>;
    layoutCls: string;
    layoutStyle: Object;
    paddings: any;
    paddingLeft: number;
    paddingRight: number;
    paddingTop: number;
    paddingBottom: number;
    ngAfterViewInit(): void;
    getPanel(region: string): LayoutPanelComponent;
    getPaddingValue(region: any): number;
    updatePaddings(): void;
}
