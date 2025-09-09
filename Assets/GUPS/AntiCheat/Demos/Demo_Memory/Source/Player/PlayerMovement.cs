// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// Moves the player left and right based on the horizontal input.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The speed of the player flying to the left and right.
        /// </summary>
        private float speed = 7.5f;

        /// <summary>
        /// Update the player movement by getting the horizontal input and moving the player to the left or right.
        /// </summary>
        private void Update()
        {
            // Get the horizontal and vertical input.
            float horizontal = Input.GetAxis("Horizontal");

            // Calculate the movement direction.
            Vector3 direction = new Vector3(horizontal, 0.0f, 0.0f);

            // Calculate the next position of the player.
            Vector3 nextPosition = this.transform.position + direction * this.speed * UnityEngine.Time.deltaTime;

            // Clamp the next position to the screen bounds.
            nextPosition.x = Mathf.Clamp(nextPosition.x, -8.5f, 8.5f);

            // Move the player in the calculated direction.
            this.transform.position = nextPosition;
        }
    }
}
