import { Component, OnInit } from '@angular/core';
import { EventService } from 'src/app/core/services/event.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EventModel } from '../models/events.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateComponent implements OnInit {
  eventMessageForm: FormGroup;
  errorMessage: string;
  eventObserver$: Observable<EventModel>;

  constructor(
    private eventService: EventService,
    private formBuilder: FormBuilder) {
    this.eventMessageForm = this.createFormGroupWithBuilder();
  }

  ngOnInit() {
  }

  createFormGroupWithBuilder() {
    return this.formBuilder.group({
      eventName: '',
      eventDescription: ''
    });
  }

  onSubmit() {
    this.errorMessage = null;
    // Make sure to create a deep copy of the form-model
    const result: EventModel = Object.assign({}, this.eventMessageForm.value);

    // Do useful stuff with the gathered data
    console.log(result);
    this.eventObserver$ = this.eventService.addEventMessage(result);
    this.eventObserver$.subscribe((result)=>{
      this.revert();
      
    }, (error)=> {
      this.errorMessage = error.message;
    });
  }
  revert() {
    // Resets to blank object
    this.eventMessageForm.reset();

    // Resets to provided model
    this.eventMessageForm.reset({ eventMessage: new EventModel() });
  }
}
