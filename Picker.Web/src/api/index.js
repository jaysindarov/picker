import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5139',
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

export const authApi = {
  register: (data) => api.post('/api/auth/register', data),
  login:    (data) => api.post('/api/auth/login', data),
  me:       ()     => api.get('/api/auth/me'),
}

export const moviesApi = {
  getAll:   (genreId)     => api.get('/api/movies', { params: genreId ? { genreId } : {} }),
  getById:  (id)          => api.get(`/api/movies/${id}`),
  getRandom:(genreId)     => api.get('/api/movies/random', { params: genreId ? { genreId } : {} }),
  create:   (data)        => api.post('/api/movies', data),
  update:   (id, data)    => api.put(`/api/movies/${id}`, data),
  delete:   (id)          => api.delete(`/api/movies/${id}`),
}

export const booksApi = {
  getAll:   (genreId)     => api.get('/api/books', { params: genreId ? { genreId } : {} }),
  getById:  (id)          => api.get(`/api/books/${id}`),
  getRandom:(genreId)     => api.get('/api/books/random', { params: genreId ? { genreId } : {} }),
  create:   (data)        => api.post('/api/books', data),
  update:   (id, data)    => api.put(`/api/books/${id}`, data),
  delete:   (id)          => api.delete(`/api/books/${id}`),
}

export const foodsApi = {
  getAll:   (cuisineId)   => api.get('/api/foods', { params: cuisineId ? { cuisineId } : {} }),
  getById:  (id)          => api.get(`/api/foods/${id}`),
  getRandom:(cuisineId)   => api.get('/api/foods/random', { params: cuisineId ? { cuisineId } : {} }),
  create:   (data)        => api.post('/api/foods', data),
  update:   (id, data)    => api.put(`/api/foods/${id}`, data),
  delete:   (id)          => api.delete(`/api/foods/${id}`),
}

export const genresApi = {
  getAll: ()           => api.get('/api/genres'),
  create: (data)       => api.post('/api/genres', data),
  update: (id, data)   => api.put(`/api/genres/${id}`, data),
  delete: (id)         => api.delete(`/api/genres/${id}`),
}

export const cuisinesApi = {
  getAll: ()           => api.get('/api/cuisines'),
  create: (data)       => api.post('/api/cuisines', data),
  update: (id, data)   => api.put(`/api/cuisines/${id}`, data),
  delete: (id)         => api.delete(`/api/cuisines/${id}`),
}

export const commentsApi = {
  getByItem: (itemId, categoryType) => api.get('/api/comments', { params: { itemId, categoryType } }),
  create:    (data)                 => api.post('/api/comments', data),
  update:    (id, data)             => api.put(`/api/comments/${id}`, data),
  delete:    (id)                   => api.delete(`/api/comments/${id}`),
}

export const ratingsApi = {
  getMyRating: (itemId, categoryType) => api.get('/api/ratings', { params: { itemId, categoryType } }),
  upsert:      (data)                 => api.post('/api/ratings', data),
}

export const adminApi = {
  getUsers:   ()               => api.get('/api/admin/users'),
  assignRole: (userId, role)   => api.post(`/api/admin/users/${userId}/assign-role`, { role }),
}
