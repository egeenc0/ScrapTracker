<template>
  <div class="container">
    <header class="page-header">
      <h2>Hurda listesi</h2>
      <p v-if="isAdmin" class="page-lead">
        Malzeme tanımlarını görüntüleyin. Yeni kayıt için üst menüden «Yeni eşya ekle»; hurda
        güncellemesi için satırdaki Düzenle.
      </p>
      <p v-else class="page-lead">Malzeme tanımlarını görüntüleyin (salt okunur).</p>
    </header>

    <div class="search-row">
      <div class="search-group">
        <label for="material-search" class="search-label">Malzeme tanımında ara</label>
        <input
          id="material-search"
          v-model.trim="searchQuery"
          type="search"
          class="search-input"
          placeholder="Örn. BMW, bobbin…"
          autocomplete="off"
        />
      </div>
    </div>

    <div class="table-scroll">
    <table class="hurda-table">
      <thead>
        <tr>
          <th
            class="th-sortable col-id"
            :class="{ 'th-sort-active': sortKey === 'materialCode' }"
            @click="setSort('materialCode')"
          >
            Malzeme Kodu
            <span class="sort-indicator" v-if="sortKey === 'materialCode'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th
            class="th-sortable col-name"
            :class="{ 'th-sort-active': sortKey === 'name' }"
            @click="setSort('name')"
          >
            Malzeme Tanımı
            <span class="sort-indicator" v-if="sortKey === 'name'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th
            class="th-sortable col-desc"
            :class="{ 'th-sort-active': sortKey === 'moldCode' }"
            @click="setSort('moldCode')"
          >
            Kalıp Kodu
            <span class="sort-indicator" v-if="sortKey === 'moldCode'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th
            class="th-sortable col-cavity"
            :class="{ 'th-sort-active': sortKey === 'cavity' }"
            @click="setSort('cavity')"
          >
            Cavity Numarası
            <span class="sort-indicator" v-if="sortKey === 'cavity'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th
            class="th-sortable col-scrap"
            :class="{ 'th-sort-active': sortKey === 'scrapAmount' }"
            @click="setSort('scrapAmount')"
          >
            Başlangıç Hurda Sayısı
            <span class="sort-indicator" v-if="sortKey === 'scrapAmount'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th
            class="th-sortable col-date"
            :class="{ 'th-sort-active': sortKey === 'createdDate' }"
            @click="setSort('createdDate')"
          >
            Oluşturma Tarihi
            <span class="sort-indicator" v-if="sortKey === 'createdDate'">{{ sortDir === 'asc' ? ' ▲' : ' ▼' }}</span>
          </th>
          <th v-if="isAdmin" class="col-actions">İşlem</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in filteredSortedItems" :key="item.id">
          <td class="col-id">{{ displayMaterialCode(item) }}</td>
          <td class="col-name">{{ item.name }}</td>
          <td class="col-desc">{{ displayMoldCode(item) }}</td>
          <td class="col-cavity">{{ displayCavity(item) }}</td>
          <td class="col-scrap">
            <span v-if="editingId !== item.id">{{ item.scrapAmount }}</span>
            <input
              v-else
              v-model.number="editScrapAmount"
              type="number"
              class="edit-input"
            />
          </td>
          <td class="col-date">{{ formatDate(item.createdDate) }}</td>
          <td v-if="isAdmin" class="col-actions">
            <button
              v-if="editingId !== item.id"
              @click="startEdit(item)"
              class="btn btn-edit"
            >
              Düzenle
            </button>
            <div v-else class="edit-actions">
              <button @click="saveEdit(item.id)" class="btn btn-save">Kaydet</button>
              <button @click="cancelEdit" class="btn btn-cancel">İptal</button>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    </div>

    <p v-if="message" class="message">{{ message }}</p>
  </div>
</template>

<script>
import api from '../api'

