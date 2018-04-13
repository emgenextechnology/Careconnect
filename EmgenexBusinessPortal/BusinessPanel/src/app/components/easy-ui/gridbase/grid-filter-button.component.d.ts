import { ComboBoxComponent } from '../combobox/combobox.component';
import { GridColumnComponent } from './grid-column.component';
export declare class GridFilterButtonComponent extends ComboBoxComponent {
    arrowIconCls: string;
    panelStyle: Object;
    inputStyle: Object;
    editable: boolean;
    column: GridColumnComponent;
    ngAfterViewInit(): void;
}
