import { HttpInterceptor, HttpRequest, HttpHandler, HttpSentEvent, HttpHeaderResponse, HttpProgressEvent, HttpResponse, HttpUserEvent } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import 'rxjs/add/operator/do';
import { HttpEvent, HttpErrorResponse } from "@angular/common/http";
import { LoaderService } from './loader/loader.service';
// import { MessageService } from './message/message.service';

@Injectable()
export class Interceptor implements HttpInterceptor {
    public baseUrl = "https://crm.careconnectsystems.com/";
    //   public baseUrl = "http://stagingcrm.careconnectsystems.com/";
    //public baseUrl = "http://emgen2016.com/";

    constructor(
        private loaderService: LoaderService,
        // private messageService: MessageService
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpSentEvent | HttpHeaderResponse | HttpProgressEvent | HttpResponse<any> | HttpUserEvent<any>> {

        //#region BearerAuth
        // const idToken = localStorage.getItem("authorizationData");
        // if (idToken) {
        //     const cloned = req.clone({
        //         headers: req.headers.set("Authorization",
        //             "Bearer " + idToken)
        //     });
        //     return next.handle(cloned);
        // } else {
        //     return next.handle(req);
        // }
        //#endregion

        const cloned = req.clone({
            url: this.baseUrl + req.url
        });

        return next.handle(cloned).do((event: HttpEvent<any>) => {
            // console.log(event);
            // this.showMessage();

            this.showLoader();
            if (event instanceof HttpResponse) {
                this.hideLoader();
            }
        }, (err: any) => {
            if (err instanceof HttpErrorResponse) {
                if (err.status === 500) {
                    window.location.href = this.baseUrl;
                }
            }
        });
    }

    private showLoader(): void {
        this.loaderService.show();
    }

    private hideLoader(): void {
        this.loaderService.hide();
    }

    // private showMessage(): void {
    //     this.messageService.show();
    // }

    // private hideMessage(): void {
    //     this.messageService.hide();
    // }
}