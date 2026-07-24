// English is the source of truth. Its shape is exported as `Messages`, and
// every other locale is typed against it — a missing or misspelled key is a
// compile error, not a silent fallback at runtime.
const en = {
  common: {
    appName: 'Booking System',
    language: 'Language',
    english: 'English',
    arabic: 'العربية',

    nav: {
      services: 'Services',
      dashboard: 'Dashboard',
      slots: 'Slots',
      bookings: 'Bookings',
      myBookings: 'My Bookings',
      login: 'Login',
      register: 'Register',
      logout: 'Logout',
      admin: 'admin',
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
      user: 'User',
      optional: 'Optional',
      active: 'Active',
    },

    loading: 'Loading…',
    minutesShort: '{count} min',
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
    createAccount: 'Create account',
    register: 'Register',
    noAccount: 'No account?',
    haveAccount: 'Already have an account?',
    loginFailed: 'Login failed.',
    registerFailed: 'Registration failed.',
  },

  services: {
    title: 'Our services',
    subtitle: 'Choose a service to book an appointment.',
    empty: 'No services are available yet.',
    noDescription: 'No description.',
    bookNow: 'Book now',

    adminTitle: 'Services',
    newService: 'New service',
    editService: 'Edit service',
    adminEmpty: 'No services yet.',
    durationField: 'Duration (min)',
    statusActive: 'active',
    statusInactive: 'inactive',

    confirmDelete: 'Delete "{name}"? This cannot be undone.',
    saveFailed: 'Could not save the service.',
    deleteFailed: 'Could not delete the service.',
  },

  slots: {
    title: 'Available slots',
    newSlot: 'New slot',
    editSlot: 'Edit slot',
    empty: 'No slots for this service yet.',
    noServices: 'No services — create one first',

    dayOfWeek: 'Day of week',
    day: 'Day',
    time: 'Time',
    startTime: 'Start time',
    endTime: 'End time',
    maxBookings: 'Max bookings',

    confirmDelete: 'Delete this slot?',
    saveFailed: 'Could not save the slot.',
    deleteFailed: 'Could not delete the slot.',
  },

  bookings: {
    bookTitle: 'Book: {name}',
    chooseSlot: 'Choose a slot',
    noSlots: 'This service has no available slots.',
    confirmBooking: 'Confirm booking',
    serviceNotFound: 'Service not found.',
    loadFailed: 'Could not load this service.',
    createFailed: 'Could not create the booking.',
    slotRequired: 'Please choose a slot.',
    dayMismatch: 'The chosen date does not fall on the slot’s day of week.',
    pickDay: 'Pick a {day}.',

    mineTitle: 'My bookings',
    mineEmpty: 'You have no bookings yet.',
    browseServices: 'Browse services',
    allTitle: 'All bookings',
    allEmpty: 'No bookings yet.',
    unnamedService: 'Service #{id}',

    confirmCancel: 'Cancel this booking?',
    cancelFailed: 'Could not cancel the booking.',
    confirmFailed: 'Could not confirm the booking.',

    status: {
      pending: 'pending',
      confirmed: 'confirmed',
      cancelled: 'cancelled',
    },
  },

  admin: {
    welcome: 'Welcome back, {name}',
    overview: 'Here’s an overview of your booking system.',

    cards: {
      services: 'Services',
      servicesSub: '{count} active',
      bookings: 'Bookings',
      bookingsSub: 'all time',
      pending: 'Pending',
      pendingSub: 'awaiting confirmation',
      confirmed: 'Confirmed',
      confirmedSub: 'upcoming',
    },

    shortcuts: {
      services: 'Manage services',
      slots: 'Manage slots',
      bookings: 'Manage bookings',
    },
  },
}

export default en

// Shape every other locale must satisfy exactly.
export type Messages = typeof en
