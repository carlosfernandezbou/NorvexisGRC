import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LoginService {
    private http = inject(HttpClient);

    private baseUrl = 'http://localhost:5000/api';

    getUsers(): Observable<any[]> {
        return this.http.get<any[]>(`${this.baseUrl}/users`);
    }
    login(email: string, password: string) {
        return this.http.post<any>(`${this.baseUrl}/auth/login`, {
            email,
            password
        });
    }

    saveToken(token: string, expiresInMinutes: number): void {
        const expiresAt = Date.now() + expiresInMinutes * 60 * 1000;

        localStorage.setItem('accessToken', token);
        localStorage.setItem('expiresAt', expiresAt.toString());
    }

    getToken(): string | null {
        return localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    }

    saveUserId(userId: string): void {
        localStorage.setItem('userId', userId);
    }

    getUserRoles(userId: string): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/users/${userId}/roles`);
    }

    saveUserRole(role: string): void {
        localStorage.setItem('userRole', role);
    }

    getUserRole(): string | null {
        return localStorage.getItem('userRole');
    }

    isLoggedIn(): boolean {
        const token = localStorage.getItem('accessToken');
        const expiresAt = localStorage.getItem('expiresAt');

        if (!token || !expiresAt) {
            return false;
        }

        const isExpired = Date.now() > Number(expiresAt);

        if (isExpired) {
            this.logout();
            return false;
        }

        return true;
    }

    logout(): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/auth/logout`, {});
    }

    clearSession(): void {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('expiresAt');
        localStorage.removeItem('Id');
        localStorage.removeItem('userId');
        localStorage.removeItem('userRole');

        sessionStorage.removeItem('accessToken');
    }

    // logout(): void {
    //     localStorage.removeItem('accessToken');
    //     localStorage.removeItem('expiresAt');
    //     localStorage.removeItem('Id');
    //     localStorage.removeItem('userRole');
    // }
}
