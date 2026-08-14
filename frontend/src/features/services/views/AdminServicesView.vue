<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useServicesStore } from "../store";
import type { Service, UpdateServiceRequest } from "../types";
import { apiErrorMessage } from "@/shared/api/client";
import { useConfirm } from "@/shared/composables/useConfirm";
import ServiceForm from "../components/ServiceForm.vue";
import BaseButton from "@/shared/components/BaseButton.vue";
import BaseModal from "@/shared/components/BaseModal.vue";
import AlertMessage from "@/shared/components/AlertMessage.vue";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";
import SearchInput from "@/shared/components/SearchInput.vue";
import FilterChips from "@/shared/components/FilterChips.vue";

const store = useServicesStore();
const { adminItems: items, adminCounts: counts, adminLoaded, loading } = storeToRefs(store);
const { t } = useI18n();
const { confirm } = useConfirm();

const showForm = ref(false);
const editing = ref<Service | null>(null);
const submitting = ref(false);
const error = ref<string | null>(null);

const search = ref("");
const status = ref<"all" | "active" | "inactive">("all");

watch([search, status], () => store.search({ search: search.value, status: status.value }), {
  immediate: true,
});

const statusOptions = computed(() => [
  { value: "all", label: t("services.filterAll"), count: counts.value.total },
  {
    value: "active",
    label: t("services.statusActive"),
    count: counts.value.active,
  },
  {
    value: "inactive",
    label: t("services.statusInactive"),
    count: counts.value.inactive,
  },
]);

const hasFilters = computed(() => search.value.trim() !== "" || status.value !== "all");
const emptyMessage = computed(() =>
  hasFilters.value ? t("services.filterEmpty") : t("services.adminEmpty"),
);

function openCreate(): void {
  editing.value = null;
  error.value = null;
  showForm.value = true;
}

function openEdit(service: Service): void {
  editing.value = service;
  error.value = null;
  showForm.value = true;
}

async function save(payload: UpdateServiceRequest): Promise<void> {
  submitting.value = true;
  error.value = null;
  try {
    if (editing.value) {
      await store.update(editing.value.id, payload);
    } else {
      await store.create({
        name: payload.name,
        description: payload.description,
        duration: payload.duration,
        price: payload.price,
      });
    }
    showForm.value = false;
  } catch (e) {
    error.value = apiErrorMessage(e, t("services.saveFailed"));
  } finally {
    submitting.value = false;
  }
}

async function remove(service: Service): Promise<void> {
  const ok = await confirm({
    message: t("services.confirmDelete", { name: service.name }),
    confirmLabel: t("common.actions.delete"),
  });
  if (!ok) return;

  error.value = null;
  try {
    await store.remove(service.id);
  } catch (e) {
    // 409 when the service has bookings — the API asks you to deactivate instead.
    error.value = apiErrorMessage(e, t("services.deleteFailed"));
  }
}
</script>

