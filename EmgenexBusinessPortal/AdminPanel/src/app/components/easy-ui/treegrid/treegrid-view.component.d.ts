import { GridViewComponent } from '../gridbase/grid-view.component';
import { TreeGridComponent } from './treegrid.component';
export declare class TreeGridViewComponent extends GridViewComponent {
    grid: TreeGridComponent;
    constructor(grid: TreeGridComponent);
    onHeaderCellClick(event: any): void;
}
