import { useState } from 'react';

export default function AddAnimalForm({ token, onAnimalAdded }) {
  const [name, setName] = useState('');
  const [species, setSpecies] = useState('');
  const [age, setAge] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');
    setSuccess('');

    if (!name.trim()) {
      setError('Name cannot be empty.');
      return;
    }

    const animalAge = Number(age);

    if (
      age === '' ||
      !Number.isInteger(animalAge) ||
      animalAge < 0 ||
      animalAge > 2147483647
    ) {
      setError('Age must be a valid non-negative whole number.');
      return;
    }

    setLoading(true);

    try {
      const response = await fetch('/api/zoo', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name: name.trim(),
          species: species.trim(),
          age: animalAge,
        }),
      });

      if (response.status === 401) {
        throw new Error('Your session has expired. Please sign in again.');
      }

      if (response.status === 403) {
        throw new Error('Only administrators can add animals.');
      }

      if (!response.ok) {
        throw new Error(
          `Failed to add animal (${response.status}). Check the entered values.`
        );
      }

      setName('');
      setSpecies('');
      setAge('');
      setSuccess('Animal added successfully.');

      await onAnimalAdded();
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
      <h2>Dodaj životinju</h2>

      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}

      {success && <p role="status">{success}</p>}

      <form className="login-form" onSubmit={handleSubmit}>
        <label htmlFor="animal-name">Ime</label>
        <input
          id="animal-name"
          value={name}
          onChange={(event) => setName(event.target.value)}
          disabled={loading}
          required
        />

        <label htmlFor="animal-species">Vrsta</label>
        <input
          id="animal-species"
          value={species}
          onChange={(event) => setSpecies(event.target.value)}
          disabled={loading}
        />

        <label htmlFor="animal-age">Dob u godinama</label>
        <input
          id="animal-age"
          type="number"
          min="0"
          max="2147483647"
          step="1"
          value={age}
          onChange={(event) => setAge(event.target.value)}
          disabled={loading}
          required
        />

        <button
          className="zoo-button"
          type="submit"
          disabled={loading}
        >
          {loading ? 'Spremanje…' : 'Dodaj životinju'}
        </button>
      </form>
    </section>
  );
}