import { useEffect, useState } from 'react';
import RegisterForm from '../components/RegisterForm';
import AddAnimalForm from '../components/AddAnimalForm';

const PAGE_SIZE = 4;

export default function Animals() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const [token, setToken] = useState(
    () => localStorage.getItem('zoo_token') ?? ''
  );

  const [role, setRole] = useState(
    () => localStorage.getItem('zoo_role') ?? ''
  );

  const [animals, setAnimals] = useState([]);
  const [loading, setLoading] = useState(false);
  const [busy, setBusy] = useState(false);
  const [loaded, setLoaded] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  const [page, setPage] = useState(1);
  const [refreshVersion, setRefreshVersion] = useState(0);
  const [editingAnimal, setEditingAnimal] = useState(null);

  const isAdmin = role === 'Admin';

  const totalPages = Math.max(
    1,
    Math.ceil(animals.length / PAGE_SIZE)
  );

  const currentPage = Math.min(page, totalPages);
  const startIndex = (currentPage - 1) * PAGE_SIZE;

  const visibleAnimals = animals.slice(
    startIndex,
    startIndex + PAGE_SIZE
  );

  function clearSession() {
    localStorage.removeItem('zoo_token');
    localStorage.removeItem('zoo_role');

    setToken('');
    setRole('');
    setPassword('');
    setAnimals([]);
    setLoaded(false);
    setEditingAnimal(null);
    setPage(1);
    setMessage('');
  }

  function getErrorMessage(err) {
    if (err instanceof TypeError) {
      return 'Unable to connect. Check that the API is running.';
    }

    return err instanceof Error
      ? err.message
      : 'An unexpected error occurred.';
  }

  // Dohvat se pokreće nakon prijave, povratka na stranicu
  // i ručnog osvježavanja popisa.
  useEffect(() => {
    if (!token) {
      return;
    }

    const controller = new AbortController();

    async function fetchAnimals() {
      setLoading(true);
      setError('');

      try {
        const response = await fetch('/api/zoo', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
          signal: controller.signal,
        });

        if (controller.signal.aborted) {
          return;
        }

        if (response.status === 401) {
          clearSession();
          throw new Error(
            'Your session has expired. Please sign in again.'
          );
        }

        if (!response.ok) {
          throw new Error(
            `Failed to load animals (${response.status}).`
          );
        }

        const data = await response.json();

        if (!Array.isArray(data)) {
          throw new Error('The API returned an unexpected response.');
        }

        if (!controller.signal.aborted) {
          setAnimals(data);
          setLoaded(true);
        }
      } catch (err) {
        if (!controller.signal.aborted) {
          setError(getErrorMessage(err));
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false);
        }
      }
    }

    fetchAnimals();

    return () => controller.abort();
  }, [token, refreshVersion]);

  async function handleLogin(event) {
    event.preventDefault();

    setBusy(true);
    setError('');
    setMessage('');

    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: username.trim(),
          password,
        }),
      });

      if (response.status === 401) {
        throw new Error('Invalid username or password.');
      }

      if (!response.ok) {
        throw new Error(`Sign-in failed (${response.status}).`);
      }

      const data = await response.json();

      if (typeof data.token !== 'string' || !data.token) {
        throw new Error(
          'The sign-in response does not contain a token.'
        );
      }

      localStorage.setItem('zoo_token', data.token);
      localStorage.setItem('zoo_role', data.role ?? '');

      setRole(data.role ?? '');
      setPassword('');
      setLoaded(false);
      setPage(1);
      setToken(data.token);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setBusy(false);
    }
  }

  function handleLogout() {
    clearSession();
    setError('');
  }

  function handleRefresh() {
    setRefreshVersion((value) => value + 1);
  }

  async function checkMutationResponse(response) {
    if (response.status === 401) {
      clearSession();
      throw new Error(
        'Your session has expired. Please sign in again.'
      );
    }

    if (response.status === 403) {
      throw new Error(
        'Only administrators can modify animals.'
      );
    }

    if (response.status === 404) {
      handleRefresh();
      throw new Error('Animal not found. Refreshing the list.');
    }

    if (!response.ok) {
      throw new Error(
        `Request failed (${response.status}). Check the entered values.`
      );
    }
  }

  function startEditing(animal) {
    setError('');
    setMessage('');

    // Kopija omogućuje uređivanje bez promjene postojećeg popisa.
    setEditingAnimal({
      id: animal.id,
      name: animal.name,
      species: animal.species,
      age: String(animal.age),
    });
  }

  async function handleUpdate(event) {
    event.preventDefault();
    setError('');
    setMessage('');

    const age = Number(editingAnimal.age);

    if (!editingAnimal.name.trim()) {
      setError('Name cannot be empty.');
      return;
    }

    if (
      editingAnimal.age === '' ||
      !Number.isInteger(age) ||
      age < 0 ||
      age > 2147483647
    ) {
      setError('Age must be a valid non-negative whole number.');
      return;
    }

    setBusy(true);

    try {
      const response = await fetch(
        `/api/zoo/${editingAnimal.id}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            id: editingAnimal.id,
            name: editingAnimal.name.trim(),
            species: editingAnimal.species.trim(),
            age,
          }),
        }
      );

      await checkMutationResponse(response);

      setEditingAnimal(null);
      setMessage('Animal updated successfully.');
      handleRefresh();
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setBusy(false);
    }
  }

  async function handleDelete(animal) {
    const confirmed = window.confirm(
      `Želiš li obrisati životinju "${animal.name}"?`
    );

    if (!confirmed) {
      return;
    }

    setBusy(true);
    setError('');
    setMessage('');

    try {
      const response = await fetch(`/api/zoo/${animal.id}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      await checkMutationResponse(response);

      setAnimals((previous) =>
        previous.filter((item) => item.id !== animal.id)
      );

      if (editingAnimal?.id === animal.id) {
        setEditingAnimal(null);
      }

      setMessage('Animal deleted successfully.');
      handleRefresh();
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setBusy(false);
    }
  }

  function changePage(nextPage) {
    setPage(nextPage);
    setEditingAnimal(null);
    setError('');
    setMessage('');
  }

  return (
    <main className="home">
      <p className="subtitle">Upoznaj stanovnike našeg zoološkog vrta</p>
      <h1>Životinje</h1>

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {message && <p role="status">{message}</p>}

      {!token ? (
        <>
          <section className="info-card">
            <h2>Sign in</h2>

            <form className="login-form" onSubmit={handleLogin}>
              <label htmlFor="username">Username</label>
              <input
                id="username"
                autoComplete="username"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
                disabled={busy}
                required
              />

              <label htmlFor="password">Password</label>
              <input
                id="password"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                disabled={busy}
                required
              />

              <button
                className="zoo-button"
                type="submit"
                disabled={busy}
              >
                {busy ? 'Signing in…' : 'Sign in'}
              </button>
            </form>
          </section>

          <RegisterForm />
        </>
      ) : (
        <>
          <div className="animal-actions">
            <button
              className="zoo-button"
              onClick={handleRefresh}
              disabled={loading || busy}
            >
              Osvježi popis
            </button>

            <button
              className="zoo-button"
              onClick={handleLogout}
              disabled={loading || busy}
            >
              Sign out
            </button>
          </div>

          {loading && <p role="status">Loading…</p>}

          {loaded && <p>Ukupno životinja: {animals.length}</p>}

          {loaded && animals.length === 0 && (
            <p>Trenutno nema životinja.</p>
          )}

          <div className="animal-grid">
            {visibleAnimals.map((animal) => (
              <article className="animal-card" key={animal.id}>
                <h2>{animal.name}</h2>
                <p>Vrsta: {animal.species}</p>
                <p>Dob: {animal.age}</p>

                {isAdmin && (
                  <div className="animal-actions">
                    <button
                      className="zoo-button"
                      onClick={() => startEditing(animal)}
                      disabled={busy || loading}
                    >
                      Uredi
                    </button>

                    <button
                      className="zoo-button danger-button"
                      onClick={() => handleDelete(animal)}
                      disabled={busy || loading}
                    >
                      Obriši
                    </button>
                  </div>
                )}
              </article>
            ))}
          </div>

          {loaded && animals.length > 0 && (
            <nav className="pagination" aria-label="Stranice životinja">
              <button
                className="zoo-button"
                onClick={() => changePage(currentPage - 1)}
                disabled={currentPage === 1 || busy || loading}
              >
                ← Prethodna
              </button>

              {Array.from({ length: totalPages }, (_, index) => {
                const pageNumber = index + 1;

                return (
                  <button
                    key={pageNumber}
                    className="zoo-button page-button"
                    aria-label={`Stranica ${pageNumber}`}
                    aria-current={
                      currentPage === pageNumber ? 'page' : undefined
                    }
                    onClick={() => changePage(pageNumber)}
                    disabled={busy || loading}
                  >
                    {pageNumber}
                  </button>
                );
              })}

              <button
                className="zoo-button"
                onClick={() => changePage(currentPage + 1)}
                disabled={
                  currentPage === totalPages || busy || loading
                }
              >
                Sljedeća →
              </button>

              <p className="page-summary">
                Stranica {currentPage} od {totalPages}
              </p>
            </nav>
          )}

          {isAdmin && editingAnimal && (
            <section className="info-card">
              <h2>Uredi životinju</h2>

              <form className="login-form" onSubmit={handleUpdate}>
                <label htmlFor="edit-name">Ime</label>
                <input
                  id="edit-name"
                  value={editingAnimal.name}
                  onChange={(event) =>
                    setEditingAnimal({
                      ...editingAnimal,
                      name: event.target.value,
                    })
                  }
                  disabled={busy}
                  required
                />

                <label htmlFor="edit-species">Vrsta</label>
                <input
                  id="edit-species"
                  value={editingAnimal.species}
                  onChange={(event) =>
                    setEditingAnimal({
                      ...editingAnimal,
                      species: event.target.value,
                    })
                  }
                  disabled={busy}
                />

                <label htmlFor="edit-age">Dob u godinama</label>
                <input
                  id="edit-age"
                  type="number"
                  min="0"
                  max="2147483647"
                  step="1"
                  value={editingAnimal.age}
                  onChange={(event) =>
                    setEditingAnimal({
                      ...editingAnimal,
                      age: event.target.value,
                    })
                  }
                  disabled={busy}
                  required
                />

                <button
                  className="zoo-button"
                  type="submit"
                  disabled={busy || loading}
                >
                  {busy ? 'Spremanje…' : 'Spremi promjene'}
                </button>

                <button
                  className="zoo-button"
                  type="button"
                  onClick={() => setEditingAnimal(null)}
                  disabled={busy}
                >
                  Odustani
                </button>
              </form>
            </section>
          )}

          {isAdmin && !editingAnimal && (
            <AddAnimalForm
              token={token}
              onAnimalAdded={handleRefresh}
            />
          )}
        </>
      )}
    </main>
  );
}