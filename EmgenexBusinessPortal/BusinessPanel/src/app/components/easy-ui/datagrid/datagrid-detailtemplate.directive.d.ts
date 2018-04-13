import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class DataGridDetailTemplateDirective {
    viewContainer: ViewContainerRef;
    row: any;
    rowIndex: number;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
