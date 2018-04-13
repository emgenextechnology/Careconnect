import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class DataGridGroupTemplateDirective {
    viewContainer: ViewContainerRef;
    value: any;
    rows: any[];
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
