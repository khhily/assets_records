import { createRouter, createWebHistory } from 'vue-router';
import AssetList from '../views/AssetList.vue';
import AssetCreate from '../views/AssetCreate.vue';
import AssetChart from '../views/AssetChart.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: AssetList,
    },
    {
      path: '/create',
      name: 'create',
      component: AssetCreate,
    },
    {
      path: '/edit/:batchId',
      name: 'edit',
      component: AssetCreate,
      props: true,
    },
    {
      path: '/chart',
      name: 'chart',
      component: AssetChart,
    },
  ],
});

export default router;
