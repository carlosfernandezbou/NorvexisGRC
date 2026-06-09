import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom, forkJoin, Observable } from 'rxjs';
import { ServicesService } from '../../services/services';

import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

interface RiskIdentificationPayload {
  riskId: string;
  category: string;
  subCategory: string;
  assetType: string;
  assetName: string;
  vulnerability: string;
  threats: string;
  coreValueImpacted: string;
}

interface RiskAssessmentPayload {
  riskId: string;
  riskImpactDescription: string;
  existingControls: string;
  impact: string;
  likelihood: string;
  riskCategory: string;
}

interface RiskTreatmentPayload {
  riskId: string;
  riskTreatmentOption: string;
  treatmentPlan: string;
  owner: string;
  dueDate: string;
  status: string;
  residualRisk: string;
  comments: string;
}

interface RiskFormAssessmentModel extends RiskAssessmentPayload {
  documentId: string;
}

interface RiskFormTreatmentModel extends RiskTreatmentPayload {
  documentId: string;
}

interface RiskFormModel
  extends RiskIdentificationPayload,
  RiskAssessmentPayload,
  RiskTreatmentPayload {
  description: string;
  assessments: RiskFormAssessmentModel[];
  treatments: RiskFormTreatmentModel[];
}

interface RiskRegisterItem {
  riskId: string;
  id: string;
  identificationDocumentId: string;
  assessmentDocumentId: string;
  assessmentDocumentIds: string[];
  treatmentDocumentIds: string[];
  description: string;
  category: string;
  subCategory: string;
  assetType: string;
  assetName: string;
  vulnerability: string;
  threats: string;
  coreValueImpacted: string;
  riskImpactDescription: string;
  existingControls: string;
  impact: string;
  likelihood: string;
  riskCategory: string;
  riskTreatmentOption: string;
  treatmentPlan: string;
  owner: string;
  dueDate: string;
  status: string;
  residualRisk: string;
  comments: string;
  assessments: Array<{
    documentId: string;
    riskImpactDescription: string;
    existingControls: string;
    impact: string;
    likelihood: string;
    riskCategory: string;
    comments: string;
  }>;
  treatments: Array<{
    documentId: string;
    riskTreatmentOption: string;
    treatmentPlan: string;
    owner: string;
    dueDate: string;
    status: string;
    residualRisk: string;
    comments: string;
  }>;
}

@Component({
  selector: 'app-risk-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './risk-register.html',
  styleUrl: './risk-register.css'
})
export class RiskRegisterComponent implements OnInit {
  /* =========================================================
     Dependencies
     ========================================================= */
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly api = inject(ServicesService);

  /* =========================================================
     Static UI configuration
     ========================================================= */
  readonly sections = [
    'Instructions',
    'Risk register',
    'Threats catalog',
    'Risk categories catalog',
    'Other catalogs'
  ];

  /* =========================================================
     UI state
     ========================================================= */
  menuOpen = false;
  loading = false;
  submittingRisk = false;
  deletingRisk = false;

  addRiskOpen = false;
  isEditMode = false;
  editingRiskId = '';
  editAssessmentNumber = 1;
  editTreatmentNumber = 1;
  selectedSection = 'Risk register';
  selectedRisk: RiskRegisterItem | null = null;

  /* =========================================================
     Feedback messages
     ========================================================= */
  addRiskError = '';
  addRiskSuccess = '';
  deleteRiskError = '';
  deleteRiskSuccess = '';

  /* =========================================================
     Section data
     ========================================================= */
  riskRegister: RiskRegisterItem[] = [];

  /* =========================================================
     Add risk form model
     ========================================================= */
  newRisk: RiskFormModel = this.createEmptyRiskForm();

  async ngOnInit(): Promise<void> {
    await this.loadSectionData(this.selectedSection);
  }

  /* =========================================================
     //UI Helpers
     Generic UI helpers
     ========================================================= */

  createEmptyRiskForm(): RiskFormModel {
    return {
      riskId: '',
      description: '',
      category: '',
      subCategory: '',
      assetType: '',
      assetName: '',
      vulnerability: '',
      threats: '',
      coreValueImpacted: '',
      riskImpactDescription: '',
      existingControls: '',
      impact: '',
      likelihood: '',
      riskCategory: '',
      riskTreatmentOption: '',
      treatmentPlan: '',
      owner: '',
      dueDate: '',
      status: 'Open',
      residualRisk: '',
      comments: '',
      assessments: [],
      treatments: []
    };
  }

  private createEmptyAssessment(riskId: string): RiskFormAssessmentModel {
    return {
      documentId: '',
      riskId,
      riskImpactDescription: '',
      existingControls: '',
      impact: '',
      likelihood: '',
      riskCategory: ''
    };
  }

  private createEmptyTreatment(riskId: string): RiskFormTreatmentModel {
    return {
      documentId: '',
      riskId,
      riskTreatmentOption: '',
      treatmentPlan: '',
      owner: '',
      dueDate: '',
      status: 'Open',
      residualRisk: '',
      comments: ''
    };
  }

