import { TemplateRef, ViewContainerRef, EmbeddedViewRef } from '@angular/core';
export declare class DateBoxCellTemplateDirective {
    viewContainer: ViewContainerRef;
    date: Date;
    template: TemplateRef<any>;
    view: EmbeddedViewRef<any>;
    constructor(viewContainer: ViewContainerRef);
    ngOnInit(): void;
    ngOnDestroy(): void;
}
