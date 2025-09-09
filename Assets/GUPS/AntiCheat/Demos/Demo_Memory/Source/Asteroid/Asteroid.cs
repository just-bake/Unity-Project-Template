// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// An random sized asteroid that moves downwards, starting from the top of the screen, and rotates. If a bullet collides with the asteroid, 
    /// the score increases. Else if the player collides with the asteroid, the score decreases.
    /// </summary>
    public class Asteroid : MonoBehaviour
    {
        /// <summary>
        /// The score manager.
        /// </summary>
        private ScoreManager scoreManager;

        /// <summary>
        /// The random speed of the asteroid.
        /// </summary>
        private float speed = 1f;

        /// <summary>
        /// The random rotation speed of the asteroid.
        /// </summary>
        private float rotationSpeed = 1f;

        /// <summary>
        /// Set the size, speed and rotation speed of the asteroid.
        /// </summary>
        private void Awake()
        {
            // Set a random size for the asteroid.
            float scale = Random.Range(0.4f, 0.6f);

            // Set the scale of the asteroid.
            this.transform.localScale = new Vector3(scale, scale, scale);

            // Randomize the speed of the asteroid.
            this.speed = Random.Range(0.5f, 1.5f);

            // Randomize the rotation speed of the asteroid.
            this.rotationSpeed = Random.Range(5f, 15f);
        }

        /// <summary>
        /// On start find the score manager.
        /// </summary>
        private void Start()
        {
            // Find the score manager.
            this.scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }

        /// <summary>
        /// Move the asteroid downwards and rotate it.
        /// </summary>
        private void Update()
        {
            // Move the asteroid downwards.
            this.transform.position += Vector3.down * this.speed * UnityEngine.Time.deltaTime;

            // Rotate the asteroid.
            this.transform.Rotate(Vector3.forward, this.rotationSpeed * UnityEngine.Time.deltaTime);

            // If the asteroid is out of the screen, destroy it.
            if (this.transform.position.y < -6f)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Destroy the asteroid if it collides witht the player and decrease the score.
        /// </summary>
        /// <param name="collider">The other collider.</param>
        private void OnTriggerEnter(Collider collider)
        {
            // If the asteroid collides with the player, destroy the asteroid.
            if (collider.gameObject.name.StartsWith("Player"))
            {
                // Destroy the asteroid.
                Destroy(this.gameObject);

                // Decrease the score.
                this.scoreManager.DecreaseScore();
            }
        }
    }
}
