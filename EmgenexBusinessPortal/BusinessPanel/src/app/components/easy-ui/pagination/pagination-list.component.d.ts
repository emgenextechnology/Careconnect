import { ElementRef } from '@angular/core';
import { PaginationComponent } from './pagination.component';
export declare class PaginationListComponent {
    pagination: PaginationComponent;
    listRef: ElementRef;
    constructor(pagination: PaginationComponent);
    ngAfterViewInit(): void;
    onChange(event: any): void;
}
