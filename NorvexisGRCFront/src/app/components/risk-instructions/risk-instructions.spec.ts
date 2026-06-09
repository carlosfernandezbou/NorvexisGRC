import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskInstructions } from './risk-instructions';

describe('RiskInstructions', () => {
  let component: RiskInstructions;
  let fixture: ComponentFixture<RiskInstructions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskInstructions],
    }).compileComponents();

    fixture = TestBed.createComponent(RiskInstructions);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