export default {
  data() {
    return {
      items: [],
      searchQuery: '',
      sortKey: 'name',
      sortDir: 'asc',
      editingId: null,
      editScrapAmount: 0,
      message: '',
    }
  },
 computed: {
  filteredSortedItems() {
    const q = this.searchQuery.toLocaleLowerCase('tr-TR').trim()
    let liste = [...this.items]

    if (q) {
      liste = liste.filter((item) =>
        (item.name || '').toLocaleLowerCase('tr-TR').includes(q)
      )
    }

    const key = this.sortKey
    const dir = this.sortDir === 'asc' ? 1 : -1

    const cmp = (a, b) => {
      let va
      let vb
      if (key === 'materialCode' || key === 'moldCode' || key === 'cavity') {
        const pa = this.parseHurdaDescription(a.description)
        const pb = this.parseHurdaDescription(b.description)
        va = (pa[key] || '').toString()
        vb = (pb[key] || '').toString()
        return dir * va.localeCompare(vb, 'tr', { numeric: true, sensitivity: 'base' })
      }
      if (key === 'scrapAmount') {
        va = Number(a.scrapAmount) || 0
        vb = Number(b.scrapAmount) || 0
        return dir * (va - vb)
      }
      if (key === 'createdDate') {
        va = a.createdDate ? new Date(a.createdDate).getTime() : 0
        vb = b.createdDate ? new Date(b.createdDate).getTime() : 0
        return dir * (va - vb)
      }
      va = (a[key] || '').toString()
      vb = (b[key] || '').toString()
      return dir * va.localeCompare(vb, 'tr')
    }

    liste.sort(cmp)
    return liste
  },
    isAdmin() {
      return localStorage.getItem('role') === 'Admin'
    },
  },
  async mounted() {
    if (this.$route.query.added === '1') {
      this.message = 'Yeni kayıt eklendi.'
      this.$router.replace({ path: '/hurda', query: {} })
      setTimeout(() => {
        this.message = ''
      }, 2500)
    }
    await this.fetchItems()
  },
  methods: {
    setSort(key) {
      if (this.sortKey === key) {
        this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc'
      } else {
        this.sortKey = key
        this.sortDir = 'asc'
      }
    },
    async fetchItems() {
      try {
        const res = await api.get('/hurda')
        this.items = res.data
      } catch {
        this.items = []
        this.message =
          'API\'ye bağlanılamadı. Terminalde proje klasöründe `dotnet run` ile sunucuyu başlat (adres: http://localhost:5283).'
      }
    },
    startEdit(item) {
      this.editingId = item.id
      this.editScrapAmount = item.scrapAmount
    },
    cancelEdit() {
      this.editingId = null
    },
    async saveEdit(id) {
      try {
        await api.put(`/hurda/${id}`, { scrapAmount: this.editScrapAmount })
        this.message = 'Güncellendi!'
        this.editingId = null
        await this.fetchItems()
        setTimeout(() => (this.message = ''), 2000)
      } catch {
        this.message = 'Güncelleme başarısız!'
      }
    },
    formatDate(dateStr) {
      if (!dateStr) return '—'
      return new Date(dateStr).toLocaleDateString('tr-TR')
    },
    /** Beklenen: Kod: … | Kalıp: … | Cavity: … */
    parseHurdaDescription(desc) {
      const empty = { materialCode: '', moldCode: '', cavity: '' }
      if (!desc || typeof desc !== 'string') return empty
      const m = desc.match(
        /Kod:\s*(.+?)\s*\|\s*Kalıp:\s*(.+?)\s*\|\s*Cavity:\s*(.+)$/i
      )
      if (!m) return empty
      let cavity = m[3].trim()
      if (/^cavity\s*:/i.test(cavity)) {
        cavity = cavity.replace(/^cavity\s*:\s*/i, '').trim()
      }
      return {
        materialCode: m[1].trim(),
        moldCode: m[2].trim(),
        cavity,
      }
    },
    displayMaterialCode(item) {
      const c = this.parseHurdaDescription(item.description).materialCode
      return c || '—'
    },
    displayMoldCode(item) {
      const c = this.parseHurdaDescription(item.description).moldCode
      return c || '—'
    },
    displayCavity(item) {
      const c = this.parseHurdaDescription(item.description).cavity
      return c || '—'
    },
  },
}
</script>
