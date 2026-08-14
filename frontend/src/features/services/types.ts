// Mirrors Features/Services/ServiceDTOs.cs.
export interface Service {
  id: number;
  name: string;
  description: string | null;
  duration: number; // minutes
  price: number;
  isActive: boolean;
}

// Mirrors ServiceQuery: the admin list's filters, applied by the database.
export interface ServiceQuery {
  search?: string;
  status?: "all" | "active" | "inactive";
}

// Counts honour the search but not the status filter, so the chips can say how
// many are hiding on the other side of it.
export interface ServiceCounts {
  total: number;
  active: number;
  inactive: number;
}

export interface ServiceListResponse {
  items: Service[];
  counts: ServiceCounts;
}

export interface CreateServiceRequest {
  name: string;
  description: string | null;
  duration: number;
  price: number;
}

export interface UpdateServiceRequest {
  name: string;
  description: string | null;
  duration: number;
  price: number;
  isActive: boolean;
}
