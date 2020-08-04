import { Component } from '@angular/core';
import { JsonDataService } from './json-data.service';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  //To store the string URL for iFrame SRC
  public ReportUrl: SafeResourceUrl;

  //Store the latest product Id selected
  public ProductId: string = "";

  //Store the latest country selected
  public Country: string = "";

  //Root url of the RDLC Server
  private rootUrl: string = "http://localhost:51540/RDLCReportViewer?";
  constructor(public _jsonDataService: JsonDataService, private _domSanitizer: DomSanitizer) {
  }

  ViewReport(){
    this.ReportUrl = this._domSanitizer.bypassSecurityTrustResourceUrl(this.getQueryString());
  }

  private getQueryString(): string{
    return this.rootUrl + "ProductId=" + this.ProductId +
            "&ShipCountry=" + this.Country
  }
}
