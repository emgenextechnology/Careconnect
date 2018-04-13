import { ComboBaseComponent } from '../base/combo-base.component';
import { DataGridComponent } from '../datagrid/datagrid.component';
export declare const COMBOGRID_TEMPLATE: string;
export declare class ComboGridComponent extends ComboBaseComponent<any> {
    datagrid: DataGridComponent;
    valueField: string;
    textField: string;
    editable: boolean;
    timer: any;
    mappingTexts: {};
    displayingText: string;
    inputingText: string;
    datagridScrollTop: number;
    private _data;
    private _text;
    data: any[];
    text: string;
    private _multiple;
    multiple: boolean;
    ngAfterContentInit(): void;
    onComboGridBlur(): void;
    doFilter(value: string): void;
    openPanel(): void;
    closePanel(): void;
    updateText(): void;
    findRow(value: any): any;
}
