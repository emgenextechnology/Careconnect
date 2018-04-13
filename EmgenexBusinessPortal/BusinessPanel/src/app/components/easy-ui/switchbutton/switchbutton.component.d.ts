import { ValueAccessorBase } from '../base/value-accessor-base';
export declare const SWITCHBUTTON_TEMPLATE: string;
export declare class SwitchButtonComponent extends ValueAccessorBase<boolean> {
    onText: string;
    offText: string;
    handleText: string;
    disabled: boolean;
    readonly: boolean;
    inputId: string;
    protected _value: boolean;
    ngOnInit(): void;
    onClick(event: any): void;
}
