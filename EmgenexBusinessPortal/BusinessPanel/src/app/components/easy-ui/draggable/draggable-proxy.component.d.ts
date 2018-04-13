import { ElementRef } from '@angular/core';
export declare class DraggableProxyComponent {
    hostRef: ElementRef;
    proxyRef: ElementRef;
    left: number;
    top: number;
    width: number;
    height: number;
    proxyCls: string;
    proxyStyle: Object;
    reverting: boolean;
    closed: boolean;
    constructor(hostRef: ElementRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
    onTransitionEnd(event: any): void;
}
