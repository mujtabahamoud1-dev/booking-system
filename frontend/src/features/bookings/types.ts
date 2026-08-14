// Mirrors Features/Bookings/BookingDTOs.cs and BookingStatus.cs.
// Status crosses the wire as a camelCase string, matching the backend enum.
export type BookingStatus = "pending" | "confirmed" | "cancelled";

export interface Booking {
  id: number;
  userId: number;
  serviceId: number;
  slotId: number;
  bookingDate: string; // "yyyy-MM-dd"
  status: BookingStatus;
  notes: string | null;
  createdAt: string;
}

// Mirrors AdminBookingResponse: what GET /bookings returns to an admin. The
// patient is carried on the row so staff can find a booking by the name someone
// gives them on the phone, which a bare userId cannot answer.
export interface AdminBooking extends Booking {
  patientName: string;
  patientEmail: string;
  patientPhone: string | null;
}

// Mirrors AdminBookingQuery. `from`/`to` are inclusive "yyyy-MM-dd" bounds; the
// named ranges the UI offers are turned into dates before the request is made,
// so the API stays a plain date-range filter.
export interface AdminBookingQuery {
  search?: string;
  status?: BookingStatus | "all";
  from?: string;
  to?: string;
}

// Counts honour the search and date range but not the status filter, so the
// chips keep reporting the whole picture while one of them is selected.
export interface BookingStatusCounts {
  total: number;
  pending: number;
  confirmed: number;
  cancelled: number;
}

export interface AdminBookingListResponse {
  items: AdminBooking[];
  counts: BookingStatusCounts;
}

export interface CreateBookingRequest {
  serviceId: number;
  slotId: number;
  bookingDate: string;
  notes?: string | null;
}
