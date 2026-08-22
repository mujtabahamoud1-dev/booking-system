import type { Messages } from "./en";

// Typed against the English shape: drop a key, add a stray one, or misspell a
// nested group and `vue-tsc` fails the build.
const ar: Messages = {
  common: {
    language: "اللغة",
    english: "English",
    arabic: "العربية",

    brand: {
      name: "أطلس",
      discipline: "العلاج الطبيعي",
      note: "المواعيد بالحجز المسبق فقط",
    },

    nav: {
      services: "الخدمات",
      dashboard: "لوحة التحكم",
      slots: "الأوقات",
      bookings: "الحجوزات",
      myBookings: "حجوزاتي",
      login: "تسجيل الدخول",
      register: "إنشاء حساب",
      logout: "تسجيل الخروج",
      admin: "مشرف",
    },

    actions: {
      save: "حفظ",
      saveChanges: "حفظ التغييرات",
      create: "إنشاء",
      cancel: "إلغاء",
      edit: "تعديل",
      delete: "حذف",
      close: "إغلاق",
      confirm: "تأكيد",
    },

    confirmDialog: {
      title: "يرجى التأكيد",
      dismiss: "تراجع",
    },

    fields: {
      name: "الاسم",
      description: "الوصف",
      email: "البريد الإلكتروني",
      password: "كلمة المرور",
      phone: "رقم الهاتف",
      date: "التاريخ",
      dateTime: "التاريخ والوقت",
      time: "الوقت",
      notes: "ملاحظات",
      status: "الحالة",
      service: "الخدمة",
      duration: "المدة",
      price: "السعر",
      user: "المراجع",
      optional: "اختياري",
      active: "مفعّلة",
    },

    search: {
      clear: "مسح البحث",
      results: "عرض {count} من {total}",
    },

    filters: {
      status: "تصفية حسب الحالة",
      day: "تصفية حسب اليوم",
      date: "تصفية حسب التاريخ",
      clear: "مسح عوامل التصفية",
    },

    loading: "جارٍ التحميل",
    minutesShort: "{count} دقيقة",
    minutesUnit: "دقيقة",
    emptyValue: "—",

    days: {
      0: "الأحد",
      1: "الإثنين",
      2: "الثلاثاء",
      3: "الأربعاء",
      4: "الخميس",
      5: "الجمعة",
      6: "السبت",
    },
  },

  auth: {
    signIn: "تسجيل الدخول",
    signInHelp: "سجّل الدخول لحجز جلسة أو لمتابعة مواعيدك.",
    createAccount: "إنشاء حساب",
    createAccountHelp: "تحتاج إلى حساب لحجز موعد.",
    register: "إنشاء حساب",
    noAccount: "أول مرة هنا؟",
    haveAccount: "لديك حساب بالفعل؟",
    loginFailed: "البريد الإلكتروني أو كلمة المرور غير صحيحة.",
    registerFailed: "تعذّر إنشاء الحساب. تحقّق من البيانات وحاول مرة أخرى.",
  },

  services: {
    eyebrow: "المواعيد",
    title: "احجز جلستك القادمة",
    subtitle: "اختر خدمة لعرض أوقاتها خلال الأسبوع، ثم اختر الوقت المناسب لك.",
    sessionLength: "مدة الجلسة",
    runsOn: "أيام العمل",
    noDays: "لا توجد أوقات بعد",
    empty: "لا توجد خدمات متاحة للحجز بعد.",
    noDescription: "لا يوجد وصف بعد.",
    bookNow: "احجز",

    adminTitle: "الخدمات",
    adminSubtitle: "الخدمات التي يقدّمها المركز، ومدة كل جلسة وسعرها.",
    newService: "خدمة جديدة",
    editService: "تعديل الخدمة",
    adminEmpty: "لا توجد خدمات بعد. أنشئ الأولى لبدء استقبال الحجوزات.",
    durationField: "المدة (دقيقة)",

    nameEn: "الاسم (بالإنجليزية)",
    nameAr: "الاسم (بالعربية)",
    descriptionEn: "الوصف (بالإنجليزية)",
    descriptionAr: "الوصف (بالعربية)",
    translationHint: "يظهر للمرضى الذين يتصفحون الموقع بالعربية.",
    statusActive: "مفعّلة",
    statusInactive: "موقوفة",
    activeHelp: "متاحة للحجز",

    searchLabel: "البحث في الخدمات",
    searchPlaceholder: "الاسم أو الوصف",
    filterAll: "الكل",
    filterEmpty: "لا توجد خدمات مطابقة لعوامل التصفية.",

    confirmDelete: "حذف «{name}»؟ لا يمكن التراجع عن هذا.",
    saveFailed: "تعذّر حفظ الخدمة.",
    deleteFailed: "تعذّر حذف الخدمة. أوقفها بدلاً من ذلك إذا كانت مرتبطة بحجوزات.",
  },

  slots: {
    title: "الأوقات",
    subtitle: "ساعات توفّر كل خدمة، تُضبط مرة واحدة وتتكرر كل أسبوع.",
    newSlot: "وقت جديد",
    editSlot: "تعديل الوقت",
    empty: "لا توجد أوقات لهذه الخدمة، لذلك لا يمكن حجزها بعد.",
    noServices: "لا توجد خدمات — أنشئ واحدة أولاً",

    weekCover: "التغطية الأسبوعية",
    weeklyHours: "{hours} ساعة أسبوعياً",
    selectHint: "اختر وقتاً لتعديله أو حذفه.",
    capacityOf: "السعة {count}",
    dayClosed: "مغلق",

    allServices: "كل الخدمات",
    allServicesCover: "التغطية عبر كل الخدمات",
    everyDay: "كل الأيام",
    createNeedsService: "اختر خدمة واحدة لإضافة وقت إليها.",

    dayOfWeek: "يوم الأسبوع",
    startTime: "وقت البداية",
    endTime: "وقت النهاية",
    maxBookings: "الحد الأقصى للحجوزات",

    confirmDelete: "حذف هذا الوقت؟",
    saveFailed: "تعذّر حفظ الوقت.",
    deleteFailed: "تعذّر حذف الوقت لوجود حجوزات مرتبطة به.",
  },

  bookings: {
    backToServices: "كل الخدمات",
    stepTime: "اختر الوقت",
    stepTimeHelp: "هذه ساعات توفّر الخدمة خلال الأسبوع.",
    stepDate: "اختر التاريخ",
    stepDateHelp: "هذا الوقت متاح يوم {day}. اختر أحد التواريخ القادمة، أو حدّد تاريخاً آخر.",
    stepNotes: "ما الذي يجب أن نعرفه",
    stepNotesHelp: "الإصابات، أو عملية جراحية حديثة، أو ما ترغب في العمل عليه.",
    notesPlaceholder: "اختياري",
    otherDate: "اختيار تاريخ آخر",
    summaryLead: "الحجز",

    chooseSlot: "الأوقات المتاحة",
    noSlots: "لا توجد أوقات متاحة لهذه الخدمة بعد.",
    confirmBooking: "تأكيد الحجز",
    serviceNotFound: "هذه الخدمة غير موجودة.",
    loadFailed: "تعذّر تحميل هذه الخدمة.",
    createFailed: "تعذّر إنشاء الحجز.",
    slotRequired: "اختر وقتاً أولاً.",
    dayMismatch: "التاريخ المحدّد لا يقع في يوم الوقت المختار.",
    pickDay: "اختر يوم {day}.",

    sentEyebrow: "تم استلام الطلب",
    sentTitle: "طلبك وصل إلى العيادة",
    sentBody:
      "لم يتم تأكيد الموعد بعد. يراجع الاستقبال الطلبات الجديدة خلال ساعات العمل، وسيتحوّل الموعد إلى «مؤكد» بمجرد اعتماده من أحد الموظفين.",
    sentWhen: "الموعد المطلوب",
    viewBookings: "عرض حجوزاتي",
    bookAnother: "حجز جلسة أخرى",

    mineTitle: "حجوزاتي",
    mineSubtitle: "مواعيدك، الأقرب أولاً.",
    mineEmpty: "لا توجد لديك مواعيد محجوزة.",
    browseServices: "تصفّح الخدمات",
    allTitle: "الحجوزات",
    allSubtitle: "جميع المواعيد في المركز.",
    allEmpty: "لا توجد حجوزات بعد.",
    filterAll: "الكل",
    filterEmpty: "لا توجد حجوزات مطابقة لعوامل التصفية.",
    unnamedService: "خدمة رقم {id}",

    searchLabel: "البحث في الحجوزات",
    searchPlaceholder: "المراجع أو البريد أو الهاتف أو الخدمة",

    dateAll: "أي تاريخ",
    dateToday: "اليوم",
    dateWeek: "خلال ٧ أيام",
    dateUpcoming: "القادمة",
    datePast: "السابقة",

    confirmCancel: "إلغاء هذا الحجز؟",
    cancelFailed: "تعذّر إلغاء الحجز.",
    confirmFailed: "تعذّر تأكيد الحجز.",

    status: {
      pending: "قيد الانتظار",
      confirmed: "مؤكّد",
      cancelled: "ملغى",
    },
  },

  admin: {
    welcome: "أهلاً بعودتك، {name}",
    manage: "الإدارة",

    pendingLead: "بانتظار التأكيد",
    pendingAction: "مراجعة الحجوزات",
    pendingClear: "لا يوجد ما ينتظر التأكيد.",

    metrics: {
      services: "الخدمات",
      active: "المفعّلة",
      total: "الحجوزات",
      confirmed: "المؤكّدة",
      cancelled: "الملغاة",
    },

    shortcuts: {
      services: "الخدمات",
      servicesHint: "أضف خدمة أو عدّل مدتها وسعرها",
      slots: "الأوقات",
      slotsHint: "اضبط ساعات توفّر كل خدمة",
      bookings: "الحجوزات",
      bookingsHint: "أكّد المواعيد أو ألغِها",
    },
  },
};

export default ar;
