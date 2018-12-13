import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import { EventModel } from '../models/events.model';
import { Observable, from, of } from 'rxjs';
@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {

  private hubConnection: HubConnection | undefined;
  public async: any;
  errorMessage: string;
  dataSource: Observable<EventModel[]> | undefined;
  messages: EventModel[] = [];
  displayedColumns: string[] = ['name', 'description'];

  constructor() {
  }

  ngOnInit() {

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/eventhub')
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection.start().catch(err => this.errorMessage = err.toString());

    this.hubConnection.on('EventMessageArrived', (data: EventModel) => {
      console.log('message received.');
      if (!this.messages || this.messages == undefined) {
        this.errorMessage = 'messages not defined!';
      }
      if (this.messages.length > 5) {
        this.messages = [...this.messages.slice(1), data];
      } else {
        // this.messages.push(data);
        this.messages = [...this.messages, data];
      }
      this.dataSource = of(this.messages);
    });
  }
}
