import { SpinnerBaseComponent } from '../base/spinner-base.component';
export declare class TimeSpinnerComponent extends SpinnerBaseComponent<string> {
    min: string;
    max: string;
    increment: number;
    highlight: number;
    formatter: Function;
    parser: Function;
    selections: number[][];
    highlighting: boolean;
    inputingText: string;
    _text: string;
    _format: string;
    text: string;
    format: string;
    protected _defaultTextFormatter(value: any): any;
    protected _transform(value: string): string;
    ngOnInit(): void;
    onClickMe(event: any): void;
    onPressMe(event: any): void;
    onTimeSpinnerBlur(): void;
    doSpin(down: boolean): void;
    doSpinUp(): void;
    doSpinDown(): void;
    highlightRange(index: number): void;
    parseD(value: string): Date;
    defaultFormatter(date: Date): string;
    defaultParser(value: string): Date;
}
