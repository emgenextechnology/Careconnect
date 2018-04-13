import { Component, OnInit
  , AfterViewInit 
} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BusinessProfileModel } from '../../models/business-profile-model';
import { MessageService } from '../../services/message/message.service';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-business-profile',
  templateUrl: './business-profile.component.html',
  styleUrls: ['./business-profile.component.css']
})
export class BusinessProfileComponent implements OnInit
, AfterViewInit 
{

  NewBusinessProfileModel = new BusinessProfileModel();
  DateRange
  SalesGroupBy

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllUserPrivileges()
    this.httpClient.get('lookup/getallDashboardperiods').subscribe(data => {
      this.DateRange = data['Model'].List
    })
    this.httpClient.get('lookup/getallSalesGroupbyValues').subscribe(data => {
      this.SalesGroupBy = data['Model'].List
    })
  }

  getAllUserPrivileges() {
    this.httpClient.get('api/businessprofile').subscribe(data => {
      this.NewBusinessProfileModel = data['Model']
    })
  }
  
  update(businessProfileModel: BusinessProfileModel) {
    this.httpClient.post('api/businessprofile/save', businessProfileModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllUserPrivileges();
      }
    });
    return false
  }

  ngAfterViewInit() {
    // $('#OtherEmails').multiple_emails();
  }
}
