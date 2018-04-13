import { EventEmitter } from '@angular/core';
import { ItemTemplateDirective } from '../base/template-base';
export declare class TreeComponent {
    itemTemplate: ItemTemplateDirective;
    data: any[];
    selection: any;
    checkbox: boolean;
    cascadeCheck: boolean;
    filterIncludingChild: boolean;
    filter: Function;
    selectionChange: EventEmitter<{}>;
    nodeClick: EventEmitter<{}>;
    nodeExpand: EventEmitter<{}>;
    nodeCollapse: EventEmitter<{}>;
    nodeCheck: EventEmitter<{}>;
    nodeUncheck: EventEmitter<{}>;
    checkChange: EventEmitter<{}>;
    nodeContextMenu: EventEmitter<{}>;
    highlightNode: any;
    readonly checkedNodes: any[];
    getCheckedNodes(state?: string): any[];
    selectNode(node: any): void;
    checkNode(node: any): void;
    uncheckNode(node: any): void;
    uncheckAllNodes(): void;
    adjustCheck(node: any): void;
    doFilter(q: any): void;
}
