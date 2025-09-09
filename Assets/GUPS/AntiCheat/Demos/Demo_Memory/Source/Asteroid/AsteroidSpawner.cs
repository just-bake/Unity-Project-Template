// System
using System.Collections;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{    
    /// <summary>
    /// Spawns asteroids at a regular interval at the top of the screen.
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        /// <summary>
        /// The prefabs representing the asteroids to be spawned.
        /// </summary>
        public GameObject[] Asteroids;

        /// <summary>
        /// The rate at which asteroids are spawned, in seconds.
        /// </summary>
        public float SpawnRate = 1f;

        /// <summary>
        /// Start a coroutine to spawn asteroids at a regular interval.
        /// </summary>
        private void Start()
        {
            // Start the coroutine to spawn asteroids.
            this.StartCoroutine(this.Spawn());
        }

        /// <summary>
        /// Spawns asteroids at a regular interval.
        /// </summary>
        private IEnumerator Spawn()
        {
            // Run indefinitely.
            while (true)
            {
                // Wait for the specified amount of time.
                yield return new WaitForSeconds(this.SpawnRate);

                // Spawn a random asteroid.
                GameObject asteroid = this.Asteroids[Random.Range(0, this.Asteroids.Length)];

                // Get a random position within the screen bounds.
                Vector3 position = new Vector3(Random.Range(-8, 8), 6, 2);

                // Instantiate the asteroid at the random position.
                Instantiate(asteroid, position, Quaternion.identity);
            }
        }
    }
}
