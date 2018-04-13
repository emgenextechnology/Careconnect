import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class TabHeaderTemplateDirective {
    viewContainer: ViewContainerRef;
    tab: any;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
