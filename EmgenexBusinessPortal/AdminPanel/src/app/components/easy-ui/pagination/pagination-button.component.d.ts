import { PaginationComponent } from './pagination.component';
import { LinkButtonComponent } from '../linkbutton/linkbutton.component';
export declare class PaginationButtonComponent extends LinkButtonComponent {
    pagination: PaginationComponent;
    plain: boolean;
    private _name;
    name: string;
    private _iconCls;
    iconCls: string;
    private _disabled;
    disabled: boolean;
    constructor(pagination: PaginationComponent);
}
