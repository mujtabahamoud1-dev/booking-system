// Mirrors Features/Availability/SlotDTOs.cs.
// TimeOnly crosses the wire as "HH:mm:ss"; kept as a string here.
export interface Slot {
  id: number
  serviceId: number
  dayOfWeek: number // 0 = Sunday .. 6 = Saturday
  startTime: string
  endTime: string
  maxBookings: number
}

export interface CreateSlotRequest {
  serviceId: number
  dayOfWeek: number
  startTime: string
  endTime: string
  maxBookings: number
}

// ServiceId is intentionally omitted: a slot cannot be moved to another service.
export interface UpdateSlotRequest {
  dayOfWeek: number
  startTime: string
  endTime: string
  maxBookings: number
}

// Weekday indexes as the backend numbers them (0 = Sunday .. 6 = Saturday).
// The labels are localised — look them up with `common.days.<index>`.
export const DAY_INDEXES = [0, 1, 2, 3, 4, 5, 6] as const

// "09:00:00" -> "09:00"
export const shortTime = (t: string): string => t.slice(0, 5)
