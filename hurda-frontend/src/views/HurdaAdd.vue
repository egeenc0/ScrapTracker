<template>
  <div class="container">
    <header class="page-header">
      <h2>Yeni eşya ekle</h2>
      <p class="page-lead">
        Malzeme bilgilerini girin; kayıt listeye eklenir.
      </p>
    </header>

    <div class="add-row-panel">
      <div class="add-row-grid">
        <label class="add-field">
          <span>Malzeme kodu</span>
          <input v-model.trim="newItem.materialCode" type="text" placeholder="0008002" />
        </label>
        <label class="add-field">
          <span>Kalıp kodu</span>
          <input v-model.trim="newItem.moldCode" type="text" placeholder="C000007391" />
        </label>
        <label class="add-field">
          <span>Cavity numarası</span>
          <input v-model.trim="newItem.cavity" type="text" placeholder="A-B-C-D" />
        </label>
        <label class="add-field">
          <span>Başlangıç hurda sayısı</span>
          <input v-model.number="newItem.scrapAmount" type="number" min="0" />
        </label>
        <label class="add-field add-field-wide">
          <span>Malzeme tanımı *</span>
          <input v-model.trim="newItem.name" type="text" placeholder="Örn. HV CHIMNEY" />
        </label>
      </div>
      <div class="add-row-actions">
        <button
          type="button"
          class="btn btn-save"
          :disabled="adding || !newItem.name"
          @click="submitNewItem"
        >
          {{ adding ? 'Kaydediliyor…' : 'Ekle' }}
        </button>
        <button type="button" class="btn btn-cancel" :disabled="adding" @click="resetNewItem">
          Temizle
        </button>
        <button type="button" class="btn btn-secondary" :disabled="adding" @click="goToList">
          Listeye dön
        </button>
      </div>
    </div>

    <p v-if="message" class="message">{{ message }}</p>
  </div>
</template>

<script>
import api from '../api'

export default {
  data() {
    return {
      adding: false,
      message: '',
      newItem: {
        materialCode: '',
        name: '',
        moldCode: '',
        cavity: '',
        scrapAmount: 0,
      },
    }
  },
  methods: {
    buildDescriptionFromParts(mc, mk, cav) {
      const k = (mc || '').trim()
      const m = (mk || '').trim()
      const c = (cav || '').trim()
      return `Kod: ${k} | Kalıp: ${m} | Cavity: ${c}`
    },
    resetNewItem() {
      this.newItem = {
        materialCode: '',
        name: '',
        moldCode: '',
        cavity: '',
        scrapAmount: 0,
      }
    },
    goToList() {
      this.$router.push('/hurda')
    },
    async submitNewItem() {
      if (!this.newItem.name) return
      this.adding = true
      this.message = ''
      try {
        const description = this.buildDescriptionFromParts(
          this.newItem.materialCode,
          this.newItem.moldCode,
          this.newItem.cavity
        )
        await api.post('/hurda/items', {
          name: this.newItem.name,
          description,
          scrapAmount: Number(this.newItem.scrapAmount) || 0,
        })
        this.resetNewItem()
        this.$router.push({ path: '/hurda', query: { added: '1' } })
      } catch (err) {
        const status = err.response?.status
        const data = err.response?.data
        let detail =
          typeof data === 'string'
            ? data
            : data?.message || data?.title || (data && JSON.stringify(data))
        if (status === 401) {
          detail = 'Oturum süresi dolmuş veya giriş yok. Çıkış yapıp tekrar giriş yapın.'
        } else if (status === 403) {
          detail = 'Bu işlem için Admin yetkisi gerekir.'
        }
        this.message =
          detail || `Eklenemedi (sunucu ${status ?? '—'}). Tekrar deneyin veya API loglarına bakın.`
      } finally {
        this.adding = false
      }
    },
  },
}
</script>
