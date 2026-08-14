import { ref } from "vue";
import { defineStore } from "pinia";
import { servicesApi } from "./api";
import type {
  CreateServiceRequest,
  Service,
  ServiceCounts,
  ServiceQuery,
  UpdateServiceRequest,
} from "./types";

export const useServicesStore = defineStore("services", () => {
  const items = ref<Service[]>([]);
  const loading = ref(false);

  // The admin list is a separate, query-driven view of the same table: the
  // server decides what it holds, so it is never filtered again on the client.
  const adminItems = ref<Service[]>([]);
  const adminCounts = ref<ServiceCounts>({ total: 0, active: 0, inactive: 0 });
  const adminLoaded = ref(false);
  const adminQuery = ref<ServiceQuery>({});

  // Typing races the network: a slow response for "phy" must not overwrite the
  // finished one for "physio". Only the newest request is allowed to land.
  let latest = 0;

  async function load(includeInactive = false): Promise<void> {
    loading.value = true;
    try {
      items.value = await servicesApi.getAll(includeInactive);
    } finally {
      loading.value = false;
    }
  }

  async function search(query: ServiceQuery): Promise<void> {
    adminQuery.value = query;
    const token = ++latest;
    loading.value = true;
    try {
      const response = await servicesApi.search(query);
      if (token !== latest) return;
      adminItems.value = response.items;
      adminCounts.value = response.counts;
      adminLoaded.value = true;
    } finally {
      if (token === latest) loading.value = false;
    }
  }

  // The list belongs to the server now, so a write is followed by a re-read
  // rather than by patching the array — a renamed service may no longer match
  // the search that is on screen.
  const refresh = (): Promise<void> => search(adminQuery.value);

  async function create(req: CreateServiceRequest): Promise<Service> {
    const created = await servicesApi.create(req);
    await refresh();
    return created;
  }

  async function update(id: number, req: UpdateServiceRequest): Promise<Service> {
    const updated = await servicesApi.update(id, req);
    items.value = items.value.map((s) => (s.id === id ? updated : s));
    await refresh();
    return updated;
  }

  async function remove(id: number): Promise<void> {
    await servicesApi.remove(id);
    items.value = items.value.filter((s) => s.id !== id);
    await refresh();
  }

  return {
    items,
    loading,
    adminItems,
    adminCounts,
    adminLoaded,
    load,
    search,
    refresh,
    create,
    update,
    remove,
  };
});
