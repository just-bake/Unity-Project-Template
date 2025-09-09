// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Protected
{
    /// <summary>
    /// Shoots bullets when the player presses the fire button at a certain rate.
    /// </summary>
    public class PlayerShoot : MonoBehaviour
    {
        /// <summary>
        /// The bullet prefab.
        /// </summary>
        public GameObject bullet;

        /// <summary>
        /// The fire rate of the player.
        /// </summary>
        private float fireRate = 0.5f;

        /// <summary>
        /// The next time the player can fire.
        /// </summary>
        private float nextFire = 0.0f;

        /// <summary>
        /// Fire a bullet when the player presses the fire button in a certain interval.
        /// </summary>
        private void Update()
        {
            if (Input.GetButton("Fire1") && UnityEngine.Time.time > this.nextFire)
            {
                this.nextFire = UnityEngine.Time.time + this.fireRate;
                this.Fire();
            }
        }

        /// <summary>
        /// When a player fires, create a bullet.
        /// </summary>
        private void Fire()
        {
            // Create a bullet.
            Vector3 bulletPosition = this.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            Instantiate(this.bullet, bulletPosition, Quaternion.identity);
        }
    }
}
