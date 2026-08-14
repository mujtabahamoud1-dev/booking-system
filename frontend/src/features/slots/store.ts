import { ref } from "vue";
import { defineStore } from "pinia";
import { slotsApi } from "./api";
import type { CreateSlotRequest, Slot, SlotCounts, SlotQuery, UpdateSlotRequest } from "./types";

// Two views of the same table. A patient browsing one service reads the cache
// keyed by service id; the admin week is query-driven and filtered by the
// server, because it can span every service at once.
export const useSlotsStore = defineStore("slots", () => {
  const byService = ref<Record<number, Slot[]>>({});
  const loading = ref(false);

  const adminItems = ref<Slot[]>([]);
  const adminCounts = ref<SlotCounts>({ total: 0, byDay: [0, 0, 0, 0, 0, 0, 0] });
  const adminLoaded = ref(false);
  const adminQuery = ref<SlotQuery>({});

  // Only the newest request is allowed to land, so quickly changing the service
  // or day cannot leave an earlier response on screen.
  let latest = 0;

  function forService(serviceId: number): Slot[] {
    return byService.value[serviceId] ?? [];
  }

  async function load(serviceId: number): Promise<void> {
    loading.value = true;
    try {
      byService.value = {
        ...byService.value,
        [serviceId]: await slotsApi.getForService(serviceId),
      };
    } finally {
      loading.value = false;
    }
  }

  async function search(query: SlotQuery): Promise<void> {
    adminQuery.value = query;
    const token = ++latest;
    loading.value = true;
    try {
      const response = await slotsApi.search(query);
      if (token !== latest) return;
      adminItems.value = response.items;
      adminCounts.value = response.counts;
      adminLoaded.value = true;
    } finally {
      if (token === latest) loading.value = false;
    }
  }

  // The admin list belongs to the server, so a write is followed by a re-read
  // rather than by patching the array — a slot moved to another day may no
  // longer match the day filter that is on screen.
  const refresh = (): Promise<void> => search(adminQuery.value);

  async function create(req: CreateSlotRequest): Promise<Slot> {
    const created = await slotsApi.create(req);
    const existing = byService.value[req.serviceId] ?? [];
    byService.value = { ...byService.value, [req.serviceId]: [...existing, created] };
    await refresh();
    return created;
  }

  async function update(id: number, serviceId: number, req: UpdateSlotRequest): Promise<Slot> {
    const updated = await slotsApi.update(id, req);
    const existing = byService.value[serviceId] ?? [];
    byService.value = {
      ...byService.value,
      [serviceId]: existing.map((s) => (s.id === id ? updated : s)),
    };
    await refresh();
    return updated;
  }

  async function remove(id: number, serviceId: number): Promise<void> {
    await slotsApi.remove(id);
    const existing = byService.value[serviceId] ?? [];
    byService.value = { ...byService.value, [serviceId]: existing.filter((s) => s.id !== id) };
    await refresh();
  }

  return {
    byService,
    loading,
    adminItems,
    adminCounts,
    adminLoaded,
    forService,
    load,
    search,
    refresh,
    create,
    update,
    remove,
  };
});
