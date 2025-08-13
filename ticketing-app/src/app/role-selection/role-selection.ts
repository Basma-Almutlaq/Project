import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-selection',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './role-selection.html',
  styleUrls: ['./role-selection.scss']
})
export class RoleSelectionComponent {
  constructor(private router: Router, private authService: AuthService) {}

  selectRole(role: string) {
    localStorage.setItem('selectedRole', role);
    this.router.navigate(['/register'], { queryParams: { role } });
  }
}

