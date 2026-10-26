import {
  BrowserRouter,
  NavLink,
  Route,
  Routes,
} from 'react-router';

import Home from './Pages/Home';
import Animals from './Pages/Animals';
import './App.css';

export default function App() {
  return (
    <BrowserRouter>
      <header className="site-header">
        <NavLink to="/" className="logo">
          Zoo
        </NavLink>

        <nav className="site-nav" aria-label="Glavna navigacija">
          <NavLink to="/" end>
            Početna
          </NavLink>

          <NavLink to="/animals">
            Životinje
          </NavLink>
        </nav>
      </header>

      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/animals" element={<Animals />} />

        <Route
          path="*"
          element={
            <main className="home">
              <h1>Stranica nije pronađena</h1>
              <NavLink to="/">Povratak na početnu</NavLink>
            </main>
          }
        />
      </Routes>

      <footer className="site-footer">
        Zoo — projekt za učenje Reacta
      </footer>
    </BrowserRouter>
  );
}