<template>
  <section>
    <header class="mb-8 flex flex-wrap items-end justify-between gap-5">
      <div>
        <h1 class="u-display text-3xl text-balance md:text-4xl">
          {{ t("services.adminTitle") }}
        </h1>
        <p class="mt-3 text-ink-soft">{{ t("services.adminSubtitle") }}</p>
      </div>
      <BaseButton class="w-full sm:w-auto" @click="openCreate">
        {{ t("services.newService") }}
      </BaseButton>
    </header>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <div v-if="adminLoaded" class="mb-6 space-y-4">
      <div class="sm:max-w-sm">
        <SearchInput
          v-model="search"
          :label="t('services.searchLabel')"
          :placeholder="t('services.searchPlaceholder')"
          :debounce="300"
          :result-label="
            t('common.search.results', {
              count: items.length,
              total: counts.total,
            })
          "
        />
      </div>
      <FilterChips
        v-model="status"
        :options="statusOptions"
        :group-label="t('common.filters.status')"
      />
    </div>

    <!-- Only the first load takes the page away. Later queries leave the table
         in place and dim it, so typing does not make the screen flicker. -->
    <LoadingSpinner v-if="!adminLoaded" />

    <template v-else>
      <!-- The desktop table becomes a record list on a phone rather than a
         sideways scroll: name, what it costs you in time and money, and the two
         actions, all reachable without moving the page. -->
      <ul
        class="border-t border-line transition-opacity md:hidden"
        :class="loading ? 'opacity-60' : ''"
        :aria-busy="loading"
      >
        <li v-for="service in items" :key="service.id" class="border-b border-line py-4">
          <div class="flex items-start justify-between gap-3">
            <p class="min-w-0 font-medium">{{ service.name }}</p>
            <span
              class="u-label inline-flex shrink-0 items-center gap-1.5 rounded-sm px-2 py-1"
              :class="service.isActive ? 'bg-brand-soft text-brand' : 'bg-ground text-ink-faint'"
            >
              <span
                class="h-1.5 w-1.5 rounded-full"
                :class="service.isActive ? 'bg-brand' : 'bg-ink-faint'"
                aria-hidden="true"
              />
              {{ service.isActive ? t("services.statusActive") : t("services.statusInactive") }}
            </span>
          </div>

          <p v-if="service.description" class="mt-1 text-sm text-ink-soft">
            {{ service.description }}
          </p>

          <p class="u-data mt-2 text-sm text-ink-soft">
            {{ t("common.minutesShort", { count: service.duration }) }}
            <span class="mx-2 text-line" aria-hidden="true">/</span>
            <span dir="ltr" class="inline-block">${{ service.price.toFixed(2) }}</span>
          </p>

          <div class="mt-1 flex items-center gap-5">
            <button class="u-action u-label text-ink-soft" @click="openEdit(service)">
              {{ t("common.actions.edit") }}
            </button>
            <button class="u-action u-label text-alert" @click="remove(service)">
              {{ t("common.actions.delete") }}
            </button>
          </div>
        </li>
        <li
          v-if="items.length === 0"
          class="border-b border-line py-14 text-center text-sm text-ink-faint"
        >
          {{ emptyMessage }}
        </li>
      </ul>

      <div
        class="hidden border border-line bg-surface transition-opacity md:block"
        :class="loading ? 'opacity-60' : ''"
        :aria-busy="loading"
      >
        <table class="min-w-full text-sm">
          <thead>
            <tr class="border-b border-line">
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.name") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.duration") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.price") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.status") }}
              </th>
              <th class="px-5 py-4">
                <span class="sr-only">{{ t("common.actions.edit") }}</span>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="service in items"
              :key="service.id"
              class="border-b border-line last:border-0"
            >
              <td class="px-5 py-4">
                <div class="font-medium">{{ service.name }}</div>
                <div class="mt-0.5 text-ink-faint">
                  {{ service.description || t("common.emptyValue") }}
                </div>
              </td>
              <td class="u-data px-5 py-4 text-ink-soft">
                {{ t("common.minutesShort", { count: service.duration }) }}
              </td>
              <td class="u-data px-5 py-4 text-ink-soft">
                <span dir="ltr" class="inline-block">${{ service.price.toFixed(2) }}</span>
              </td>
              <td class="px-5 py-4">
                <span
                  class="u-label inline-flex items-center gap-1.5 rounded-sm px-2 py-1"
                  :class="
                    service.isActive ? 'bg-brand-soft text-brand' : 'bg-ground text-ink-faint'
                  "
                >
                  <span
                    class="h-1.5 w-1.5 rounded-full"
                    :class="service.isActive ? 'bg-brand' : 'bg-ink-faint'"
                    aria-hidden="true"
                  />
                  {{ service.isActive ? t("services.statusActive") : t("services.statusInactive") }}
                </span>
              </td>
              <td class="px-5 py-4 text-end whitespace-nowrap">
                <button
                  class="u-action u-label text-ink-soft transition-colors hover:text-ink"
                  @click="openEdit(service)"
                >
                  {{ t("common.actions.edit") }}
                </button>
                <button
                  class="u-action u-label ms-4 text-alert transition-opacity hover:opacity-70"
                  @click="remove(service)"
                >
                  {{ t("common.actions.delete") }}
                </button>
              </td>
            </tr>
            <tr v-if="items.length === 0">
              <td colspan="5" class="px-5 py-16 text-center text-ink-faint">
                {{ emptyMessage }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <BaseModal
      v-if="showForm"
      :title="editing ? t('services.editService') : t('services.newService')"
      @close="showForm = false"
    >
      <ServiceForm
        :service="editing"
        :submitting="submitting"
        @save="save"
        @cancel="showForm = false"
      />
    </BaseModal>
  </section>
</template>