  /**
   * Opens or closes the hamburger menu.
   */
  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  /**
   * Changes the current section and loads its data if needed.
   */
  async selectSection(section: string): Promise<void> {
    this.selectedSection = section;
    this.menuOpen = false;
    await this.loadSectionData(section);
  }

  /**
   * Loads the data needed for the selected section.
   * Static JSON files are loaded only once; the risk register is refreshed on demand.
   */
  async loadSectionData(section: string): Promise<void> {
    this.loading = true;

    try {
      if (section === 'Risk register') {
        await this.loadRiskRegisterFromApi();
      }


    } catch (error) {
      console.error(`Error loading section "${section}"`, error);
    } finally {
      this.loading = false;
      this.cdr.detectChanges();
    }
  }

  /* =========================================================
     //Load Risk Register
     Risk register loading and transformation
     ========================================================= */
  async loadRiskRegisterFromApi(): Promise<void> {
    try {
      const [identifications, assessments, treatments] = await firstValueFrom(
        forkJoin([
          this.api.getRiskIdentifications(),
          this.api.getRiskAssessments(),
          this.api.getRiskTreatment(),
        ])
      );

      this.riskRegister = this.mergeRiskData(
        Array.isArray(identifications) ? identifications : [],
        Array.isArray(assessments) ? assessments : [],
        Array.isArray(treatments) ? treatments : []
      );
    } catch (error) {
      console.error('Error loading risk register from APIs', error);
      this.riskRegister = [];
    }
  }

  private mergeRiskData(
    identifications: any[],
    assessments: any[],
    treatments: any[]
  ): RiskRegisterItem[] {
    const identificationMap = new Map<string, any>();
    const assessmentMap = new Map<string, any[]>();
    const treatmentMap = new Map<string, any[]>();

    for (const item of identifications) {
      const riskId = this.readValue(item, 'riskId');
      if (riskId) {
        identificationMap.set(riskId, item);
      }
    }

    for (const item of assessments) {
      const riskId = this.readValue(item, 'riskId');
      if (!riskId) {
        continue;
      }

      const current = assessmentMap.get(riskId) || [];
      current.push(item);
      assessmentMap.set(riskId, current);
    }

    for (const item of treatments) {
      const riskId = this.readValue(item, 'riskId');
      if (!riskId) {
        continue;
      }

      const current = treatmentMap.get(riskId) || [];
      current.push(item);
      treatmentMap.set(riskId, current);
    }

    const riskIds = Array.from(
      new Set([
        ...Array.from(identificationMap.keys()),
        ...Array.from(assessmentMap.keys()),
        ...Array.from(treatmentMap.keys())
      ])
    ).sort();

    return riskIds.map((riskId) => {
      const identification = identificationMap.get(riskId) || null;
      const assessmentList = assessmentMap.get(riskId) || [];
      const assessment = assessmentList[0] || null;
      const treatmentList = treatmentMap.get(riskId) || [];
      const firstTreatment = treatmentList[0] || null;

      return {
        riskId,
        id: riskId,
        identificationDocumentId: this.readDocumentId(identification),
        assessmentDocumentId: this.readDocumentId(assessment),
        assessmentDocumentIds: assessmentList
          .map((item) => this.readDocumentId(item))
          .filter((id) => !!id),
        treatmentDocumentIds: treatmentList
          .map((item) => this.readDocumentId(item))
          .filter((id) => !!id),
        description:
          this.readValue(identification, 'description') ||
          this.readValue(identification, 'assetName') ||
          riskId,
        category: this.readValue(identification, 'category'),
        subCategory: this.readValue(identification, 'subCategory'),
        assetType: this.readValue(identification, 'assetType'),
        assetName: this.readValue(identification, 'assetName'),
        vulnerability: this.readValue(identification, 'vulnerability'),
        threats: this.readValue(identification, 'threats'),
        coreValueImpacted: this.readValue(identification, 'coreValueImpacted'),
        riskImpactDescription: this.readValue(assessment, 'riskImpactDescription'),
        existingControls: this.readValue(assessment, 'existingControls'),
        impact: this.normalizeSelectValue(this.readValue(assessment, 'impact')),
        likelihood: this.normalizeSelectValue(this.readValue(assessment, 'likelihood')),
        riskCategory: this.readValue(assessment, 'riskCategory'),
        riskTreatmentOption: this.readValue(firstTreatment, 'riskTreatmentOption'),
        treatmentPlan: this.readValue(firstTreatment, 'treatmentPlan'),
        owner: this.readValue(firstTreatment, 'owner'),
        dueDate: this.readValue(firstTreatment, 'dueDate'),
        status: this.readValue(firstTreatment, 'status'),
        residualRisk: this.readValue(firstTreatment, 'residualRisk'),
        comments: this.readValue(firstTreatment, 'comments'),
        assessments: assessmentList.map((item) => ({
          documentId: this.readDocumentId(item),
          riskImpactDescription: this.readValue(item, 'riskImpactDescription'),
          existingControls: this.readValue(item, 'existingControls'),
          impact: this.normalizeSelectValue(this.readValue(item, 'impact')),
          likelihood: this.normalizeSelectValue(this.readValue(item, 'likelihood')),
          riskCategory: this.readValue(item, 'riskCategory'),
          comments: this.readValue(item, 'comments')
        })),
        treatments: treatmentList.map((item) => ({
          documentId: this.readDocumentId(item),
          riskTreatmentOption: this.readValue(item, 'riskTreatmentOption'),
          treatmentPlan: this.readValue(item, 'treatmentPlan'),
          owner: this.readValue(item, 'owner'),
          dueDate: this.readValue(item, 'dueDate'),
          status: this.readValue(item, 'status'),
          residualRisk: this.readValue(item, 'residualRisk'),
          comments: this.readValue(item, 'comments')
        }))
      };
    });
  }


