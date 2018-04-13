import { Subject } from 'rxjs/Subject';
export declare class MessagerService {
    alertSubject: Subject<{}>;
    confirmSubject: Subject<{}>;
    promptSubject: Subject<{}>;
    alert(options: any): void;
    confirm(options: any): void;
    prompt(options: any): void;
}
