import { StrictMode } from 'react'
import './index.css'
import App from './App.tsx'
import { BrowserRouter, Route, Routes } from 'react-router';
import { createRoot } from 'react-dom/client';
import Home from './page/Home.tsx';
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";

const queryClient = new QueryClient()

const root = document.getElementById('root')!;

createRoot(root).render(
  <StrictMode>
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <App>
          <Routes>
            <Route index element={<Home />} />
          </Routes>
        </App>
      </QueryClientProvider>
    </BrowserRouter>
  </StrictMode>
);

// https://reactrouter.com/start/declarative/routing