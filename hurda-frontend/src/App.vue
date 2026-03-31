<template>
  <div id="app">
    <nav v-if="isLoggedIn" class="navbar">
      <span class="brand">Hurda Yönetim</span>
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
