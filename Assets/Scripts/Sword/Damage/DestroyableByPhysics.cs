using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableByPhysics : MonoBehaviour
{

    public float NeededForce = 8f;
    public float ThresholdForce = 3f;

    public float DefaultMultiplier = 0.1f;

    public bool AccumulateDamage = false;

    public float minHitCooldown = 0.2f;

    public UnityEvent OnDestroyed, OnHit;

    public DamageException[] ExceptionsFromRule;

    [System.Serializable]
    public class DamageException
    {
        public string Tag;
        public float Multiplier;
    }


    private HashSet<GameObject> AlreadyCollided = new HashSet<GameObject>();
    private int lastFrame = -1;
    private void ClearCollidedListIfDirty()
    {
        if (lastFrame == Time.frameCount) return;
        AlreadyCollided.Clear();
        lastFrame = Time.frameCount;
    }


    public double lastShotTime = 0.2f;
    private void OnCollisionEnter(Collision collision)
    {

        var time = Time.timeAsDouble;
        if (time - lastShotTime < minHitCooldown) return;
        lastShotTime = time;


        ClearCollidedListIfDirty();
        if (AlreadyCollided.Contains(collision.gameObject)) return;
        AlreadyCollided.Add(collision.gameObject);


        //var damager = GetDamager(collision);
        //var multiplier = damager?.DamageMultiplier ?? DefaultMultiplier;
        //if (damager != null) multiplier = ExceptionsFromRule.FirstOrDefault(r => damager.CompareTag(r.Tag))?.Multiplier ?? multiplier;
        var multiplier = 1f;

        var force = collision.relativeVelocity.magnitude;
        var damage = force * multiplier;

        if (damage < ThresholdForce) return;

        Debug.Log($"{collision.gameObject.name} impacted into {gameObject.name} with force {force} ({damage} dmg)");

        this.Impact(damage);
    }

    public void Impact(float force)
    {
        if (force >= NeededForce)
            DestroySelf();
        else
            DamageSelf(force);
    }

    private void DamageSelf(float force)
    {
        if (AccumulateDamage) NeededForce -= force;
        OnHit.Invoke();
    }
    public void DestroySelf()
    {
        Debug.Log($"{gameObject.name} was destroyed!");
        OnDestroyed.Invoke();
        Destroy(this.gameObject);
    }
}
