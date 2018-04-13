import { QueryList, EventEmitter } from '@angular/core';
import { PaginationComponent } from '../pagination/pagination.component';
import { PageTemplateDirective } from '../base/template-base';
export declare class ListBaseComponent {
    pageTemplate: PageTemplateDirective;
    pageRefs: QueryList<PaginationComponent>;
    border: boolean;
    loading: boolean;
    loadMsg: string;
    pagination: boolean;
    pagePosition: string;
    pageOptions: any;
    lazy: boolean;
    virtualScroll: boolean;
    rowHeight: number;
    pageNumber: number;
    pageSize: number;
    total: number;
    idField: string;
    selectionMode: string;
    filterable: boolean;
    filterRules: any[];
    filterDelay: number;
    filterMatchingType: string;
    filterPosition: string;
    filterBtnPosition: string;
    filterChange: EventEmitter<{}>;
    selectionChange: EventEmitter<{}>;
    pageChange: EventEmitter<{}>;
    rowSelect: EventEmitter<{}>;
    rowUnselect: EventEmitter<{}>;
    rowClick: EventEmitter<{}>;
    cellSelect: EventEmitter<{}>;
    cellUnselect: EventEmitter<{}>;
    cellClick: EventEmitter<{}>;
    _initialized: boolean;
    pageState: any;
    highlightRow: any;
    highlightCell: any;
    selectedRows: any[];
    selectedCells: any[];
    rows: any[];
    _data: any[];
    _filteredData: any[];
    _filterOperators: any;
    selection: any;
    data: any[];
    filterOperators: any;
    ngOnInit(): void;
    ngAfterContentInit(): void;
    ngAfterViewInit(): void;
    setData(value: any[]): void;
    onPageChange(event: any): void;
    onVirtualPageChange(event: any): void;
    onRowClick(row: any, event: any): void;
    onCellClick(row: any, column: any, event: any): void;
    sortData(): void;
    filterData(data: any[]): any[];
    doFilter(rule?: any): void;
    doEnter(): void;
    getSelectedIndex(row: any): number;
    getSelectedCellIndex(row: any, column: any): number;
    isCellSelectionMode(): boolean;
    isHighlighted(row: any, column?: any): boolean;
    isSelected(row: any, column?: any): boolean;
    selectRow(row: any): void;
    unselectRow(row: any): void;
    selectCell(row: any, column: any): void;
    unselectCell(row: any, column: any): void;
    clearSelections(): void;
    navRow(step: number): void;
    readonly defaultOperators: {
        nofilter: {
            text: string;
            isMatch: () => boolean;
        };
        contains: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        equal: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        notequal: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        beginwith: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        endwith: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        less: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        lessorequal: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        greater: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
        greaterorequal: {
            text: string;
            isMatch: (source: any, value: any) => boolean;
        };
    };
    getFilterRuleIndex(field: string): number;
    getFilterRule(field: string): any;
    addFilterRule(rule: any): void;
    removeFilterRule(field: string): void;
}
