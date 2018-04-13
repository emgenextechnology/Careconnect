import { ElementRef } from '@angular/core';
export declare class LabelDirective {
    hostRef: ElementRef;
    target: any;
    align: string;
    constructor(hostRef: ElementRef);
    ngAfterViewInit(): void;
    initAttributes(): string;
    static index: number;
    static nextId(): string;
}
