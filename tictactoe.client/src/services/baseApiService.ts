export abstract class BaseApiService {
  protected abstract readonly baseUrl: string;

  protected async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }
    
    const data = await response.json();
    
    // Convert date strings to Date objects
    if (data) {
      this.convertDates(data);
    }
    
    return data;
  }

  protected convertDates(obj: unknown): void {
    if (Array.isArray(obj)) {
      obj.forEach(item => this.convertDates(item));
      return;
    }
    
    if (obj && typeof obj === 'object') {
      for (const key in obj) {
        if (Object.prototype.hasOwnProperty.call(obj, key)) {
          const value = (obj as Record<string, unknown>)[key];
          if (typeof value === 'string' && this.isDateString(key)) {
            (obj as Record<string, unknown>)[key] = new Date(value);
          } else if (typeof value === 'object') {
            this.convertDates(value);
          }
        }
      }
    }
  }

  protected isDateString(key: string): boolean {
    const dateFields = ['createdAt', 'startedAt', 'endedAt', 'updatedAt'];
    return dateFields.includes(key);
  }

  protected async get<T>(url: string): Promise<T> {
    const response = await fetch(url);
    return this.handleResponse<T>(response);
  }

  protected async post<T>(url: string, data?: any): Promise<T> {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: data ? JSON.stringify(data) : undefined,
    });
    return this.handleResponse<T>(response);
  }

  protected async put<T>(url: string, data?: any): Promise<T> {
    const response = await fetch(url, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: data ? JSON.stringify(data) : undefined,
    });
    return this.handleResponse<T>(response);
  }

  protected async delete(url: string): Promise<void> {
    const response = await fetch(url, {
      method: 'DELETE',
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }
  }

  // Generic CRUD operations
  async getById<T>(id: number): Promise<T> {
    return this.get<T>(`${this.baseUrl}/${id}`);
  }

  async getAll<T>(): Promise<T[]> {
    return this.get<T[]>(this.baseUrl);
  }

  async create<T>(data: any): Promise<T> {
    return this.post<T>(this.baseUrl, data);
  }

  async updateById<T>(id: number, data: any): Promise<T> {
    return this.put<T>(`${this.baseUrl}/${id}`, data);
  }

  async deleteById(id: number): Promise<void> {
    return this.delete(`${this.baseUrl}/${id}`);
  }
} 