import { GridViewComponent } from '../gridbase/grid-view.component';
import { DataGridBodyComponent } from './datagrid-body.component';
import { DataGridComponent } from './datagrid.component';
export declare class DataGridViewComponent extends GridViewComponent {
    grid: DataGridComponent;
    body: DataGridBodyComponent;
    constructor(grid: DataGridComponent);
    onHeaderCellClick(event: any): void;
}