  private readDocumentId(source: any): string {
    if (!source) {
      return '';
    }

    const variants = ['id', 'Id', '_id'];
    const value = variants
      .map((variant) => source[variant])
      .find((item) => item !== undefined && item !== null);

    return String(value ?? '').trim();
  }


  private readValue(source: any, key: string): string {
    if (!source) {
      return '';
    }

    const variants = [key, key.charAt(0).toUpperCase() + key.slice(1)];
    const value = variants
      .map((variant) => source[variant])
      .find((item) => item !== undefined && item !== null);

    return String(value ?? '').trim();
  }

  private normalizeSelectValue(value: string): string {
    return String(value || '').trim().replace(/-\s+/g, '-');
  }

  /* =========================================================
     //Update Form
     Add and edit risk form flow
     ========================================================= */
  openAddRisk(): void {
    this.addRiskError = '';
    this.addRiskSuccess = '';
    this.isEditMode = false;
    this.editingRiskId = '';
    this.editAssessmentNumber = 1;
    this.editTreatmentNumber = 1;
    this.newRisk = this.createEmptyRiskForm();
    this.newRisk.riskId = this.getNextRiskId();
    this.newRisk.status = 'Open';
    this.addRiskOpen = true;
    this.selectedRisk = null;
  }


  openEditRisk(risk: RiskRegisterItem): void {
    this.addRiskError = '';
    this.addRiskSuccess = '';
    this.isEditMode = true;
    this.editingRiskId = risk.riskId;
    this.editAssessmentNumber = 1;
    this.editTreatmentNumber = 1;

    const assessments = (risk.assessments?.length
      ? risk.assessments
      : [{
        documentId: risk.assessmentDocumentId,
        riskImpactDescription: risk.riskImpactDescription,
        existingControls: risk.existingControls,
        impact: risk.impact,
        likelihood: risk.likelihood,
        riskCategory: risk.riskCategory,
        comments: ''
      }]
    ).map((assessment) => ({
      documentId: String(assessment.documentId || '').trim(),
      riskId: String(risk.riskId || '').trim(),
      riskImpactDescription: String(assessment.riskImpactDescription || '').trim(),
      existingControls: String(assessment.existingControls || '').trim(),
      impact: this.normalizeSelectValue(assessment.impact),
      likelihood: this.normalizeSelectValue(assessment.likelihood),
      riskCategory: String(assessment.riskCategory || '').trim()
    }));

    const treatments = (risk.treatments?.length
      ? risk.treatments
      : [{
        documentId: (risk.treatmentDocumentIds || [])[0] || '',
        riskTreatmentOption: risk.riskTreatmentOption,
        treatmentPlan: risk.treatmentPlan,
        owner: risk.owner,
        dueDate: risk.dueDate,
        status: risk.status,
        residualRisk: risk.residualRisk,
        comments: risk.comments
      }]
    ).map((treatment) => ({
      documentId: String(treatment.documentId || '').trim(),
      riskId: String(risk.riskId || '').trim(),
      riskTreatmentOption: String(treatment.riskTreatmentOption || '').trim(),
      treatmentPlan: String(treatment.treatmentPlan || '').trim(),
      owner: String(treatment.owner || '').trim(),
      dueDate: String(treatment.dueDate || '').trim(),
      status: String(treatment.status || 'Open').trim(),
      residualRisk: String(treatment.residualRisk || '').trim(),
      comments: String(treatment.comments || '').trim()
    }));

    const firstAssessment = assessments[0] || this.createEmptyRiskForm();
    const firstTreatment = treatments[0] || this.createEmptyRiskForm();

    this.newRisk = {
      riskId: String(risk.riskId || '').trim(),
      description: String(risk.description || '').trim(),
      category: String(risk.category || '').trim(),
      subCategory: String(risk.subCategory || '').trim(),
      assetType: String(risk.assetType || '').trim(),
      assetName: String(risk.assetName || '').trim(),
      vulnerability: String(risk.vulnerability || '').trim(),
      threats: String(risk.threats || '').trim(),
      coreValueImpacted: String(risk.coreValueImpacted || '').trim(),
      riskImpactDescription: String(firstAssessment.riskImpactDescription || '').trim(),
      existingControls: String(firstAssessment.existingControls || '').trim(),
      impact: this.normalizeSelectValue(firstAssessment.impact),
      likelihood: this.normalizeSelectValue(firstAssessment.likelihood),
      riskCategory: String(firstAssessment.riskCategory || '').trim(),
      riskTreatmentOption: String(firstTreatment.riskTreatmentOption || '').trim(),
      treatmentPlan: String(firstTreatment.treatmentPlan || '').trim(),
      owner: String(firstTreatment.owner || '').trim(),
      dueDate: String(firstTreatment.dueDate || '').trim(),
      status: String(firstTreatment.status || 'Open').trim(),
      residualRisk: String(firstTreatment.residualRisk || '').trim(),
      comments: String(firstTreatment.comments || '').trim(),
      assessments,
      treatments
    };

    this.addRiskOpen = true;
    this.selectedRisk = risk;
  }

