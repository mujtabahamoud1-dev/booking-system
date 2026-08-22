import { useI18n } from "vue-i18n";
import type { Service } from "./types";

// Arabic is optional; an untranslated service falls back to its English copy.
export function serviceName(service: Service, locale: string): string {
  return (isArabic(locale) ? service.nameAr : service.name) || service.name;
}

export function serviceDescription(service: Service, locale: string): string | null {
  const preferred = isArabic(locale) ? service.descriptionAr : service.description;
  return preferred || service.description || null;
}

const isArabic = (locale: string): boolean => locale.startsWith("ar");

// The same two, bound to the active locale, for templates.
export function useServiceLabels() {
  const { locale } = useI18n();
  return {
    serviceName: (service: Service) => serviceName(service, locale.value),
    serviceDescription: (service: Service) => serviceDescription(service, locale.value),
  };
}
