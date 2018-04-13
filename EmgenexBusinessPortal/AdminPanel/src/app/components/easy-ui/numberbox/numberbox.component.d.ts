import { SpinnerBaseComponent } from '../base/spinner-base.component';
export declare class NumberBoxComponent extends SpinnerBaseComponent<number> {
    min: number;
    max: number;
    increment: number;
    precision: number;
    decimalSeparator: string;
    groupSeparator: string;
    prefix: string;
    suffix: string;
    private _text;
    text: string;
    protected _defaultTextFormatter(value: any): any;
    ngOnInit(): void;
    _onKeypress(event: any): any;
    _onBlur(): void;
    filter(e: any): boolean;
    formatter(value: number): string;
    parser(s: string): number;
    doSpinUp(): void;
    doSpinDown(): void;
}
