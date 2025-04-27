import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './index.css'

// Import debug utilities to make them available globally
import './lib/debug-toggle.js'

ReactDOM.createRoot(document.getElementById('root')).render(
    <App />
);
