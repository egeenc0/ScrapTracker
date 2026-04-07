<template>
  <div id="app">
    <nav v-if="isLoggedIn" class="navbar">
      <div class="nav-left">
        <span class="brand">Hurda Yönetim</span>
        <router-link
          v-if="isAdmin"
          to="/hurda/yeni"
          class="nav-link"
          :class="{ 'nav-link-active': $route.path === '/hurda/yeni' }"
        >
          Yeni eşya ekle
        </router-link>
        <router-link
          to="/hurda"
          class="nav-link"
          :class="{ 'nav-link-active': $route.path === '/hurda' }"
        >
          Liste
        </router-link>
      </div>
      <div class="nav-right">
        <span class="user-info">{{ username }} ({{ role }})</span>
        <button @click="logout" class="btn btn-logout">Çıkış</button>
      </div>
    </nav>
    <router-view />
  </div>
</template>

<script>
export default {
  computed: {
    isLoggedIn() {
      return !!localStorage.getItem('token')
    },
    isAdmin() {
      return localStorage.getItem('role') === 'Admin'
    },
    username() {
      return localStorage.getItem('username') || ''
    },
    role() {
      return localStorage.getItem('role') || ''
    },
  },
  methods: {
    logout() {
      localStorage.removeItem('token')
      localStorage.removeItem('username')
      localStorage.removeItem('role')
      this.$router.push('/login')
    },
  },
}
</script>
