import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class MenuItemTemplateDirective {
    viewContainer: ViewContainerRef;
    value: any;
    text: string;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
