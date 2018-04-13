import { EventEmitter } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';
export declare class ValueAccessorBase<T> implements ControlValueAccessor {
    protected _value: T;
    protected _initialized: boolean;
    protected _changed: ((v: T) => void)[];
    protected _touched: (() => void)[];
    protected _transform(value: any): T;
    valueChange: EventEmitter<{}>;
    value: T;
    touched(): void;
    writeValue(value: T): void;
    registerOnChange(fn: (value: T) => void): void;
    registerOnTouched(fn: () => void): void;
}
