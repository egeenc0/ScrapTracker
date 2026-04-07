<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Hurda Yönetim</h1>
      <p class="subtitle">Giriş Yap</p>
      <form @submit.prevent="login">
        <div class="form-group">
          <label>Kullanıcı Adı</label>
          <input v-model="username" type="text" placeholder="Kullanıcı adınız" required />
        </div>
        <div class="form-group">
          <label>Şifre</label>
          <input v-model="password" type="password" placeholder="Şifreniz" required />
        </div>
        <p v-if="error" class="error">{{ error }}</p>
        <button type="submit" class="btn btn-primary" :disabled="loading">
          {{ loading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script>
import api from '../api'

export default {
  data() {
    return {
      username: '',
      password: '',
      error: '',
      loading: false,
    }
  },
  methods: {
    async login() {
      this.error = ''
      this.loading = true
      try {
        const res = await api.post('/auth/login', {
          username: this.username,
          password: this.password,
        })
        const d = res.data
        const token = d.token ?? d.Token
        if (!token) {
          this.error = 'Sunucu yanıtında token yok.'
          return
        }
        localStorage.setItem('token', token)
        localStorage.setItem('username', d.username ?? d.Username ?? '')
        localStorage.setItem('role', d.role ?? d.Role ?? '')
        this.$router.push('/hurda')
      } catch {
        this.error = 'Kullanıcı adı veya şifre hatalı'
      } finally {
        this.loading = false
      }
    },
  },
}
</script>
