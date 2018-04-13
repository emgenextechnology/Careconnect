export declare class TreeHelper {
    cascadeCheck: boolean;
    checkNode(node: any, callback: Function): void;
    uncheckNode(node: any, callback: Function): void;
    uncheckAllNodes(nodes: any[], callback: Function): void;
    setParentCheckbox(node: any): void;
    setChildCheckbox(node: any, checkState: string): void;
    adjustCheck(node: any): void;
    calcNodeState(node: any): "unchecked" | "checked" | "indeterminate";
    forNodes(fromNodes: any[], callback: Function): void;
    findNode(nodes: any[], field: string, value: any): any;
}
export declare const treeHelper: TreeHelper;
