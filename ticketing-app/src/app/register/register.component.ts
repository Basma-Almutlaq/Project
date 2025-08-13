import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service'; 
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
  imports: [ReactiveFormsModule, CommonModule, RouterModule]
})
export class RegisterComponent {
  role: string | null = null;
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.registerForm = this.fb.group({
        email: ['', [Validators.required, Validators.email]],  
        password: ['', [Validators.required, Validators.minLength(6)]], 
        role: ['User'] 
      });
      this.route.queryParamMap.subscribe(params => {
        const roleFromQuery = params.get('role');
        if (roleFromQuery) {
          this.role = roleFromQuery;
          this.registerForm.patchValue({ role: roleFromQuery });
        }
      });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Submitting:', this.registerForm.value);
  
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          this.router.navigate(['/login']);
        },
        error: (err) => {
          if (err.status === 400) {
            const errors = err.error?.errors || err.error;
            alert(JSON.stringify(errors, null, 2));
          } else {
            alert(err.error?.message || `Registration failed (Status: ${err.status})`);
          }
        }
      });      
    }
  }
  
}
