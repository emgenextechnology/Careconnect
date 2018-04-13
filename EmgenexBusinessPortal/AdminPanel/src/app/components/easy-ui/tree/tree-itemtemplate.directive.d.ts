import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class TreeItemTemplateDirective {
    viewContainer: ViewContainerRef;
    node: any;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
