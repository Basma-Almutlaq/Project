import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { formatDistanceToNow } from 'date-fns';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
  imports: [CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule
]
})
export class Dashboard implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = ['timeAgo', 'title', 'description', 'createdBy', 'status'];

  stats = { open: 0, inProgress: 0, resolved: 0, closed: 0 };
  isAdmin = false;
  showAddTicketModal = false;
  ticketForm: FormGroup;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {
    this.ticketForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.isAdmin = this.authService.getRole() === 'Admin';
    this.loadTickets();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  loadTickets() {
    const url = 'http://localhost:5123/api/Tickets';
    const params = new HttpParams()
      .set('pageNumber', '1')
      .set('pageSize', '100');

    this.http.get<any>(url, { params }).subscribe({
      next: (data) => {
        const ticketsData = data.items ?? data;
        const filtered = this.isAdmin
          ? ticketsData
          : ticketsData.filter((ticket: any) => ticket.createdBy === this.authService.getUserId());

        this.dataSource.data = filtered;
        this.calculateStats(filtered);
      },
      error: (err) => {
        console.error('Error loading tickets:', err);
      }
    });
  }

  calculateStats(tickets: any[]) {
    this.stats.open = tickets.filter(t => t.status === 'Open').length;
    this.stats.inProgress = tickets.filter(t => t.status === 'InProgress').length;
    this.stats.resolved = tickets.filter(t => t.status === 'Resolved').length;
    this.stats.closed = tickets.filter(t => t.status === 'Closed').length;
  }

  updateStatus(ticketId: number, event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const statusString = selectElement.value as keyof typeof TicketStatus;
    const statusNumber = TicketStatus[statusString as keyof typeof TicketStatus] as unknown as number;

    console.log(`Updating ticket ${ticketId} to status ${statusNumber}`);

    this.authService.updateTicketStatus(ticketId, statusNumber).subscribe({
      next: () => {
        console.log('Status updated successfully');
        this.loadTickets();
      },
      error: err => console.error('Failed to update ticket status', err)
    });
  }

  submitTicket() {
    if (this.ticketForm.invalid) {
      this.ticketForm.markAllAsTouched();
      return;
    }
    const ticketData = {
      title: this.ticketForm.value.title,
      description: this.ticketForm.value.description,
      createdBy: this.authService.getUserId() ?? ''
    };

    this.authService.createTicket(ticketData).subscribe({
      next: () => {
        alert('Ticket created successfully!');
        this.showAddTicketModal = false;
        this.ticketForm.reset();
        this.loadTickets();
      },
      error: (error) => {
        console.error('Error creating ticket:', error);
        alert('Failed to create ticket.');
      }
    });
  }

  getTimeAgo(date: string): string {
    return formatDistanceToNow(new Date(date), { addSuffix: true });
  }
}

export enum TicketStatus {
  Open = 0,
  InProgress = 1,
  Resolved = 2,
  Closed = 3
}