  openAddAssessment(risk: RiskRegisterItem): void {
    this.openEditRisk(risk);
    this.newRisk.assessments.push(this.createEmptyAssessment(risk.riskId));
    this.editAssessmentNumber = this.newRisk.assessments.length;
    this.addRiskSuccess = '';
    this.scrollToRiskForm(risk.riskId);
  }

  openAddTreatment(risk: RiskRegisterItem): void {
    this.openEditRisk(risk);
    this.newRisk.treatments.push(this.createEmptyTreatment(risk.riskId));
    this.editTreatmentNumber = this.newRisk.treatments.length;
    this.addRiskSuccess = '';
    this.scrollToRiskForm(risk.riskId);
  }

  private scrollToRiskForm(riskId: string): void {
    window.setTimeout(() => {
      const form = document.querySelector(`[data-risk-form-for="${riskId}"]`);
      form?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

 
  closeRiskForm(): void {
    this.addRiskOpen = false;
    this.isEditMode = false;
    this.editingRiskId = '';
    this.editAssessmentNumber = 1;
    this.editTreatmentNumber = 1;
    this.addRiskError = '';
    this.newRisk = this.createEmptyRiskForm();
  }

  validateRiskForm(): string {
    if (!this.newRisk.description?.trim()) return 'Description is required.';
    if (!this.newRisk.category?.trim()) return 'Category is required.';
    if (!this.newRisk.assetName?.trim()) return 'Asset name is required.';
    if (!this.newRisk.vulnerability?.trim()) return 'Vulnerability is required.';
    if (!this.newRisk.threats?.trim()) return 'Threats is required.';
    if (!this.newRisk.coreValueImpacted?.trim()) return 'Select at least one core value impacted.';

    const selectedAssessment = this.isEditMode
      ? this.getSelectedAssessment()
      : {
        riskImpactDescription: this.newRisk.riskImpactDescription,
        existingControls: this.newRisk.existingControls,
        impact: this.newRisk.impact,
        likelihood: this.newRisk.likelihood,
        riskCategory: this.newRisk.riskCategory
      };

    const assessmentLabel = this.isEditMode ? `Assessment ${this.editAssessmentNumber}` : 'Assessment';
    if (!selectedAssessment) return `${assessmentLabel}: select a valid assessment number.`;
    if (!selectedAssessment.riskImpactDescription?.trim()) return `${assessmentLabel}: risk impact description is required.`;
    if (!selectedAssessment.existingControls?.trim()) return `${assessmentLabel}: existing controls is required.`;
    if (!selectedAssessment.impact?.trim()) return `${assessmentLabel}: impact is required.`;
    if (!selectedAssessment.likelihood?.trim()) return `${assessmentLabel}: likelihood is required.`;
    if (!selectedAssessment.riskCategory?.trim()) return `${assessmentLabel}: risk category could not be calculated.`;

    const selectedTreatment = this.isEditMode
      ? this.getSelectedTreatment()
      : {
        riskTreatmentOption: this.newRisk.riskTreatmentOption,
        treatmentPlan: this.newRisk.treatmentPlan,
        owner: this.newRisk.owner,
        dueDate: this.newRisk.dueDate,
        status: this.newRisk.status
      };

    const treatmentLabel = this.isEditMode ? `Treatment ${this.editTreatmentNumber}` : 'Treatment';
    if (!selectedTreatment) return `${treatmentLabel}: select a valid treatment number.`;
    if (!selectedTreatment.riskTreatmentOption?.trim()) return `${treatmentLabel}: risk treatment option is required.`;
    if (!selectedTreatment.treatmentPlan?.trim()) return `${treatmentLabel}: treatment plan is required.`;
    if (!selectedTreatment.owner?.trim()) return `${treatmentLabel}: owner is required.`;
    if (!selectedTreatment.dueDate?.trim()) return `${treatmentLabel}: due date is required.`;
    if (!selectedTreatment.status?.trim()) return `${treatmentLabel}: status is required.`;

    return '';
  }

  //SubitForm
  async submitRiskForm(): Promise<void> {
    if (this.isEditMode) {
      await this.submitEditRisk();
      return;
    }

    await this.submitAddRisk();
  }

  async submitAddRisk(): Promise<void> {
    this.addRiskError = '';
    this.addRiskSuccess = '';

    const validationError = this.validateRiskForm();
    if (validationError) {
      this.addRiskError = validationError;
      return;
    }

    const generatedRiskId = this.getNextRiskId();
    this.newRisk.riskId = generatedRiskId;

    const identificationPayload: RiskIdentificationPayload = {
      riskId: generatedRiskId,
      category: this.newRisk.category.trim(),
      subCategory: this.newRisk.subCategory.trim(),
      assetType: this.newRisk.assetType.trim(),
      assetName: this.newRisk.assetName.trim(),
      vulnerability: this.newRisk.vulnerability.trim(),
      threats: this.newRisk.threats.trim(),
      coreValueImpacted: this.newRisk.coreValueImpacted.trim()
    };

    const assessmentPayload: RiskAssessmentPayload = {
      riskId: generatedRiskId,
      riskImpactDescription: this.newRisk.riskImpactDescription.trim(),
      existingControls: this.newRisk.existingControls.trim(),
      impact: this.normalizeSelectValue(this.newRisk.impact),
      likelihood: this.normalizeSelectValue(this.newRisk.likelihood),
      riskCategory: this.newRisk.riskCategory.trim()
    };

    const treatmentPayload: RiskTreatmentPayload = {
      riskId: generatedRiskId,
      riskTreatmentOption: this.newRisk.riskTreatmentOption.trim(),
      treatmentPlan: this.newRisk.treatmentPlan.trim(),
      owner: this.newRisk.owner.trim(),
      dueDate: this.newRisk.dueDate.trim(),
      status: this.newRisk.status.trim(),
      residualRisk: this.newRisk.residualRisk.trim(),
      comments: this.newRisk.comments.trim()
    };

    this.submittingRisk = true;

    try {
      await firstValueFrom(
        forkJoin([
          this.api.createRiskIdentification(identificationPayload),
          this.api.createRiskAssessment(assessmentPayload),
          this.api.createRiskTreatment(treatmentPayload)
        ])
      );

      const createdRiskId = generatedRiskId;

      this.addRiskSuccess = `Risk ${createdRiskId} created successfully.`;
      this.addRiskOpen = false;

      await this.loadRiskRegisterFromApi();
      this.selectedRisk = this.riskRegister.find((risk) => risk.riskId === createdRiskId) || null;
      this.newRisk = this.createEmptyRiskForm();
    } catch (error) {
      console.error('Error creating risk', error);
      this.addRiskError = 'Could not create the risk. Check the API payload and required fields.';
      this.addRiskOpen = true;
    } finally {
      this.submittingRisk = false;
      this.cdr.detectChanges();
    }
  }

  async submitEditRisk(): Promise<void> {
    this.addRiskError = '';
    this.addRiskSuccess = '';

    const validationError = this.validateRiskForm();
    if (validationError) {
      this.addRiskError = validationError;
      return;
    }

    const originalRisk = this.riskRegister.find((item) => item.riskId === this.editingRiskId);
    if (!originalRisk) {
      this.addRiskError = 'The selected risk could not be found for editing.';
      return;
    }

    const identificationDocId = String(originalRisk.identificationDocumentId || '').trim();
    const assessmentToUpdate = this.getSelectedAssessment();
    const treatmentToUpdate = this.getSelectedTreatment();
    const assessmentDocId = String(assessmentToUpdate?.documentId || '').trim();
    const treatmentDocId = String(treatmentToUpdate?.documentId || '').trim();

    if (!identificationDocId || !assessmentToUpdate || !treatmentToUpdate) {
      this.addRiskError = 'The selected risk does not contain the document ids required for editing.';
      return;
    }

    const lockedRiskId = originalRisk.riskId;
    this.newRisk.riskId = lockedRiskId;

    const identificationPayload: RiskIdentificationPayload = {
      riskId: lockedRiskId,
      category: this.newRisk.category.trim(),
      subCategory: this.newRisk.subCategory.trim(),
      assetType: this.newRisk.assetType.trim(),
      assetName: this.newRisk.assetName.trim(),
      vulnerability: this.newRisk.vulnerability.trim(),
      threats: this.newRisk.threats.trim(),
      coreValueImpacted: this.newRisk.coreValueImpacted.trim()
    };

    const assessmentPayload: RiskAssessmentPayload = {
      riskId: lockedRiskId,
      riskImpactDescription: assessmentToUpdate.riskImpactDescription.trim(),
      existingControls: assessmentToUpdate.existingControls.trim(),
      impact: this.normalizeSelectValue(assessmentToUpdate.impact),
      likelihood: this.normalizeSelectValue(assessmentToUpdate.likelihood),
      riskCategory: assessmentToUpdate.riskCategory.trim()
    };

    const treatmentPayload: RiskTreatmentPayload = {
      riskId: lockedRiskId,
      riskTreatmentOption: treatmentToUpdate.riskTreatmentOption.trim(),
      treatmentPlan: treatmentToUpdate.treatmentPlan.trim(),
      owner: treatmentToUpdate.owner.trim(),
      dueDate: treatmentToUpdate.dueDate.trim(),
      status: treatmentToUpdate.status.trim(),
      residualRisk: treatmentToUpdate.residualRisk.trim(),
      comments: treatmentToUpdate.comments.trim()
    };

    this.submittingRisk = true;

    try {
      const saveCalls: Observable<any>[] = [
        this.api.updateRiskIdentification(identificationDocId, identificationPayload),
        assessmentDocId
          ? this.api.updateRiskAssessment(assessmentDocId, assessmentPayload)
          : this.api.createRiskAssessment(assessmentPayload),
        treatmentDocId
          ? this.api.updateRiskTreatment(treatmentDocId, treatmentPayload)
          : this.api.createRiskTreatment(treatmentPayload)
      ];

      await firstValueFrom(forkJoin(saveCalls));

      const updatedRiskId = lockedRiskId;
      this.addRiskSuccess = `Risk ${updatedRiskId} updated successfully.`;
      this.addRiskOpen = false;
      this.isEditMode = false;
      this.editingRiskId = '';

      await this.loadRiskRegisterFromApi();
      this.selectedRisk = this.riskRegister.find((risk) => risk.riskId === updatedRiskId) || null;
      this.newRisk = this.createEmptyRiskForm();
    } catch (error) {
      console.error('Error updating risk', error);
      this.addRiskError = 'Could not update the risk. Check the API payload and endpoint configuration.';
      this.addRiskOpen = true;
    } finally {
      this.submittingRisk = false;
      this.cdr.detectChanges();
    }
  }

  getNextRiskId(): string {
    const numbers = this.riskRegister
      .map((risk) => String(risk?.riskId || ''))
      .map((riskId) => {
        const match = riskId.match(/^R-(\d{3})$/i);
        return match ? Number(match[1]) : 0;
      });

    const next = (numbers.length ? Math.max(...numbers) : 0) + 1;
    return `R-${String(next).padStart(3, '0')}`;
  }


  isCoreValueSelected(value: string): boolean {
    const current = this.newRisk.coreValueImpacted || '';
    return current
      .split(',')
      .map((item) => item.trim())
      .includes(value);
  }

 
  onCoreValueChange(value: string, checked: boolean): void {
    const values = (this.newRisk.coreValueImpacted || '')
      .split(',')
      .map((item) => item.trim())
      .filter((item) => !!item);

    const next = checked
      ? Array.from(new Set([...values, value]))
      : values.filter((item) => item !== value);

    this.newRisk.coreValueImpacted = next.join(',');
  }

  updateRiskCategory(): void {
    this.newRisk.riskCategory = this.calculateRiskCategory(this.newRisk.impact, this.newRisk.likelihood);
  }


  getSelectedAssessment(): RiskFormAssessmentModel | null {
    const index = Number(this.editAssessmentNumber) - 1;
    return Number.isInteger(index) && index >= 0
      ? this.newRisk.assessments[index] || null
      : null;
  }

  getSelectedTreatment(): RiskFormTreatmentModel | null {
    const index = Number(this.editTreatmentNumber) - 1;
    return Number.isInteger(index) && index >= 0
      ? this.newRisk.treatments[index] || null
      : null;
  }

  clampSelectedAssessmentNumber(): void {
    const max = Math.max(this.newRisk.assessments.length, 1);
    const value = Number(this.editAssessmentNumber) || 1;
    this.editAssessmentNumber = Math.min(Math.max(Math.trunc(value), 1), max);
  }

  clampSelectedTreatmentNumber(): void {
    const max = Math.max(this.newRisk.treatments.length, 1);
    const value = Number(this.editTreatmentNumber) || 1;
    this.editTreatmentNumber = Math.min(Math.max(Math.trunc(value), 1), max);
  }

  updateAssessmentRiskCategory(index: number): void {
    const assessment = this.newRisk.assessments[index];
    if (!assessment) {
      return;
    }

    assessment.riskCategory = this.calculateRiskCategory(assessment.impact, assessment.likelihood);
  }

  private calculateRiskCategory(impactValue: string, likelihoodValue: string): string {
    const impactMap: Record<string, number> = {
      '1-Negligible': 1,
      '2-Limited': 2,
      '3-Considerable': 3,
      '4-Threatening the existence of the organization': 4
    };

    const likelihoodMap: Record<string, number> = {
      '1-Rarely': 1,
      '2-Medium': 2,
      '3-Frequently': 3,
      '4-Very frequently': 4
    };

    const matrix: Record<number, Record<number, string>> = {
      1: { 1: 'Low', 2: 'Low', 3: 'Low', 4: 'Low' },
      2: { 1: 'Low', 2: 'Low', 3: 'Medium', 4: 'High' },
      3: { 1: 'Medium', 2: 'Medium', 3: 'High', 4: 'Very high' },
      4: { 1: 'Medium', 2: 'High', 3: 'Very high', 4: 'Very high' }
    };

    const normalizedImpact = this.normalizeSelectValue(impactValue);
    const normalizedLikelihood = this.normalizeSelectValue(likelihoodValue);

    const impact = impactMap[normalizedImpact];
    const likelihood = likelihoodMap[normalizedLikelihood];

    return impact && likelihood ? matrix[impact][likelihood] : '';
  }

  // Delete Risk
  promptDeleteRisk(): void {
    const typedRiskId = window.prompt('Enter the Risk ID to delete (example: R-001):', 'R-001');
    if (typedRiskId === null) {
      return;
    }

    void this.deleteRiskById(typedRiskId);
  }

  async deleteRisk(risk: RiskRegisterItem | null): Promise<void> {
    await this.deleteRiskById(risk?.riskId || '');
  }

  async deleteRiskById(riskIdInput: string): Promise<void> {
    this.deleteRiskError = '';
    this.deleteRiskSuccess = '';

    const riskId = String(riskIdInput || '').trim().toUpperCase();

    if (!/^R-\d{3}$/i.test(riskId)) {
      this.deleteRiskError = 'Enter a valid Risk ID with format R-00x.';
      this.cdr.detectChanges();
      return;
    }

    const risk = this.riskRegister.find((item) => String(item?.riskId || '').toUpperCase() === riskId);
    if (!risk) {
      this.deleteRiskError = `Risk ${riskId} was not found in the current register.`;
      this.cdr.detectChanges();
      return;
    }

    const identificationDocId = String(risk.identificationDocumentId || '').trim();
    const assessmentDocIds = Array.isArray(risk.assessmentDocumentIds)
      ? risk.assessmentDocumentIds.map((id) => String(id || '').trim()).filter((id) => !!id)
      : String(risk.assessmentDocumentId || '').trim()
        ? [String(risk.assessmentDocumentId || '').trim()]
        : [];
    const treatmentDocIds = Array.isArray(risk.treatmentDocumentIds)
      ? risk.treatmentDocumentIds.map((id) => String(id || '').trim()).filter((id) => !!id)
      : [];

    if (!identificationDocId && assessmentDocIds.length === 0 && treatmentDocIds.length === 0) {
      this.deleteRiskError = `Risk ${riskId} does not have document ids to delete.`;
      this.cdr.detectChanges();
      return;
    }

    const confirmed = window.confirm(`Are you sure you want to delete risk ${riskId}? This action cannot be undone.`);
    if (!confirmed) {
      return;
    }

    this.deletingRisk = true;

    try {
      const deleteCalls: Observable<any>[] = [];

      if (identificationDocId) {
        deleteCalls.push(this.api.deleteRiskIdentification(identificationDocId));
      }

      for (const assessmentDocId of assessmentDocIds) {
        deleteCalls.push(this.api.deleteRiskAssessment(assessmentDocId));
      }

      for (const treatmentDocId of treatmentDocIds) {
        deleteCalls.push(this.api.deleteRiskTreatment(treatmentDocId));
      }

      await firstValueFrom(forkJoin(deleteCalls));

      this.riskRegister = this.riskRegister.filter((item) => String(item?.riskId || '').toUpperCase() !== riskId);

      if (String(this.selectedRisk?.riskId || '').toUpperCase() === riskId) {
        this.selectedRisk = null;
      }

      if (String(this.editingRiskId || '').toUpperCase() === riskId) {
        this.closeRiskForm();
      }

      this.deleteRiskSuccess = `Risk ${riskId} deleted successfully.`;
    } catch (error) {
      console.error('Error deleting risk', error);
      this.deleteRiskError = 'Could not delete the risk with its real document ids.';
    } finally {
      this.deletingRisk = false;
      this.cdr.detectChanges();
    }
  }

  async deleteAssessment(
    risk: RiskRegisterItem,
    assessment?: { documentId?: string },
    index = 0
  ): Promise<void> {
    this.deleteRiskError = '';
    this.deleteRiskSuccess = '';

    const riskId = String(risk?.riskId || '').trim();
    const documentId = String(
      assessment?.documentId ||
      risk?.assessments?.[index]?.documentId ||
      risk?.assessmentDocumentIds?.[index] ||
      risk?.assessmentDocumentId ||
      ''
    ).trim();

    if (!riskId || !documentId) {
      this.deleteRiskError = 'This assessment does not have a document id to delete.';
      this.cdr.detectChanges();
      return;
    }

    const confirmed = window.confirm(`Delete Assessment ${index + 1} from ${riskId}? This action cannot be undone.`);
    if (!confirmed) {
      return;
    }

    this.deletingRisk = true;

    try {
      await firstValueFrom(this.api.deleteRiskAssessment(documentId));
      this.deleteRiskSuccess = `Assessment ${index + 1} deleted from ${riskId}.`;

      await this.loadRiskRegisterFromApi();
      this.selectedRisk = this.riskRegister.find((item) => item.riskId === riskId) || null;

      if (this.isEditMode && this.editingRiskId === riskId) {
        const refreshedRisk = this.selectedRisk;
        if (refreshedRisk) {
          this.openEditRisk(refreshedRisk);
        } else {
          this.closeRiskForm();
        }
      }
    } catch (error) {
      console.error('Error deleting assessment', error);
      this.deleteRiskError = 'Could not delete the assessment.';
    } finally {
      this.deletingRisk = false;
      this.cdr.detectChanges();
    }
  }

  async deleteTreatment(
    risk: RiskRegisterItem,
    treatment?: { documentId?: string },
    index = 0
  ): Promise<void> {
    this.deleteRiskError = '';
    this.deleteRiskSuccess = '';

    const riskId = String(risk?.riskId || '').trim();
    const documentId = String(
      treatment?.documentId ||
      risk?.treatments?.[index]?.documentId ||
      risk?.treatmentDocumentIds?.[index] ||
      ''
    ).trim();

    if (!riskId || !documentId) {
      this.deleteRiskError = 'This treatment does not have a document id to delete.';
      this.cdr.detectChanges();
      return;
    }

    const confirmed = window.confirm(`Delete Treatment ${index + 1} from ${riskId}? This action cannot be undone.`);
    if (!confirmed) {
      return;
    }

    this.deletingRisk = true;

    try {
      await firstValueFrom(this.api.deleteRiskTreatment(documentId));
      this.deleteRiskSuccess = `Treatment ${index + 1} deleted from ${riskId}.`;

      await this.loadRiskRegisterFromApi();
      this.selectedRisk = this.riskRegister.find((item) => item.riskId === riskId) || null;

      if (this.isEditMode && this.editingRiskId === riskId) {
        const refreshedRisk = this.selectedRisk;
        if (refreshedRisk) {
          this.openEditRisk(refreshedRisk);
        } else {
          this.closeRiskForm();
        }
      }
    } catch (error) {
      console.error('Error deleting treatment', error);
      this.deleteRiskError = 'Could not delete the treatment.';
    } finally {
      this.deletingRisk = false;
      this.cdr.detectChanges();
    }
  }

  //Risk Detail
  selectRisk(risk: RiskRegisterItem): void {
    this.selectedRisk = risk;
  }

  closeRiskDetail(): void {
    this.selectedRisk = null;
  }

  getRiskLevelClass(level: string): string {
    const value = (level || '').trim().toLowerCase();

    switch (value) {
      case 'low':
        return 'risk-low';
      case 'medium':
        return 'risk-medium';
      case 'high':
        return 'risk-high';
      case 'very high':
        return 'risk-very-high';
      default:
        return 'risk-unknown';
    }
  }


  // Returns the CSS class used to paint the treatment status.
  getStatusClass(status: string): string {
    const value = (status || '').trim().toLowerCase();

    switch (value) {
      case 'open':
        return 'status-open';
      case 'closed':
        return 'status-closed';
      case 'in progress':
        return 'status-in-progress';
      default:
        return 'status-default';
    }
  }

  trackByRisk(_index: number, risk: RiskRegisterItem): string {
    return risk?.riskId ?? _index.toString();
  }

  exportToPDF(): void {
    const pdf = new jsPDF('l', 'mm', 'a4');

    const rows = this.riskRegister.map((risk) => [
      risk.riskId || '',
      risk.description || '',
      risk.category || '',
      risk.subCategory || '',
      risk.assetName || '',
      risk.riskCategory || '',
      risk.riskTreatmentOption || '',
      risk.status || '',
      risk.owner || '',
      risk.dueDate || '',
      risk.residualRisk || '',
      risk.comments || ''
    ]);

    const logo = new Image();
    logo.src = '/logo.png';

    logo.onload = () => {
      const pageWidth = pdf.internal.pageSize.getWidth();

      pdf.addImage(logo, 'PNG', pageWidth - 55, 10, 40, 40);

      pdf.setFontSize(16);
      pdf.text('Risk Assessment & Treatment Plan', 14, 24);

      pdf.setFontSize(11);
      pdf.text(`Total Risks: ${this.riskRegister.length}`, 14, 31);

      autoTable(pdf, {
        startY: 53,
        head: [[
          'Risk ID',
          'Description',
          'Category',
          'Sub category',
          'Asset',
          'Criticality',
          'Treatment',
          'Status',
          'Owner',
          'Due date',
          'Residual risk',
          'Comments'
        ]],
        body: rows,
        styles: {
          fontSize: 7,
          cellPadding: 2,
          overflow: 'linebreak'
        },
        headStyles: {
          fillColor: [225, 15, 28]
        }
      });

      pdf.save('Risk-Register.pdf');
    };
  }

}
