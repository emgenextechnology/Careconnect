import { GridBodyComponent } from '../gridbase/grid-body.component';
import { TreeGridViewComponent } from './treegrid-view.component';
export declare const TREEGRID_BODY_TEMPLATE: string;
export declare class TreeGridBodyComponent extends GridBodyComponent {
    view: TreeGridViewComponent;
    constructor(view: TreeGridViewComponent);
}
