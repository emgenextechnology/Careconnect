import { ElementRef } from '@angular/core';
import { ValueAccessorBase } from '../base/value-accessor-base';
export declare const SLIDER_TEMPLATE: string;
export declare class SliderComponent extends ValueAccessorBase<any> {
    sliderRef: ElementRef;
    mode: string;
    reversed: boolean;
    showTip: boolean;
    disabled: boolean;
    range: boolean;
    min: number;
    max: number;
    step: number;
    rule: any[];
    readonly value1: any;
    readonly value2: any;
    readonly displayingRule: any[];
    getPosStyle(value: number): {
        left: string;
    } | {
        top: string;
    };
    getRuleValueStyle(index: number): {
        left: string;
    } | {
        top: string;
    };
    onDragHandle(event: any, second?: boolean): void;
    doDown(event: any, sinner: Element): void;
    setPos(pos: number, second?: boolean): number;
    value2pos(value: number): number;
    pos2value(pos: number): number;
}
