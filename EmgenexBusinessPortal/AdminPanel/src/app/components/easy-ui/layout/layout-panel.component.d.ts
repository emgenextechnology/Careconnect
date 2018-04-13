import { ElementRef } from '@angular/core';
import { LayoutComponent } from './layout.component';
import { PanelComponent } from '../panel/panel.component';
export declare class LayoutPanelComponent extends PanelComponent {
    layout: LayoutComponent;
    hostRef: ElementRef;
    title: string;
    region: string;
    float: boolean;
    split: boolean;
    animate: boolean;
    collapsible: boolean;
    collapseToShrinkBody: boolean;
    private isExpanding;
    private _collapsed;
    collapsed: boolean;
    readonly hcls: string;
    readonly top: number;
    readonly bottom: number;
    constructor(layout: LayoutComponent, hostRef: ElementRef);
    ngAfterViewInit(): void;
    onDocumentClick(event: any): void;
    onSlideEnd(event: any): void;
    onClickCollapsibleTool1(event: any): void;
    expand(): void;
    collapse(): void;
}
