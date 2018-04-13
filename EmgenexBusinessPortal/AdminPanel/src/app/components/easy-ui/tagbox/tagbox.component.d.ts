import { ComboBoxComponent } from '../combobox/combobox.component';
export declare const TAGBOX_TEMPLATE: string;
export declare class TagBoxComponent extends ComboBoxComponent {
    hasDownArrow: boolean;
    multiple: boolean;
    limitToList: boolean;
    tagCss: any;
    cls: string;
    text: string;
    inputWidth: number;
    ngAfterContentInit(): void;
    ngAfterViewInit(): void;
    onKeyDown(event: any): void;
    onClick(event: any): void;
    doEnter(): void;
    fixValue(): void;
    autoSizeInput(): void;
    removeTag(index: number): void;
    getCss(css: any, row: any, type: string): any;
    getTagClass(row: any): any;
    getTagStyle(row: any): any;
}
