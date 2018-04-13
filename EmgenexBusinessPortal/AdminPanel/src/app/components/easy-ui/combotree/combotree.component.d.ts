import { ComboBaseComponent } from '../base/combo-base.component';
import { TreeComponent } from '../tree/tree.component';
export declare const COMBOTREE_TEMPLATE: string;
export declare class ComboTreeComponent extends ComboBaseComponent<any> {
    tree: TreeComponent;
    valueField: string;
    textField: string;
    editable: boolean;
    timer: any;
    mappingTexts: {};
    displayingText: string;
    inputingText: string;
    private updatingText;
    private _data;
    private _text;
    data: any[];
    text: string;
    private _multiple;
    multiple: boolean;
    ngAfterContentInit(): void;
    onComboTreeBlur(): void;
    doFilter(value: string): void;
    openPanel(): void;
    updateText(): void;
}
