// English is the source of truth. Its shape is exported as `Messages`, and
// every other locale is typed against it — a missing or misspelled key is a
// compile error, not a silent fallback at runtime.
//
// Copy rules: sentence case, active voice, and an action keeps the same name
// through the whole flow. No key interpolates a bare count into a noun, so no
// string needs plural forms to stay grammatical.
const en = {
  common: {
    language: 'Language',
    english: 'English',
    arabic: 'العربية',

    brand: {
      name: 'Atlas',
      discipline: 'Physiotherapy',
      note: 'Appointments by booking only',
    },

    nav: {
      services: 'Services',
      dashboard: 'Dashboard',
      slots: 'Slots',
      bookings: 'Bookings',
      myBookings: 'My bookings',
      login: 'Sign in',
      register: 'Create account',
      logout: 'Sign out',
      admin: 'Admin',
    },

    actions: {
      save: 'Save',
      saveChanges: 'Save changes',
      create: 'Create',
      cancel: 'Cancel',
      edit: 'Edit',
      delete: 'Delete',
      close: 'Close',
      confirm: 'Confirm',
    },

    fields: {
      name: 'Name',
      description: 'Description',
      email: 'Email',
      password: 'Password',
      phone: 'Phone',
      date: 'Date',
      notes: 'Notes',
      status: 'Status',
      service: 'Service',
      duration: 'Duration',
      price: 'Price',
      user: 'Patient',
      optional: 'Optional',
      active: 'Active',
    },

    loading: 'Loading',
    minutesShort: '{count} min',
    minutesUnit: 'min',
    emptyValue: '—',

    days: {
      0: 'Sunday',
      1: 'Monday',
      2: 'Tuesday',
      3: 'Wednesday',
      4: 'Thursday',
      5: 'Friday',
      6: 'Saturday',
    },
  },

  auth: {
    signIn: 'Sign in',
    signInHelp: 'Sign in to book a session or check your appointments.',
    createAccount: 'Create account',
    createAccountHelp: 'You need an account to hold an appointment.',
    register: 'Create account',
    noAccount: 'New here?',
    haveAccount: 'Already have an account?',
    loginFailed: 'That email and password do not match an account.',
    registerFailed: 'Could not create the account. Check the details and try again.',
  },

  services: {
    eyebrow: 'Appointments',
    title: 'Book your next session',
    subtitle: 'Pick a service to see when it runs this week, then choose a time that fits.',
    sessionLength: 'Session length',
    empty: 'No services are open for booking yet.',
    noDescription: 'No description yet.',
    bookNow: 'Book',

    adminTitle: 'Services',
    adminSubtitle: 'What the clinic offers, how long each session runs, and what it costs.',
    newService: 'New service',
    editService: 'Edit service',
    adminEmpty: 'No services yet. Create the first one to start taking bookings.',
    durationField: 'Duration (min)',
    statusActive: 'Active',
    statusInactive: 'Inactive',
    activeHelp: 'Open for booking',

    confirmDelete: 'Delete "{name}"? This cannot be undone.',
    saveFailed: 'Could not save the service.',
    deleteFailed: 'Could not delete the service. Deactivate it instead if it has bookings.',
  },

  slots: {
    title: 'Slots',
    subtitle: 'The hours each service is available, set once and repeated every week.',
    newSlot: 'New slot',
    editSlot: 'Edit slot',
    empty: 'This service has no slots, so it cannot be booked yet.',
    noServices: 'No services — create one first',

    weekCover: 'Weekly cover',
    weeklyHours: '{hours} h per week',
    selectHint: 'Select a slot to edit or delete it.',
    capacityOf: 'Capacity {count}',
    dayClosed: 'Closed',

    dayOfWeek: 'Day of week',
    startTime: 'Start time',
    endTime: 'End time',
    maxBookings: 'Max bookings',

    confirmDelete: 'Delete this slot?',
    saveFailed: 'Could not save the slot.',
    deleteFailed: 'Could not delete the slot. It already has bookings.',
  },

  bookings: {
    backToServices: 'All services',
    stepTime: 'Choose a time',
    stepTimeHelp: 'These are the hours this service runs each week.',
    stepDate: 'Choose a date',
    stepDateHelp: 'This slot runs on {day}. Pick one of the next few, or set another date.',
    stepNotes: 'Anything we should know',
    stepNotesHelp: 'Injuries, recent surgery, or what you want to work on.',
    notesPlaceholder: 'Optional',
    otherDate: 'Pick another date',
    summaryLead: 'Booking',

    chooseSlot: 'Available slots',
    noSlots: 'This service has no available slots yet.',
    confirmBooking: 'Confirm booking',
    serviceNotFound: 'That service does not exist.',
    loadFailed: 'Could not load this service.',
    createFailed: 'Could not create the booking.',
    slotRequired: 'Choose a slot first.',
    dayMismatch: 'That date does not fall on the slot’s day of week.',
    pickDay: 'Pick a {day}.',

    mineTitle: 'My bookings',
    mineSubtitle: 'Your appointments, soonest first.',
    mineEmpty: 'You have no appointments booked.',
    browseServices: 'Browse services',
    allTitle: 'Bookings',
    allSubtitle: 'Every appointment across the clinic.',
    allEmpty: 'No bookings yet.',
    filterAll: 'All',
    filterEmpty: 'No bookings with this status.',
    unnamedService: 'Service #{id}',

    confirmCancel: 'Cancel this booking?',
    cancelFailed: 'Could not cancel the booking.',
    confirmFailed: 'Could not confirm the booking.',

    status: {
      pending: 'Pending',
      confirmed: 'Confirmed',
      cancelled: 'Cancelled',
    },
  },

  admin: {
    welcome: 'Welcome back, {name}',
    manage: 'Manage',

    pendingLead: 'Waiting for confirmation',
    pendingAction: 'Review bookings',
    pendingClear: 'Nothing is waiting for confirmation.',

    metrics: {
      services: 'Services',
      active: 'Active',
      total: 'Bookings',
      confirmed: 'Confirmed',
      cancelled: 'Cancelled',
    },

    shortcuts: {
      services: 'Services',
      servicesHint: 'Add a service, or change its length and price',
      slots: 'Slots',
      slotsHint: 'Set the hours each service runs',
      bookings: 'Bookings',
      bookingsHint: 'Confirm or cancel appointments',
    },
  },
}

export default en

// Shape every other locale must satisfy exactly.
export type Messages = typeof en
