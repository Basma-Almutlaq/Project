import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { LoginRequest, LoginResponse } from '../models/auth.model';
import {jwtDecode} from 'jwt-decode';
import { TicketStatus } from '../dashboard/dashboard';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5123/api/Auth'; 

  constructor(private http: HttpClient) {}

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }).pipe(
      catchError(error => {
        return throwError(() => ({
          status: error.status,
          error: error.error
        }));
      })
    );
  }  

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, data);
  }

  storeToken(token: string): void {
    localStorage.setItem('authToken', token);
  
    try {
      const decoded: any = jwtDecode(token);
  
      const roleClaimKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  
      if (decoded && decoded[roleClaimKey]) {
        localStorage.setItem('userRole', decoded[roleClaimKey]);
      } else {
        console.warn('Role claim not found in token');
      }
    } catch (e) {
      console.error('Error decoding token', e);
    }
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getRole(): string | null {
    return localStorage.getItem('userRole');
  }  

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userRole');
  }
  
  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;
  
    try {
      const decoded: any = jwtDecode(token);
      const nameIdClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
  
      return decoded?.[nameIdClaim] || null;
    } catch (e) {
      console.error('Error decoding token', e);
      return null;
    }
  }
 
  updateTicketStatus(ticketId: number, newStatus: TicketStatus): Observable<void> {
    return this.http.put<void>(
      `http://localhost:5123/api/tickets/${ticketId}/status`,
      newStatus,
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json',
          Authorization: `Bearer ${this.getToken() ?? ''}`
        })
      }
    );
  }  
  
  createTicket(data: { title: string; description: string; createdBy: string }): Observable<any> {
    return this.http.post<any>(
      'http://localhost:5123/api/tickets',
      JSON.stringify(data),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json',
          Authorization: `Bearer ${this.getToken() ?? ''}`
        })
      }
    );
  }
    
}
