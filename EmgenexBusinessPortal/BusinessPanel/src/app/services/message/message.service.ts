import { Injectable, Inject } from '@angular/core';
import { Subject } from 'rxjs/Subject';

import { MessageState } from './message';
import { Component } from '@angular/core/src/metadata/directives';
import { MatSnackBar, MAT_SNACK_BAR_DATA } from '@angular/material';
import { MessageServiceComponent } from '../../app.component';

@Injectable()

export class MessageService {

    private messageSubject = new Subject<MessageState>();

    messageState = this.messageSubject.asObservable();

    constructor(
        public snackBar: MatSnackBar
    ) { }

    show() {
        this.messageSubject.next(<MessageState>{ showMessage: true });
    }

    hide() {
        this.messageSubject.next(<MessageState>{ showMessage: false });
    }

    openSnackBar(message: string, status: number) {
        this.snackBar.openFromComponent(MessageServiceComponent, {
            duration: 3000,
            data: {
                message: message,
                css: status == 200 ? 'mat-snack-bar-msg-success' : 'mat-snack-bar-msg-error'
            }
        });
    }
}
