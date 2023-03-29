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


    public AudioClip[] soundsOnDamaged, soundsOnDestroyed;


    private MeshRenderer[] rend;

    private Color colorOnDamaged = Color.red;
    private const float damagedBlinkSegmentDuration = 0.1f, damagedNonBlinkSegmentDuration=0.1f;

    private AudioSource audioSrc;

    public DamageException[] ExceptionsFromRule;

    [System.Serializable]
    public class DamageException
    {
        public string Tag;
        public float Multiplier;
    }

    private void Start()
    {
        rend = GetComponentsInChildren<MeshRenderer>();
        audioSrc = GetComponent<AudioSource>();
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

    /*private PhysicsDamager GetDamager(Collision collision)
    {
        return collision.collider.gameObject.GetComponent<PhysicsDamager>() 
            ?? collision.gameObject.GetComponent<PhysicsDamager>();
    }*/


    public void Impact(float force)
    {
        if (force >= NeededForce)
            DestroySelf();
        else
            DamageSelf(force);
    }

    private void DamageSelf(float force)
    {
        StartCoroutine(impl());
        IEnumerator<YieldInstruction> impl()
        {
            if (soundsOnDamaged.IsNotNullNotEmpty() && audioSrc != null) audioSrc.PlayOneShot(soundsOnDamaged.RandomElement());
            if (AccumulateDamage) NeededForce -= force;
            OnHit.Invoke();
            foreach (var b in doDamagedBlink(1)) yield return b;
        }
    }
    public void DestroySelf()
    {
        StartCoroutine(impl());
        IEnumerator<YieldInstruction> impl()
        {
            if (soundsOnDestroyed.IsNotNullNotEmpty() && audioSrc != null) audioSrc.PlayOneShot(soundsOnDestroyed
                .RandomElement());
            foreach (var b in doDamagedBlink(3)) yield return b;
            Debug.Log($"{gameObject.name} was destroyed!");
            OnDestroyed.Invoke();
            Destroy(this.gameObject);
        }
    }

    private bool isBeingDestroyed = false;

    private IEnumerable<YieldInstruction> doDamagedBlink(int segments)
    {
        if (isBeingDestroyed) yield break;
        isBeingDestroyed = true;
        var originalColor = rend[0].material.color;
        for(; --segments>= 0;)
        {
            foreach(var r in rend)r.material.color = colorOnDamaged;
            yield return new WaitForSeconds(damagedBlinkSegmentDuration);
            foreach (var r in rend) r.material.color = originalColor;
            if (segments > 0)
                yield return new WaitForSeconds(damagedNonBlinkSegmentDuration);
        }
        isBeingDestroyed = false;
    }
}
