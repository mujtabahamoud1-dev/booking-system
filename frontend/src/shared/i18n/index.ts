import { createI18n } from "vue-i18n";
import en from "./locales/en";
import ar from "./locales/ar";

export type Locale = "en" | "ar";

export const LOCALES: Locale[] = ["en", "ar"];

// Locales written right-to-left. Drives <html dir> and the `rtl:` Tailwind variant.
const RTL_LOCALES: Locale[] = ["ar"];

const STORAGE_KEY = "booking.locale";

function detect(): Locale {
  const stored = localStorage.getItem(STORAGE_KEY) as Locale | null;
  if (stored && LOCALES.includes(stored)) return stored;
  return navigator.language.startsWith("ar") ? "ar" : "en";
}

export const i18n = createI18n({
  legacy: false,
  locale: detect(),
  fallbackLocale: "en",
  messages: { en, ar },
});

export const isRtl = (locale: Locale): boolean => RTL_LOCALES.includes(locale);

export function currentLocale(): Locale {
  return i18n.global.locale.value as Locale;
}

// Keeps <html lang>/<html dir> in step with the active locale so the browser
// mirrors the layout and Tailwind's rtl: variants apply.
export function applyDocumentLocale(locale: Locale): void {
  document.documentElement.lang = locale;
  document.documentElement.dir = isRtl(locale) ? "rtl" : "ltr";
}

export function setLocale(locale: Locale): void {
  i18n.global.locale.value = locale;
  localStorage.setItem(STORAGE_KEY, locale);
  applyDocumentLocale(locale);
}

// Called once at startup to align the document with the detected locale.
export function initLocale(): void {
  applyDocumentLocale(currentLocale());
}
