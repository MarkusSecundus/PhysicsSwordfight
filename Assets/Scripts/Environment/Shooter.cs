using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment
{
    /// <summary>
    /// Object that can shoot projectiles
    /// </summary>
    public class Shooter : MonoBehaviour
    {
        /// <summary>
        /// Prototype of the projectile to be shot
        /// </summary>
        public Rigidbody Projectile;

        /// <summary>
        /// Force impulse to apply to the projectile when shooting it
        /// </summary>
        public Vector3 shootForce;
        /// <summary>
        /// How many projectiles max can be in the pool
        /// </summary>
        public int maxProjectileInExistenceCount = 999;

        /// <summary>
        /// Sound variants to play on shot
        /// </summary>
        public AudioClip[] ShootSounds;

        /// <summary>
        /// Pool of shot projectiles
        /// </summary>
        private Queue<GameObject> Projectiles = new Queue<GameObject>();

        private AudioSource audioSrc;
        private void Start()
        {
            audioSrc = GetComponent<AudioSource>();
        }


        private void OnDestroy()
        {
            while (Projectiles.Count > 0) Destroy(Projectiles.Dequeue());
        }

        /// <summary>
        /// Shoot a projectile
        /// </summary>
        public void DoShoot()
        {
            if (!ShootSounds.IsNullOrEmpty()) audioSrc.PlayOneShot(ShootSounds.RandomElement());
            var projectile = Projectile.gameObject.InstantiateWithTransform();
            Projectiles.Enqueue(projectile);
            while (Projectiles.Count > maxProjectileInExistenceCount) Destroy(Projectiles.Dequeue());
            var rb = projectile.GetComponent<Rigidbody>();
            rb.AddRelativeForce(shootForce, ForceMode.Impulse);
        }
    }
}