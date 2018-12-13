import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { WizardRoutingModule } from './wizard-routing.module';
import { CreateComponent } from './create/create.component';
import { DateSelectionComponent } from './date-selection/date-selection.component';
import { HttpClientModule } from '@angular/common/http';
import { AppMaterialModule } from '../app-material.module';
import { MessagesComponent } from './messages/messages.component';


@NgModule({
  declarations: [CreateComponent, DateSelectionComponent, MessagesComponent],
  imports: [
    AppMaterialModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    WizardRoutingModule
  ]
})
export class WizardModule { }
