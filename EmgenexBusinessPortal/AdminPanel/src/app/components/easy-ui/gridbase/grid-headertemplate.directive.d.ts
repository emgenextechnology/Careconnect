import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class GridHeaderTemplateDirective {
    viewContainer: ViewContainerRef;
    column: any;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
