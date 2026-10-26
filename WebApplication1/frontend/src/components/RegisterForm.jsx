import { useState } from 'react';

export default function RegisterForm() {
  const [isOpen, setIsOpen] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  async function handleRegister(event) {
    event.preventDefault();
    setError('');
    setSuccess('');

    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setLoading(true);

    try {
      const response = await fetch('/api/auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: username.trim(),
          password,
        }),
      });

      if (!response.ok) {
        const data = await response.json().catch(() => null);

        throw new Error(
          data?.message || `Registration failed (${response.status}).`
        );
      }

      setSuccess('Account created. You can now sign in.');
      setUsername('');
      setPassword('');
      setConfirmPassword('');
      setIsOpen(false);
    } catch (err) {
      setError(
        err instanceof TypeError
          ? 'Unable to connect. Check that the API is running.'
          : err instanceof Error
            ? err.message
            : 'An unexpected error occurred.'
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="info-card">
      <h2>Create an account</h2>

      {success && <p role="status">{success}</p>}

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {!isOpen ? (
        <button
          className="zoo-button"
          type="button"
          onClick={() => {
            setIsOpen(true);
            setError('');
            setSuccess('');
          }}
        >
          Register
        </button>
      ) : (
        <form className="login-form" onSubmit={handleRegister}>
          <label htmlFor="register-username">Username</label>
          <input
            id="register-username"
            name="username"
            autoComplete="username"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            maxLength={100}
            disabled={loading}
            required
          />

          <label htmlFor="register-password">Password</label>
          <input
            id="register-password"
            name="password"
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            minLength={10}
            maxLength={128}
            disabled={loading}
            required
          />

          <label htmlFor="confirm-password">Confirm password</label>
          <input
            id="confirm-password"
            name="confirmPassword"
            type="password"
            autoComplete="new-password"
            value={confirmPassword}
            onChange={(event) => setConfirmPassword(event.target.value)}
            minLength={10}
            maxLength={128}
            disabled={loading}
            required
          />

          <p>Password must contain between 10 and 128 characters.</p>

          <button
            className="zoo-button"
            type="submit"
            disabled={loading}
          >
            {loading ? 'Creating account…' : 'Create account'}
          </button>

          <button
            className="zoo-button"
            type="button"
            disabled={loading}
            onClick={() => {
              setIsOpen(false);
              setPassword('');
              setConfirmPassword('');
              setError('');
            }}
          >
            Cancel
          </button>
        </form>
      )}
    </section>
  );
}