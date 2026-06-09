import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskOtherCatalogs } from './risk-other-catalogs';

describe('RiskOtherCatalogs', () => {
  let component: RiskOtherCatalogs;
  let fixture: ComponentFixture<RiskOtherCatalogs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RiskOtherCatalogs],
    }).compileComponents();

    fixture = TestBed.createComponent(RiskOtherCatalogs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
