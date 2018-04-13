import { Component, AfterViewInit, ViewEncapsulation, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/mergeMap';

import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { Interceptor } from './services/interceptor';
import { BusinessProfileModel } from './models/business-profile-model';
import { MAT_SNACK_BAR_DATA } from '@angular/material';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],

  encapsulation: ViewEncapsulation.None,
  providers: [Interceptor]
})

export class AppComponent implements AfterViewInit {
  CurrentBusinessModel = new BusinessProfileModel();
  hasProfileImage: boolean = true;
  title = "Dashboard";

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title,
    private interceptor: Interceptor,
    private httpClient: HttpClient
  ) { }

  ngOnInit() {
    this.router.events
      .filter(event => event instanceof NavigationEnd)
      .map(() => this.activatedRoute)
      .map(route => {
        while (route.firstChild) route = route.firstChild;
        return route;
      })
      .filter(route => route.outlet === 'primary')
      .mergeMap(route => route.data)
      .subscribe((event) =>
        this.setTitles(event)
      );
    this.httpClient.get('api/BusinessUserProfile').subscribe(data => {
      this.CurrentBusinessModel = data['Model']
    })
  }

  ngAfterViewInit() {
    $(".button-collapse").sideNav({
      closeOnClick: true
    });
    $('.modal').modal();
    $('select').material_select('destroy');
  }

  setTitles(objTitle) {
    this.titleService.setTitle(objTitle['title']);
    this.title = objTitle['shortTitle'] != null ? objTitle['shortTitle'] : objTitle['title'];
  }

  logOff() {
    this.httpClient.post('account/logout', null).subscribe(data => {
      if (data['Status'] === 200)
        window.location.href = this.interceptor.baseUrl;
    });
  }

  redirectToPortal() {
    window.location.href = this.interceptor.baseUrl;
  }
}

@Component({
  template: '<div class="mat-snack-bar-msg-style {{data.css}}">{{ data.message }}</div>'
})

export class MessageServiceComponent {
  constructor( @Inject(MAT_SNACK_BAR_DATA) public data: any) { }
}