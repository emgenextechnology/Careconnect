import { InputBaseComponent } from '../base/input-base.component';
export declare const SPINNER_BASE_TEMPLATE: string;
export declare class SpinnerBaseComponent<T> extends InputBaseComponent<T> {
    reversed: boolean;
    spinners: boolean;
    spinAlign: string;
    private _cls;
    cls: string;
    onClickUp(): void;
    onClickDown(): void;
    doSpinUp(): void;
    doSpinDown(): void;
}
