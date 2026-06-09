import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskThreatsCatalog } from './risk-threats-catalog';

describe('RiskThreatsCatalog', () => {
  let component: RiskThreatsCatalog;
  let fixture: ComponentFixture<RiskThreatsCatalog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskThreatsCatalog],
    }).compileComponents();

    fixture = TestBed.createComponent(RiskThreatsCatalog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
