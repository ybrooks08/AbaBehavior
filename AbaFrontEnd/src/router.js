import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

export default new Router({
  //mode: 'history',
  routes: [
    //{ path: '/', redirect: '/home' },
    {
      path: '/login',
      name: 'login',
      component: () => import(/* webpackChunkName: "Login" */ '@/views/Login'),
    },
    {
      path: "/sign/:id?",
      name: 'sign',
      component: () => import(/* webpackChunkName: "SignSession" */ '@/components/sessions/Sign/SignSession'),
      props: true,
    },
    {
      path: '/',
      component: () => import(/* webpackChunkName: "MainLayout" */ '@/views/MainLayout'),
      redirect: '/home',
      children: [
        {
          path: '/403',
          name: '403',
          component: () => import(/* webpackChunkName: "403" */ '@/views/403'),
        },
        {
          path: '/home',
          name: 'home',
          component: () => import(/* webpackChunkName: "Home" */ '@/views/Home'),
        },
        {
          path: '/users',
          name: 'users',
          component: () => import(/* webpackChunkName: "Users" */ '@/components/users/Users'),
          meta: { rol: ['admin', 'hr'] },
        },
        {
          path: '/users/add_edit/:id?',
          name: 'AddEditUsers',
          component: () => import(/* webpackChunkName: "AddEditUser" */ '@/components/users/AddEditUser'),
          props: true,
          meta: { rol: ['admin', 'hr'] },
        },
        {
          path: '/users/user_details/:id',
          name: 'UserDetails',
          component: () => import(/* webpackChunkName: "UserDetails" */ '@/components/users/UserDetails'),
          props: true,
          meta: { rol: ['admin', 'hr'] },
        },
        {
          path: '/problems-behaviors',
          name: 'ProblemsBehaviors',
          component: () => import(/* webpackChunkName: "Problems" */ '@/views/ProblemsBehaviors'),
          meta: { rol: ['admin'] },
        },
        {
          path: '/replacement-program',
          name: 'ReplacementProgram',
          component: () => import(/* webpackChunkName: "ReplacementProgram" */ '@/views/ReplacementProgram'),
          meta: { rol: ['admin'] },
        },
        {
          path: '/diagnosis',
          name: 'Diagnosis',
          component: () => import(/* webpackChunkName: "Diagnosis" */ '@/views/Diagnosis'),
          meta: { rol: ['admin', 'management'] },
        },
        {
          path: '/clients',
          name: 'Clients',
          component: () => import(/* webpackChunkName: "Clients" */ '@/components/clients/Clients'),
          meta: { rol: ['admin', 'management'] },
        },
        {
          path: '/clients/add_edit/:id?',
          name: 'NewClient',
          component: () => import(/* webpackChunkName: "AddEditClient" */ '@/components/clients/AddEditClient'),
          props: true,
          meta: { rol: ['admin', 'management'] },
        },
        {
          path: '/clients/client_details/:id',
          name: 'ClientDetails',
          component: () => import(/* webpackChunkName: "ClientDetails" */ '@/components/clients/ClientDetails'),
          props: true,
          meta: { rol: ['admin', 'management', 'analyst', 'assistant', 'tech'] },
        },
        {
          path: '/clients/sessions_details',
          name: 'SessionsDetails',
          component: () => import(/* webpackChunkName: "SessionDetails" */ '@/components/sessions/SessionsDetails'),
          meta: { rol: ['admin', 'analyst', 'assistant', 'tech'] },
        },
        {
          path: '/clients/session_notes',
          name: 'SessionNotes',
          component: () => import(/* webpackChunkName: "SessionNotes" */ '@/components/sessions/SessionNotes'),
          meta: { rol: ['admin', 'analyst', 'assistant', 'tech'] },
        },
        {
          path: '/session/session_collect_data',
          name: 'SessionCollectData',
          component: () => import(/* webpackChunkName: "CollectData" */ '@/components/sessions/SessionCollectData'),
          meta: { rol: ['admin', 'analyst', 'assistant', 'tech'] },
        },
        {
          path: '/clients/new_session/:sessionType?',
          name: 'NewSession',
          component: () => import(/* webpackChunkName: "NewSession" */ '@/components/sessions/NewSession'),
          props: true,
          meta: { rol: ['analyst', 'assistant', 'tech'] },
        },
        {
          path: '/clients/edit_monthly_note',
          name: 'MonthlyNote',
          component: () => import(/* webpackChunkName: "MonthlyNote" */ '@/components/sessions/MonthlyNote'),
          meta: { rol: ['analyst', 'assistant'] },
        },
        {
          path: '/clients/new_training',
          name: 'NewTraining',
          component: () => import(/* webpackChunkName: "NewTraining" */ '@/components/sessions/NewTraining'),
          meta: { rol: ['analyst', 'assistant'] },
        },
        {
          path: '/clients/add_edit_chart_note/:id?',
          name: 'AddEditCharNote',
          component: () => import(/* webpackChunkName: "AddEditChartNote" */ '@/components/sessions/AddEditChartNote'),
          props: true,
          meta: { rol: ['analyst', 'assistant', 'tech'] },
        },
        {
          path: '/competency_checks',
          name: 'CompetencyChecks',
          component: () => import(/* webpackChunkName: "CompetencyChecks" */  '@/components/sessions/CompetencyChecks/CompetencyChecks'),
          meta: { rol: ['analyst', 'assistant'] },
        },
        {
          path: '/competency_checks/new_edit/:id?',
          name: 'AddEditCompetencyCheck',
          component: () => import(/* webpackChunkName: "AddEditCompetencyCheck" */ '@/components/sessions/CompetencyChecks/AddEditCompetencyCheck'),
          props: true,
          meta: { rol: ['analyst', 'assistant'] },
        },
        {
          path: '/reporting/billing_guide',
          name: 'BillingGuide',
          component: () => import(/* webpackChunkName: "BillingGuide" */ '@/components/reporting/BillingGuide'),
          meta: { rol: ['admin', 'billing'] },
        },
        {
          path: '/reporting/sessions_by_users',
          name: 'SessionsByUsers',
          component: () => import(/* webpackChunkName: "SessionsByUsers" */ '@/components/reporting/SessionsByUsers'),
          meta: { rol: ['admin', 'billing'] },
        },
        {
          path: '/reporting/rbt_ba_services_log',
          name: 'RbtBaServicesLog',
          component: () => import(/* webpackChunkName: "RbtBaServicesLog" */ '@/components/reporting/RbtBaServicesLog'),
          meta: { rol: ['assistant', 'tech', 'management', 'admin', 'analyst', 'billing'] },
        },
        {
          path: '/reporting/sessions-history',
          name: 'SessionsHistory',
          component: () => import(/* webpackChunkName: "SessionsHistory" */ '@/components/reporting/SessionsHistory'),
          meta: { rol: ['assistant', 'tech', 'analyst', 'admin'] },
        },
        {
          path: '/reporting/client_progress',
          name: 'ClientProgress',
          component: () => import(/* webpackChunkName: "ClientProgress" */ '@/components/reporting/ClientProgress'),
          meta: { rol: ['admin'] },
        },
        {
          path: '/user-sign/:userId',
          name: 'UserSign',
          component: () => import(/* webpackChunkName: "UserSign" */ '@/components/users/UserSign'),
          props: true,
        },
        {
          path: '/session/session_print',
          name: 'SessionPrint',
          component: () => import(/* webpackChunkName: "SessionPrint" */ '@/components/sessions/SessionPrint'),
          meta: { rol: ['assistant', 'tech', 'analyst', 'admin'] },
        },
        {
          path: '/documents_setup',
          name: 'Documents',
          component: () => import(/* webpackChunkName: "Documents" */ '@/views/Documents'),
          meta: { rol: ['admin', 'hr'] },
        },
        {
          path: '/system_logs',
          name: 'SystemLogs',
          component: () => import(/* webpackChunkName: "SystemLogs" */ '@/views/SystemLogs'),
          meta: { rol: ['admin'] },
        },
        {
          path: "/reporting/time-sheet",
          name: 'TimeSheet',
          component: () => import(/* webpackChunkName: "TimeSheet" */ '@/components/reporting/TimeSheet'),
          meta: { rol: ['admin', 'billing', 'assistant', 'tech', 'analyst'] },
        },
        {
          path: "/reporting/client_progress_print",
          name: "ClientProgressPrint",
          component: () => import(/* webpackChunkName: "ClientProgressPrint" */ "@/components/reporting/ClientProgressPrint"),
          meta: { rol: ["admin", "assistant", "tech", "analyst"] }
        },
        {
          path: "/reporting/monthly-notes",
          name: "MonthlyNotes",
          component: () => import(/* webpackChunkName: "MonthlyNotes" */ "@/components/reporting/MonthlyNotes"),
          meta: { rol: ["admin", "analyst", "assistant", "tech"] }
        },
        {
          path: "/reporting/monthly-week-data",
          name: "MonthlyWeekData",
          component: () => import(/* webpackChunkName: "MonthlyWeekData" */ "@/components/reporting/MonthlyWeekData"),
          meta: { rol: ["admin", "analyst", "assistant", "tech"] }
        },
        {
          path: "/clients/caregiver_data_collection",
          name: "CaregiverDataCollection",
          component: () => import(/* webpackChunkName: "CaregiverDataCollection" */ "@/components/sessions/CaregiverDataCollection"),
          meta: { rol: ["analyst"] }
        },
        {
          path: "/video_tutorials",
          name: "VideoTutorials",
          component: () => import(/* webpackChunkName: "VideoTutorials" */ "@/components/others/VideoTutorials"),
          meta: { rol: ["admin", "assistant", "tech", "analyst"] }
        },
        {
          path: '/reporting/ready-to-bill',
          name: 'Ready2Bill',
          component: () => import(/* webpackChunkName: "Ready2Bill" */ '@/components/reporting/Ready2Bill'),
          meta: { rol: ['admin', 'billing'] },
        },
      ],
    },
    { path: '*', redirect: '/' },
  ],
});