import { ElementRef, EventEmitter } from '@angular/core';
export declare const LINKBUTTON_TEMPLATE: string;
export declare class LinkButtonComponent {
    btnRef: ElementRef;
    textRef: ElementRef;
    disabled: boolean;
    toggle: boolean;
    selected: boolean;
    outline: boolean;
    plain: boolean;
    iconCls: string;
    iconAlign: string;
    size: string;
    href: string;
    btnCls: string;
    btnStyle: Object;
    click: EventEmitter<{}>;
    private _text;
    text: string;
    getInnerCls(): string;
    readonly btnLeftCls: string;
    readonly btnIconCls: string;
    ngAfterViewInit(): void;
    onClick(event: any): boolean;
    private _focused;
    readonly focused: boolean;
    focus(): void;
    blur(): void;
}
