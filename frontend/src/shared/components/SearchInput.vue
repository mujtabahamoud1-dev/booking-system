<script setup lang="ts">
import { nextTick, onMounted, onUnmounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

// One search field for every admin list, so the three pages read as one system
// rather than three separately-invented boxes.
//
// The model updates on a short debounce: filtering runs over data already in
// memory, so it is fast, but re-rendering a table on every keystroke still
// makes typing feel heavy on a long list.
const props = withDefaults(
  defineProps<{
    label: string;
    placeholder?: string;
    // Announced after the list re-renders, so a screen reader learns how many
    // rows survived the filter instead of being told only that text changed.
    resultLabel?: string;
    debounce?: number;
  }>(),
  { placeholder: undefined, resultLabel: undefined, debounce: 150 },
);

const model = defineModel<string>({ default: "" });

const { t } = useI18n();

const input = ref<HTMLInputElement | null>(null);
// The field the user types into. Kept apart from `model` so the debounce never
// makes the input itself feel laggy.
const draft = ref(model.value);
let timer: ReturnType<typeof setTimeout> | undefined;

watch(draft, (value) => {
  clearTimeout(timer);
  timer = setTimeout(() => {
    model.value = value;
  }, props.debounce);
});

// Reset from the outside (a "clear all filters" button) has to reach the input.
watch(model, (value) => {
  if (value !== draft.value) draft.value = value;
});

function clear(): void {
  clearTimeout(timer);
  draft.value = "";
  model.value = "";
  nextTick(() => input.value?.focus());
}

// `/` jumps to search the way it does in most tools an admin already uses,
// but not while they are typing into some other field.
function onKeydown(event: KeyboardEvent): void {
  if (event.key !== "/" || event.metaKey || event.ctrlKey || event.altKey) return;

  const target = event.target as HTMLElement | null;
  const tag = target?.tagName;
  if (tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT" || target?.isContentEditable)
    return;

  event.preventDefault();
  input.value?.focus();
}

onMounted(() => window.addEventListener("keydown", onKeydown));
onUnmounted(() => {
  window.removeEventListener("keydown", onKeydown);
  clearTimeout(timer);
});
</script>

<template>
  <div>
    <label class="block">
      <span class="u-label mb-1.5 block text-ink-soft">{{ label }}</span>
      <div class="relative">
        <!-- Icon and clear button sit on the logical start/end, so the field
             mirrors correctly in Arabic without a second set of rules. -->
        <svg
          class="pointer-events-none absolute inset-y-0 start-3 my-auto h-4 w-4 text-ink-faint"
          viewBox="0 0 16 16"
          fill="none"
          aria-hidden="true"
        >
          <circle cx="7" cy="7" r="4.5" stroke="currentColor" stroke-width="1.5" />
          <path
            d="M10.5 10.5L14 14"
            stroke="currentColor"
            stroke-width="1.5"
            stroke-linecap="round"
          />
        </svg>

        <input
          ref="input"
          v-model="draft"
          type="search"
          :placeholder="placeholder"
          class="block w-full rounded-sm border-0 bg-surface py-3 pe-9 ps-9 text-base text-ink ring-1 ring-inset ring-line transition-shadow placeholder:text-ink-faint focus:ring-2 focus:ring-inset focus:ring-brand sm:py-2.5 sm:text-sm [&::-webkit-search-cancel-button]:hidden"
          @keydown.escape="clear"
        />

        <button
          v-if="draft"
          type="button"
          class="absolute inset-y-0 end-0 flex w-9 items-center justify-center text-ink-faint transition-colors hover:text-ink"
          :aria-label="t('common.search.clear')"
          @click="clear"
        >
          <svg class="h-3.5 w-3.5" viewBox="0 0 12 12" fill="none" aria-hidden="true">
            <path
              d="M3 3l6 6M9 3l-6 6"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
            />
          </svg>
        </button>
      </div>
    </label>

    <p v-if="resultLabel" class="sr-only" role="status" aria-live="polite">{{ resultLabel }}</p>
  </div>
</template>
