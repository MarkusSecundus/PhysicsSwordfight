using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject Projectile;

    public bool deactivateProjectileTemplate = true;

    public Vector3 shootForce;
    public ForceMode shootMode = ForceMode.VelocityChange;

    public int maxProjectileInExistenceCount = 999;

    public AudioClip[] ShootSounds;

    private Queue<GameObject> Projectiles = new Queue<GameObject>();

    private AudioSource audioSrc;
    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        if(deactivateProjectileTemplate)
            Projectile.SetActive(false);
    }


    private void OnDestroy()
    {
        while (Projectiles.Count > 0) Destroy(Projectiles.Dequeue());
    }


    public void DoShoot()
    {
        if (ShootSounds.IsNotNullNotEmpty()) audioSrc.PlayOneShot(ShootSounds.RandomElement());
        var projectile = Projectile.InstantiateWithTransform();
        Projectiles.Enqueue(projectile);
        while (Projectiles.Count > maxProjectileInExistenceCount) Destroy(Projectiles.Dequeue());
        var rb = projectile.GetComponent<Rigidbody>();
        rb.AddRelativeForce(shootForce, shootMode);
    }
}
