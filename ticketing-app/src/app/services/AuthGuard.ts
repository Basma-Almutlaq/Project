import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';
import {jwtDecode} from 'jwt-decode';


@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    const token = this.authService.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return false;
    }
  
    const decoded: any = jwtDecode(token);
    if (decoded.role !== 'Admin' && this.router.url.includes('admin')) {
      this.router.navigate(['/']);
      return false;
    }
    return true;
  }
  
}
