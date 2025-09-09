// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_DataChain
{
    /// <summary>
    /// A very simple player movement script that allows the player to move left and right and jump.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// The maximum speed the player can move.
        /// </summary>
        public float MaxSpeed = 5.0f;

        /// <summary>
        /// The maximum force the player can jump.
        /// </summary>
        public float MaxJumpForce = 15.0f;

        /// <summary>
        /// If the player is currently jumping.
        /// </summary>
        private bool isJumping = false;

        /// <summary>
        /// The current jump force reducing over time.
        /// </summary>
        private float currentJumpForce = 0.0f;

        /// <summary>
        /// On update, check for input and move the player.
        /// </summary>
        private void Update()
        {
            // Get the input direction.
            float direction = 0.0f;

            if (Input.GetKey(KeyCode.A))
            {
                direction = -1.0f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction = 1.0f;
            }

            // Calculate movement.
            Vector3 movement = new Vector3(direction, 0, 0) * MaxSpeed * UnityEngine.Time.deltaTime;

            // Check if at the edge of the map.
            if(this.transform.position.x < -10.0f && direction < 0.0f)
            {
                movement = Vector3.zero;
            }
            else if(this.transform.position.x > 10.0f && direction > 0.0f)
            {
                movement = Vector3.zero;
            }

            // Check if can move in the direction.
            if (!Physics.Raycast(this.transform.position + Vector3.up / 2f, movement, 0.65f) && !Physics.Raycast(this.transform.position - Vector3.up / 2f, movement, 0.65f))
            {
                this.transform.position += movement;
            }

            // Check if on ground, else apply gravity.
            if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 1.05f))
            {
                if(hit.collider.isTrigger)
                {
                    this.transform.position += Vector3.down * 8f * UnityEngine.Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }
            else
            {
                this.transform.position += Vector3.down * 8f * UnityEngine.Time.deltaTime;
            }

            // Jump!
            if (Input.GetKeyDown(KeyCode.W) && !isJumping)
            {
                if(Physics.Raycast(this.transform.position, Vector3.down, 1.1f))
                {
                    isJumping = true;

                    currentJumpForce = MaxJumpForce;
                }
            }

            // While jumping, apply force and reduce it over time.
            if (isJumping)
            {
                currentJumpForce -= 8f * UnityEngine.Time.deltaTime;

                if (currentJumpForce < 0.0f)
                {
                    currentJumpForce = 0.0f;
                }

                this.transform.position += Vector3.up * currentJumpForce * UnityEngine.Time.deltaTime;
            }
        }
    }
}