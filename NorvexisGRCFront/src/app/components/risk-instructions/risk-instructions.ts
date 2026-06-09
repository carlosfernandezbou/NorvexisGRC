import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';

interface RiskInstruction {
  title: string;
  description: string;
}

@Component({
  selector: 'app-risk-instructions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './risk-instructions.html',
  styleUrl: './risk-instructions.css'
})
export class RiskInstructionsComponent implements OnInit {
  private readonly cdr = inject(ChangeDetectorRef);

  instructions: RiskInstruction[] = [];

  async ngOnInit(): Promise<void> {
    await this.loadInstructions();
  }

  private async loadInstructions(): Promise<void> {
    try {
      const response = await fetch('/data/RiskManagement/instructions.json');
      const json = await response.json();
      this.instructions = json.instructions || [];
    } catch (error) {
      console.error('Error loading risk instructions', error);
      this.instructions = [];
    } finally {
      this.cdr.detectChanges();
    }
  }
}
