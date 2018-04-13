import { EventEmitter } from '@angular/core';
import { CalendarComponent } from '../calendar/calendar.component';
import { ComboBaseComponent } from '../base/combo-base.component';
import { CellTemplateDirective } from '../base/template-base';
export declare const DATEBOX_TEMPLATE: string;
export declare class DateBoxComponent extends ComboBaseComponent<Date> {
    calendar: CalendarComponent;
    cellTemplate: CellTemplateDirective;
    selection: Date;
    format: string;
    formatter: Function;
    parser: Function;
    selectionChange: EventEmitter<{}>;
    cls: string;
    timer: any;
    inputingText: string;
    _text: string;
    text: string;
    ngAfterContentInit(): void;
    onKeyDown(event: any): void;
    onDateBoxBlur(): void;
    onSelectionChange(event: any): void;
    doFilter(value: string): void;
    defaultFormatter(date: Date): string;
    defaultParser(value: string): Date;
    openPanel(): void;
    selectToday(): void;
}
