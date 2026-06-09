import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskRegister } from './risk-register';

describe('RiskRegister', () => {
  let component: RiskRegister;
  let fixture: ComponentFixture<RiskRegister>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskRegister],
    }).compileComponents();

    fixture = TestBed.createComponent(RiskRegister);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
