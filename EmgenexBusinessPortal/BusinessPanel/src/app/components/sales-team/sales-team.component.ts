import { Component, OnInit, Input, Inject, ViewChild } from '@angular/core';
import { SalesTeamModel, SalesTeamFilter } from '../../models/sales-team-model';
import { HttpClient } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { PageEvent, MatPaginator, Sort } from '@angular/material';
import { MessageService } from '../../services/message/message.service';

declare var $: any

@Component({
  selector: 'app-sales-team',
  templateUrl: './sales-team.component.html',
  styleUrls: ['./sales-team.component.css']
})

export class SalesTeamComponent implements OnInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @Input() SalesTeam;
  SalesDirectors;
  Managers;
  searchFilter = new SalesTeamFilter();
  newSalesTeamModel = new SalesTeamModel();
  isMouseEnterRegistered: boolean = false;
  private salesTeamId: number = 0;
  managers = new FormControl();
  salesDirectors = new FormControl();
  pageSize: number;
  pageEvent: PageEvent;

  // animal: string;
  // name: string;

  constructor(
    private httpClient: HttpClient,
    private messageService: MessageService
    // public dialog: MatDialog
  ) { }

  // openDialog(): void {
  //   let dialogRef = this.dialog.open(DialogOverviewExampleDialog, {
  //     width: '250px',
  //     data: { name: this.name, animal: this.animal }
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     console.log('The dialog was closed');
  //     this.animal = result;
  //   });
  // }

  ngOnInit() {
    this.getAllSalesTeam(this.searchFilter);
  }

  getAllSalesTeam(searchFilter: SalesTeamFilter, resetPager: boolean = true) {
    if (this.pageEvent) {
      searchFilter.pageSize = this.pageEvent.pageSize;
      searchFilter.currentPage = this.pageEvent.pageIndex + 1;
    }

    if (resetPager === true) {
      searchFilter.currentPage = 1;
      if (this.paginator)
        this.paginator.pageIndex = 0;
    }
    
    this.httpClient.post('api/repgroups', searchFilter).subscribe(data => {
      if (this.SalesTeam = data['Model']) {
        this.SalesTeam = data['Model'].List;
        this.pageSize = data['Model'].Pager.TotalCount;
      }
    })
  }

  sortData(sort: Sort) {
    this.searchFilter.SortKey = sort.active;
    this.searchFilter.SortOrder = sort.direction;
    this.getAllSalesTeam(this.searchFilter);
  }

  openCreate() {
    this.httpClient.get('lookup/getalldirectors').subscribe(data => {
      this.SalesDirectors = data['Model'].List
    })
    this.httpClient.get('lookup/getallmanagers').subscribe(data => {
      this.Managers = data['Model'].List
    })
    this.newSalesTeamModel = new SalesTeamModel();
  }

  create(salesTeamModel: SalesTeamModel) {
    this.httpClient.post('api/repgroups/save', salesTeamModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Saved", 200);
        this.getAllSalesTeam(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#create-sales-team-mod').modal('close');
      }
    });
    return false;
  }

  edit(salesTeamId: number) {
    this.httpClient.get('lookup/getalldirectors').subscribe(data => {
      this.SalesDirectors = data['Model'].List
    })
    this.httpClient.get('lookup/getallmanagers').subscribe(data => {
      this.Managers = data['Model'].List
    })
    this.httpClient.get('api/repgroups/' + salesTeamId).subscribe(data => {
      this.newSalesTeamModel = data['Model'];
      this.newSalesTeamModel.Id = salesTeamId;
    });
  }

  detail(salesTeamId: number) {
    this.httpClient.get('api/repgroups/' + salesTeamId).subscribe(data => {
      this.newSalesTeamModel = data['Model'];
      this.newSalesTeamModel.Id = salesTeamId;
    });
  }

  update(salesTeamModel: SalesTeamModel) {
    this.httpClient.put('api/repgroups/' + salesTeamModel.Id, salesTeamModel).subscribe(data => {
      if (data["Status"] == 200) {
        this.messageService.openSnackBar("Data Successfully Updated", 200);
        this.getAllSalesTeam(this.searchFilter);
        this.isMouseEnterRegistered = false;
        $('#edit-sales-team-mod').modal('close');
      }
    });
    return false
  }

  delete(salesTeamId: number) {
    this.salesTeamId = salesTeamId;
    return false;
  }

  deleteConfirm() {
    if (this.salesTeamId <= 0)
      return true;
    this.httpClient.delete('api/repgroups/' + this.salesTeamId).subscribe(data => {
      if (data["Model"]) {
        this.messageService.openSnackBar("Data Successfully Deleted", 200);
        this.getAllSalesTeam(this.searchFilter);
        this.isMouseEnterRegistered = false;
      }
      else {
        this.messageService.openSnackBar(data["Message"], 500);
      }
    });
    return false;
  }

  ngAfterViewInit() {
    $('.modal').modal();
  }

  mouseEnter() {
    if (!this.isMouseEnterRegistered) {
      $('.dropdown-button').dropdown();
      this.isMouseEnterRegistered = true;
    }
  }
}

// export class DialogOverviewExampleDialog {

//   constructor(
//     public dialogRef: MatDialogRef<DialogOverviewExampleDialog>,
//     @Inject(MAT_DIALOG_DATA) public data: any) { }

//   onNoClick(): void {
//     this.dialogRef.close();
//   }

// }