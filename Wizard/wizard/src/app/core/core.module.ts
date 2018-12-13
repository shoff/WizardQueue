import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventService } from './services/event.service';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [],
  imports: [
    HttpClientModule,
    CommonModule
  ],
  providers: [
    EventService
  ]
})
export class CoreModule { }
