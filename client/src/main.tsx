import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import { PhienProvider } from './Phien';
import { BaoProvider } from './Bao';
import './site.css';

ReactDOM.createRoot(document.getElementById('goc')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <PhienProvider>
        <BaoProvider>
          <App />
        </BaoProvider>
      </PhienProvider>
    </BrowserRouter>
  </React.StrictMode>,
);
