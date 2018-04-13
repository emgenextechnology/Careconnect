import { ElementRef } from '@angular/core';
import { AccordionComponent } from './accordion.component';
import { PanelComponent } from '../panel/panel.component';
export declare class AccordionPanelComponent extends PanelComponent {
    accordion: AccordionComponent;
    hostRef: ElementRef;
    title: string;
    collapsed: boolean;
    collapsible: boolean;
    expandIconCls: string;
    collapseIconCls: string;
    selected: boolean;
    private _panelCls;
    panelCls: string;
    private _headerCls;
    headerCls: string;
    private _bodyCls;
    bodyCls: string;
    isFull(): boolean;
    isLast: boolean;
    constructor(accordion: AccordionComponent, hostRef: ElementRef);
    onClick(event: any): void;
    select(): void;
    unselect(): void;
    onClickCollapsibleTool(event: any): void;
}
