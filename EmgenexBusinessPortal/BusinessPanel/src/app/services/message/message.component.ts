import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';

import { MessageService } from './message.service';
import { MessageState } from './message';

@Component({
    selector: 'angular-message',
    templateUrl: 'message.component.html',
    styleUrls: ['message.component.css']
})
export class MessageComponent implements OnInit {

    showMessage = false;

    private subscription: Subscription;

    constructor(
        private messagerService: MessageService
    ) { }

    ngOnInit() { 
        this.subscription = this.messagerService.messageState
            .subscribe((state: MessageState) => {
                this.showMessage = state.showMessage;
            });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}