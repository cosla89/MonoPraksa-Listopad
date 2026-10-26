import { useState } from 'react';

export default function Animals() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState('');
  const [animals, setAnimals] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [loaded, setLoaded] = useState(false);

  async function loadAnimals(accessToken) {
    const response = await fetch('/api/zoo', {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    if (response.status === 401) {
      setToken('');
      setAnimals([]);
      setLoaded(false);

      throw new Error('Your session has expired. Please sign in again.');
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

    setAnimals(data);
    setLoaded(true);
  }

  function getErrorMessage(err) {
    if (err instanceof TypeError) {
      return 'Unable to connect. Check that the API is running.';
    }

    return err instanceof Error
      ? err.message
      : 'An unexpected error occurred.';
  }

  async function handleLogin(event) {
    event.preventDefault();

    setLoading(true);
    setError('');
    setLoaded(false);

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
        throw new Error(
          `Sign-in failed (${response.status}).`
        );
      }

      const data = await response.json();

      if (typeof data.token !== 'string' || !data.token) {
        throw new Error('The sign-in response does not contain a token.');
      }

      setToken(data.token);
      setPassword('');

      await loadAnimals(data.token);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }

  async function handleRefresh() {
    setLoading(true);
    setError('');

    try {
      await loadAnimals(token);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }

  function handleLogout() {
    setToken('');
    setPassword('');
    setAnimals([]);
    setLoaded(false);
    setError('');
  }

  return (
    <main className="home">
      <p className="subtitle">Meet our zoo residents</p>
      <h1>Animals</h1>

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {!token ? (
        <section className="info-card">
          <h2>Sign in</h2>
          <p>Sign in with your Zoo account to view the animals.</p>

          <form className="login-form" onSubmit={handleLogin}>
            <label htmlFor="username">Username</label>
            <input
              id="username"
              name="username"
              autoComplete="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              disabled={loading}
              required
            />

            <label htmlFor="password">Password</label>
            <input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              disabled={loading}
              required
            />

            <button
              className="zoo-button"
              type="submit"
              disabled={loading}
            >
              {loading ? 'Signing in…' : 'Sign in'}
            </button>
          </form>
        </section>
      ) : (
        <>
          <div className="animal-actions">
            <button
              className="zoo-button"
              onClick={handleRefresh}
              disabled={loading}
            >
              Refresh list
            </button>

            <button
              className="zoo-button"
              onClick={handleLogout}
              disabled={loading}
            >
              Sign out
            </button>
          </div>

          {loaded && (
            <p>Total animals: {animals.length}</p>
          )}

          {loaded && animals.length === 0 && (
            <p>No animals found.</p>
          )}

          <div className="animal-grid">
            {animals.map((animal) => (
              <article className="animal-card" key={animal.id}>
                <h2>{animal.name}</h2>
                <p>Species: {animal.species}</p>
                <p>Age: {animal.age}</p>
              </article>
            ))}
          </div>
        </>
      )}

      {loading && <p role="status">Loading…</p>}
    </main>
  );
}