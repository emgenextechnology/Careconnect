import { ElementRef, QueryList, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { AddonComponent } from './addon.component';
import { ValueAccessorBase } from './value-accessor-base';
export declare const INPUT_BASE_TEMPLATE: string;
export declare class InputBaseComponent<T> extends ValueAccessorBase<T> {
    cdRef: ChangeDetectorRef;
    addonRef: ElementRef;
    inputRef: ElementRef;
    addons: QueryList<AddonComponent>;
    textFormatter: Function;
    disabled: boolean;
    readonly: boolean;
    editable: boolean;
    iconCls: string;
    iconAlign: string;
    placeholder: string;
    multiline: boolean;
    invalid: boolean;
    tabindex: number;
    cls: string;
    inputCls: string;
    inputStyle: Object;
    inputId: string;
    onFocus: EventEmitter<{}>;
    onBlur: EventEmitter<{}>;
    protected _focused: boolean;
    protected _defaultTextFormatter(value: any): any;
    constructor(cdRef: ChangeDetectorRef);
    ngOnInit(): void;
    readonly focused: boolean;
    focus(): void;
    blur(): void;
    getSelectionStart(): number;
    getSelectionRange(): {
        start: number;
        end: number;
    };
    setSelectionRange(start: number, end: number): void;
}
