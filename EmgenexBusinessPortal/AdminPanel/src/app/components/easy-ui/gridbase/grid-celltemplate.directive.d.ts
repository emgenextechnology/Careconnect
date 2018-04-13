import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class GridCellTemplateDirective {
    viewContainer: ViewContainerRef;
    row: any;
    rowIndex: number;
    column: any;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
