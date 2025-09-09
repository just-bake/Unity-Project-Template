// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// A bullet shoot by the player that can destroy asteroids. If a bullet collides with an asteroid, the score increases.
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        /// <summary>
        /// The score manager.
        /// </summary>
        private ScoreManager scoreManager;

        /// <summary>
        /// The speed of the bullet.
        /// </summary>
        private float speed = 3f;

        /// <summary>
        /// On start find the score manager.
        /// </summary>
        private void Start()
        {
            // Find the score manager.
            this.scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }

        /// <summary>
        /// Move the bullet upwards and destroy it if it's out of the screen.
        /// </summary>
        private void Update()
        {
            // Move the bullet upwards.
            this.transform.position += Vector3.up * this.speed * UnityEngine.Time.deltaTime;

            // If the bullet is out of the screen, destroy it.
            if (this.transform.position.y > 6f)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Destroy the bullet and the asteroid if they collide and increase the score.
        /// </summary>
        /// <param name="collider">The other collider.</param>
        private void OnTriggerEnter(Collider collider)
        {
            // If the bullet collides with an asteroid, destroy both the bullet and the asteroid.
            if (collider.gameObject.name.StartsWith("Asteroid"))
            {
                // Destroy both the bullet and the asteroid.
                Destroy(collider.gameObject);
                Destroy(this.gameObject);

                // Increase the score.
                this.scoreManager.IncreaseScore();
            }
        }
    }
}
