import { PaginationComponent } from './pagination.component';
export declare class PaginationLinkComponent {
    pagination: PaginationComponent;
    constructor(pagination: PaginationComponent);
    readonly pages: any[];
    onClick(page: any): void;
}
