<script setup lang="ts">
import { computed, onMounted } from "vue";
import { RouterLink } from "vue-router";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useServicesStore } from "../store";
import { useAuthStore } from "@/features/auth/store";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";

const store = useServicesStore();
const auth = useAuthStore();
const { t } = useI18n();
const { items, loading } = storeToRefs(store);

onMounted(() => store.load());

// Session lengths are drawn to scale against the longest one on offer, so a
// 30-minute assessment and a 60-minute rehab session are comparable at a glance
// instead of being two numbers you have to hold in your head.
const longest = computed(() => Math.max(...items.value.map((s) => s.duration), 1));

const range = computed(() => {
  if (items.value.length === 0) return null;
  const durations = items.value.map((s) => s.duration);
  return { min: Math.min(...durations), max: Math.max(...durations) };
});
</script>

<template>
  <section>
    <header class="mb-9 max-w-2xl sm:mb-12">
      <p class="u-label mb-4 text-brand">{{ t("services.eyebrow") }}</p>
      <h1 class="u-display text-4xl text-balance md:text-5xl">{{ t("services.title") }}</h1>
      <p class="mt-4 text-base text-ink-soft sm:text-lg">{{ t("services.subtitle") }}</p>

      <p v-if="range" class="mt-6 flex items-baseline gap-3 text-xs">
        <span class="u-label text-ink-faint">{{ t("services.sessionLength") }}</span>
        <span class="u-data text-ink-soft">
          <span dir="ltr" class="inline-block">{{ range.min }}–{{ range.max }}</span>
          {{ t("common.minutesUnit") }}
        </span>
      </p>
    </header>

    <LoadingSpinner v-if="loading" />

    <p
      v-else-if="items.length === 0"
      class="border border-dashed border-line px-6 py-16 text-center text-sm text-ink-faint"
    >
      {{ t("services.empty") }}
    </p>

    <!-- Borders live on the cards, not as grid gaps: a part-filled last row then
         leaves empty space rather than a block of gap colour. -->
    <div v-else class="grid border-s border-t border-line sm:grid-cols-2 lg:grid-cols-3">
      <article
        v-for="(service, i) in items"
        :key="service.id"
        class="u-rise flex flex-col border-e border-b border-line bg-surface p-5 sm:p-6"
        :style="{ '--i': i }"
      >
        <h2 class="u-display text-xl">{{ service.name }}</h2>
        <p class="mt-2 flex-1 text-sm leading-relaxed text-ink-soft">
          {{ service.description || t("services.noDescription") }}
        </p>

        <!-- Duration as a measured bar, same visual language as the week track. -->
        <div class="mt-6">
          <div class="mb-1.5 flex items-baseline justify-between">
            <span class="u-label text-ink-faint">{{ t("common.fields.duration") }}</span>
            <span class="u-data text-sm font-medium">
              {{ t("common.minutesShort", { count: service.duration }) }}
            </span>
          </div>
          <div dir="ltr" class="h-1.5 w-full bg-ground">
            <div
              class="u-extend h-full bg-brand"
              :style="{ width: `${(service.duration / longest) * 100}%`, '--i': i }"
            />
          </div>
        </div>

        <div class="mt-5 flex items-center justify-between border-t border-line pt-5">
          <span dir="ltr" class="u-data inline-block text-base font-medium">
            ${{ service.price.toFixed(2) }}
          </span>
          <RouterLink
            v-if="!auth.isAdmin"
            :to="`/book/${service.id}`"
            class="u-action u-label rounded-sm bg-brand px-4 py-2.5 text-surface transition-colors hover:bg-brand-deep"
          >
            {{ t("services.bookNow") }}
          </RouterLink>
        </div>
      </article>
    </div>
  </section>
</template>
