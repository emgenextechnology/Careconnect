import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class MessagerBodyTemplateDirective {
    viewContainer: ViewContainerRef;
    messager: any;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
