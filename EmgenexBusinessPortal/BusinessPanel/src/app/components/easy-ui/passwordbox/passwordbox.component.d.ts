import { InputBaseComponent } from '../base/input-base.component';
export declare const PASSWORDBOX_TEMPLATE: string;
export declare class PasswordBoxComponent extends InputBaseComponent<string> {
    passwordChar: string;
    checkInterval: number;
    lastDelay: number;
    showEye: boolean;
    eyeAlign: string;
    revealed: boolean;
    lastTimer: any;
    cursorPos: number;
    inputingText: string;
    private _text;
    text: string;
    readonly eyeCls: string;
    ngAfterViewInit(): void;
    processing(): void;
    convert(value: string, all?: boolean): void;
}
