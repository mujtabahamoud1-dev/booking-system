import type { Messages } from './en'

// Typed against the English shape: drop a key, add a stray one, or misspell a
// nested group and `vue-tsc` fails the build.
const ar: Messages = {
  common: {
    appName: 'نظام الحجوزات',
    language: 'اللغة',
    english: 'English',
    arabic: 'العربية',

    nav: {
      services: 'الخدمات',
      dashboard: 'لوحة التحكم',
      slots: 'المواعيد',
      bookings: 'الحجوزات',
      myBookings: 'حجوزاتي',
      login: 'تسجيل الدخول',
      register: 'إنشاء حساب',
      logout: 'تسجيل الخروج',
      admin: 'مشرف',
    },

    actions: {
      save: 'حفظ',
      saveChanges: 'حفظ التغييرات',
      create: 'إنشاء',
      cancel: 'إلغاء',
      edit: 'تعديل',
      delete: 'حذف',
      close: 'إغلاق',
      confirm: 'تأكيد',
    },

    fields: {
      name: 'الاسم',
      description: 'الوصف',
      email: 'البريد الإلكتروني',
      password: 'كلمة المرور',
      phone: 'رقم الهاتف',
      date: 'التاريخ',
      notes: 'ملاحظات',
      status: 'الحالة',
      service: 'الخدمة',
      duration: 'المدة',
      price: 'السعر',
      user: 'المستخدم',
      optional: 'اختياري',
      active: 'مفعّلة',
    },

    loading: 'جارٍ التحميل…',
    minutesShort: '{count} دقيقة',
    emptyValue: '—',

    days: {
      0: 'الأحد',
      1: 'الإثنين',
      2: 'الثلاثاء',
      3: 'الأربعاء',
      4: 'الخميس',
      5: 'الجمعة',
      6: 'السبت',
    },
  },

  auth: {
    signIn: 'تسجيل الدخول',
    createAccount: 'إنشاء حساب',
    register: 'تسجيل',
    noAccount: 'ليس لديك حساب؟',
    haveAccount: 'لديك حساب بالفعل؟',
    loginFailed: 'فشل تسجيل الدخول.',
    registerFailed: 'فشل إنشاء الحساب.',
  },

  services: {
    title: 'خدماتنا',
    subtitle: 'اختر خدمة لحجز موعد.',
    empty: 'لا توجد خدمات متاحة بعد.',
    noDescription: 'لا يوجد وصف.',
    bookNow: 'احجز الآن',

    adminTitle: 'الخدمات',
    newService: 'خدمة جديدة',
    editService: 'تعديل الخدمة',
    adminEmpty: 'لا توجد خدمات بعد.',
    durationField: 'المدة (بالدقائق)',
    statusActive: 'مفعّلة',
    statusInactive: 'معطّلة',

    confirmDelete: 'هل تريد حذف "{name}"؟ لا يمكن التراجع عن هذا الإجراء.',
    saveFailed: 'تعذّر حفظ الخدمة.',
    deleteFailed: 'تعذّر حذف الخدمة.',
  },

  slots: {
    title: 'المواعيد المتاحة',
    newSlot: 'موعد جديد',
    editSlot: 'تعديل الموعد',
    empty: 'لا توجد مواعيد لهذه الخدمة بعد.',
    noServices: 'لا توجد خدمات — أضف خدمة أولاً',

    dayOfWeek: 'يوم الأسبوع',
    day: 'اليوم',
    time: 'الوقت',
    startTime: 'وقت البدء',
    endTime: 'وقت الانتهاء',
    maxBookings: 'الحد الأقصى للحجوزات',

    confirmDelete: 'هل تريد حذف هذا الموعد؟',
    saveFailed: 'تعذّر حفظ الموعد.',
    deleteFailed: 'تعذّر حذف الموعد.',
  },

  bookings: {
    bookTitle: 'حجز: {name}',
    chooseSlot: 'اختر موعداً',
    noSlots: 'لا توجد مواعيد متاحة لهذه الخدمة.',
    confirmBooking: 'تأكيد الحجز',
    serviceNotFound: 'الخدمة غير موجودة.',
    loadFailed: 'تعذّر تحميل هذه الخدمة.',
    createFailed: 'تعذّر إنشاء الحجز.',
    slotRequired: 'يرجى اختيار موعد.',
    dayMismatch: 'التاريخ المختار لا يقع في يوم الموعد المحدد.',
    pickDay: 'اختر يوم {day}.',

    mineTitle: 'حجوزاتي',
    mineEmpty: 'ليس لديك أي حجوزات بعد.',
    browseServices: 'تصفّح الخدمات',
    allTitle: 'جميع الحجوزات',
    allEmpty: 'لا توجد حجوزات بعد.',
    unnamedService: 'خدمة رقم {id}',

    confirmCancel: 'هل تريد إلغاء هذا الحجز؟',
    cancelFailed: 'تعذّر إلغاء الحجز.',
    confirmFailed: 'تعذّر تأكيد الحجز.',

    status: {
      pending: 'قيد الانتظار',
      confirmed: 'مؤكد',
      cancelled: 'ملغى',
    },
  },

  admin: {
    welcome: 'أهلاً بعودتك، {name}',
    overview: 'هذه نظرة عامة على نظام الحجوزات لديك.',

    cards: {
      services: 'الخدمات',
      servicesSub: '{count} مفعّلة',
      bookings: 'الحجوزات',
      bookingsSub: 'الإجمالي',
      pending: 'قيد الانتظار',
      pendingSub: 'بانتظار التأكيد',
      confirmed: 'مؤكدة',
      confirmedSub: 'قادمة',
    },

    shortcuts: {
      services: 'إدارة الخدمات',
      slots: 'إدارة المواعيد',
      bookings: 'إدارة الحجوزات',
    },
  },
}

export default ar
