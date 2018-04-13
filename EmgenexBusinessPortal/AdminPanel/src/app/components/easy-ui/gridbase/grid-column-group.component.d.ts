import { QueryList } from '@angular/core';
import { GridHeaderRowComponent } from './grid-header-row.component';
export declare class GridColumnGroupComponent {
    rows: QueryList<GridHeaderRowComponent>;
    frozen: boolean;
    align: string;
    width: any;
    ngOnInit(): void;
}
