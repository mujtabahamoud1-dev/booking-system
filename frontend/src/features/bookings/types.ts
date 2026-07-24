// Mirrors Features/Bookings/BookingDTOs.cs and BookingStatus.cs.
// Status crosses the wire as a camelCase string, matching the backend enum.
export type BookingStatus = 'pending' | 'confirmed' | 'cancelled'

export interface Booking {
  id: number
  userId: number
  serviceId: number
  slotId: number
  bookingDate: string // "yyyy-MM-dd"
  status: BookingStatus
  notes: string | null
  createdAt: string
}

export interface CreateBookingRequest {
  serviceId: number
  slotId: number
  bookingDate: string
  notes?: string | null
}
