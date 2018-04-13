import { Component, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { UserPrivilegesModel, PrivilegesModel } from '../../models/user-privileges-model';
import { MessageService } from '../../services/message/message.service';

@Component({
  selector: 'app-user-privileges',
  templateUrl: './user-privileges.component.html',
  styleUrls: ['./user-privileges.component.css']
})
export class UserPrivilegesComponent implements OnInit {


  isMouseEnterRegistered: boolean = false;
  userId: number;
  NewUserPrivileges = new UserPrivilegesModel();
  NewPrivilegesModel = new PrivilegesModel();

  constructor(
    private httpClient: HttpClient,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.getAllUserPrivileges()
  }

  getAllUserPrivileges() {
    this.route.params.subscribe(params => {
      this.userId = params['id'];
    });
    this.httpClient.get('api/users/' + this.userId + '/privileges').subscribe(data => {
      this.NewUserPrivileges = data['Model']
    })
  }

  update(userPrivilegesModel: UserPrivilegesModel) {
    this.route.params.subscribe(params => {
      this.userId = params['id'];
    });

    this.httpClient.put('api/users/' + this.userId + '/setprivileges', userPrivilegesModel.Modules).subscribe(data => {
      if (data["Status"] == 200) {
        this.getAllUserPrivileges();
      } else {
        this.messageService.openSnackBar(data["Message"], 500);
      }
    });
    return false
  }
}
