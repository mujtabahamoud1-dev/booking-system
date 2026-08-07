import { shallowRef } from 'vue'

// A promise-based stand-in for window.confirm, so a caller keeps reading as a
// straight line — `if (!(await confirm(...))) return` — while the question is
// asked by our own dialog instead of the browser's.
export interface ConfirmOptions {
  /** The question, already translated. This layer does no i18n of its own. */
  message: string
  title?: string
  /**
   * Label of the button that goes ahead. Name it after the action that opened
   * the dialog — Delete stays Delete all the way through the flow.
   */
  confirmLabel?: string
  dismissLabel?: string
  variant?: 'primary' | 'danger'
}

interface PendingRequest extends ConfirmOptions {
  settle: (answer: boolean) => void
}

// Module-level: every caller shares the one dialog App.vue mounts, so no view
// has to host markup just to ask a question. Only one can be open at a time,
// which is what window.confirm did anyway.
const request = shallowRef<PendingRequest | null>(null)

export function useConfirm() {
  function confirm(options: ConfirmOptions | string): Promise<boolean> {
    // A second request supersedes the first, which settles as "no" rather than
    // leaving its caller awaiting a promise nothing will ever resolve.
    request.value?.settle(false)

    const opts = typeof options === 'string' ? { message: options } : options

    return new Promise<boolean>((resolve) => {
      request.value = {
        ...opts,
        settle: (answer) => {
          request.value = null
          resolve(answer)
        },
      }
    })
  }

  return { confirm }
}

/** For ConfirmDialog only — the single component that renders the request. */
export function useConfirmHost() {
  return { request }
}
