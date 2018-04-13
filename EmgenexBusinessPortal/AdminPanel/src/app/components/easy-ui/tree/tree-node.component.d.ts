import { TreeComponent } from './tree.component';
export declare class TreeNodeComponent {
    tree: TreeComponent;
    node: any;
    pnode: any;
    depth: number;
    checkState: string;
    loading: boolean;
    readonly indentWidth: number;
    readonly checkboxClass: string;
    constructor(tree: TreeComponent);
    ngOnInit(): void;
    isExpanded(): boolean;
    isCollapsed(): boolean;
    isLeaf(): boolean;
    isSelected(node: any): boolean;
    toggle(event: any): void;
    onClickNode(event: any): void;
    onCheckNode(event: any): void;
    onNodeContextMenu(event: any): void;
}
