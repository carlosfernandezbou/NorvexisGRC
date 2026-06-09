import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SOA } from './soa';

describe('SOA', () => {
  let component: SOA;
  let fixture: ComponentFixture<SOA>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SOA],
    }).compileComponents();

    fixture = TestBed.createComponent(SOA);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
