import { QueryList, EventEmitter } from '@angular/core';
import { AccordionPanelComponent } from './accordion-panel.component';
export declare class AccordionComponent {
    panels: QueryList<AccordionPanelComponent>;
    border: boolean;
    multiple: boolean;
    panelSelect: EventEmitter<{}>;
    panelUnselect: EventEmitter<{}>;
    private _selectedIndex;
    selectedIndex: any;
    ngAfterViewInit(): void;
    initPanels(): void;
    initSelectedPanel(): void;
    select(index: number): void;
    unselect(index: number): void;
    getPanel(index: number): AccordionPanelComponent;
    getPanels(indexs: number[]): AccordionPanelComponent[];
    getPanelIndex(panel: AccordionPanelComponent): number;
    getSelectedIndex(): number;
    getSelectedPanel(): AccordionPanelComponent;
    getSelectedPanels(): AccordionPanelComponent[];
}
