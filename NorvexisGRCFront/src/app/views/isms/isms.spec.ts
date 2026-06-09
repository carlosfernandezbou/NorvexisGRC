import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Isms } from './isms';

describe('Isms', () => {
  let component: Isms;
  let fixture: ComponentFixture<Isms>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Isms],
    }).compileComponents();

    fixture = TestBed.createComponent(Isms);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
