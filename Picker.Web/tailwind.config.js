/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{vue,js}'],
  theme: {
    extend: {
      fontFamily: { sans: ['Inter', 'system-ui', 'sans-serif'] },
      colors: {
        surface: {
          DEFAULT: 'rgba(255,255,255,0.05)',
          hover: 'rgba(255,255,255,0.08)',
          border: 'rgba(255,255,255,0.10)',
        },
      },
      backdropBlur: { xs: '2px' },
      keyframes: {
        shimmer: { '0%,100%': { opacity: 1 }, '50%': { opacity: 0.4 } },
        pop: { '0%': { transform: 'scale(0.8)', opacity: 0 }, '100%': { transform: 'scale(1)', opacity: 1 } },
        spin_slow: { '0%': { transform: 'rotate(0deg)' }, '100%': { transform: 'rotate(360deg)' } },
        float: { '0%,100%': { transform: 'translateY(0)' }, '50%': { transform: 'translateY(-8px)' } },
        glow: { '0%,100%': { boxShadow: '0 0 20px rgba(139,92,246,0.4)' }, '50%': { boxShadow: '0 0 40px rgba(139,92,246,0.9)' } },
      },
      animation: {
        shimmer: 'shimmer 2s ease-in-out infinite',
        pop: 'pop 0.3s ease-out',
        spin_slow: 'spin_slow 3s linear infinite',
        float: 'float 3s ease-in-out infinite',
        glow: 'glow 2s ease-in-out infinite',
      },
    },
  },
  plugins: [],
}
