// Mirrors Features/Services/ServiceDTOs.cs.
export interface Service {
  id: number
  name: string
  description: string | null
  duration: number // minutes
  price: number
  isActive: boolean
}

export interface CreateServiceRequest {
  name: string
  description: string | null
  duration: number
  price: number
}

export interface UpdateServiceRequest {
  name: string
  description: string | null
  duration: number
  price: number
  isActive: boolean
}
