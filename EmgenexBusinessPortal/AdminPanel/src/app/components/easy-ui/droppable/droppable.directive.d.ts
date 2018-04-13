import { ElementRef, EventEmitter } from '@angular/core';
export declare class DroppableDirective {
    hostRef: ElementRef;
    scope: any;
    disabled: boolean;
    dragEnter: EventEmitter<{}>;
    dragOver: EventEmitter<{}>;
    dragLeave: EventEmitter<{}>;
    drop: EventEmitter<{}>;
    constructor(hostRef: ElementRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
    checkDrop(scope?: any): boolean;
}
