import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
import { PaginationComponent } from './pagination.component';
export declare class PaginationTemplateDirective {
    viewContainer: ViewContainerRef;
    pagination: PaginationComponent;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
