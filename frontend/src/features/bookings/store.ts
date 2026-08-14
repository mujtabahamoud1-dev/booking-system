import { ref } from "vue";
import { defineStore } from "pinia";
import { bookingsApi } from "./api";
import type {
  AdminBooking,
  AdminBookingQuery,
  Booking,
  BookingStatusCounts,
  CreateBookingRequest,
} from "./types";

// One store serves both scopes, but they are kept in separate lists because the
// shapes differ: the admin queue carries the patient on each row and is filtered
// by the server, a client's own bookings are neither.
export const useBookingsStore = defineStore("bookings", () => {
  const items = ref<Booking[]>([]);
  const loading = ref(false);

  const adminItems = ref<AdminBooking[]>([]);
  const adminCounts = ref<BookingStatusCounts>({
    total: 0,
    pending: 0,
    confirmed: 0,
    cancelled: 0,
  });
  const adminLoaded = ref(false);
  const adminQuery = ref<AdminBookingQuery>({});

  // Typing races the network: a slow response for "sar" must not overwrite the
  // finished one for "sarah". Only the newest request is allowed to land.
  let latest = 0;

  async function loadMine(): Promise<void> {
    loading.value = true;
    try {
      items.value = await bookingsApi.getMine();
    } finally {
      loading.value = false;
    }
  }

  async function loadAll(query: AdminBookingQuery = {}): Promise<void> {
    adminQuery.value = query;
    const token = ++latest;
    loading.value = true;
    try {
      const response = await bookingsApi.getAll(query);
      if (token !== latest) return;
      adminItems.value = response.items;
      adminCounts.value = response.counts;
      adminLoaded.value = true;
    } finally {
      if (token === latest) loading.value = false;
    }
  }

  async function create(req: CreateBookingRequest): Promise<Booking> {
    const created = await bookingsApi.create(req);
    items.value = [created, ...items.value];
    return created;
  }

  // A client's own list is patched in place. The admin queue is re-read instead:
  // confirming a booking can move it out of the status filter that is on screen,
  // and the counts on the chips have to change with it.
  function replace(updated: Booking): void {
    items.value = items.value.map((b) => (b.id === updated.id ? updated : b));
  }

  const refresh = (): Promise<void> => loadAll(adminQuery.value);

  async function cancel(id: number, asAdmin = false): Promise<void> {
    replace(await bookingsApi.cancel(id));
    if (asAdmin) await refresh();
  }

  async function confirm(id: number): Promise<void> {
    replace(await bookingsApi.confirm(id));
    await refresh();
  }

  return {
    items,
    adminItems,
    adminCounts,
    adminLoaded,
    loading,
    loadMine,
    loadAll,
    refresh,
    create,
    cancel,
    confirm,
  };
});
