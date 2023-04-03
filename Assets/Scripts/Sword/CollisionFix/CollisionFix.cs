using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CollisionFix : MonoBehaviour
{
    public static int TargetLayer => ColliderLayers.CollisionFix;

    [SerializeField]
    ContrarianCollider.Configuration ColliderConfig = ContrarianCollider.Configuration.Default;

    [SerializeField] private float ActivationRadius = 3f;

    public IReadOnlyList<Collider> AllColliders { get; private set; }
    private Rigidbody Rigidbody { get; set; }
    private SwordDescriptor SwordDescriptor { get; set; }

    private CollisionFixManager manager;

    private CallbackedTrigger InstanceCreator;
    private void Awake()
    {
        this.Rigidbody = GetComponent<Rigidbody>();
        this.SwordDescriptor = GetComponent<SwordDescriptor>();
        this.AllColliders = GetComponentsInChildren<Collider>();

        InstanceCreator = transform.CreateChild("trigger").AddComponent<CallbackedTrigger>()
            .Add<SphereCollider>(c => c.radius = ActivationRadius)
            .Init(CollisionFix.TargetLayer, onEnter: AreaEntered);

        manager = GameObjectUtils.GetUtilComponent<CollisionFixManager>();
        manager.Register(this);
    }

    private void OnDestroy()
    {
        manager.Unregister(this);
    }

    void AreaEntered(Collider collider)
    {
        if(manager.TryFindFixer(collider, out var other) && other != this)
        {
            manager.EnableFixer(this, other);
            foreach (var c in InstanceCreator.Colliders) other.SetIgnoreCollisions(c);
        }
        else
        {
            foreach (var c in InstanceCreator.Colliders) Physics.IgnoreCollision(c, collider);
        }
    }

    private void SetIgnoreCollisions(Collider collider, bool ignoreCollisions=true)
    {
        foreach (var c in AllColliders) Physics.IgnoreCollision(c, collider, ignoreCollisions);
    }


    public class Fixer : ContrarianColliderBase
    {
        public CollisionFix Host, Target;

        protected override ScaledRay GetHost() => Host.SwordDescriptor.SwordAsRay();
        protected override ScaledRay GetTarget() => Target.SwordDescriptor.SwordAsRay();

        public void SetIgnoreCollisions(CollisionFix fix) => fix.SetIgnoreCollisions(this.collider);

        private SphereCollider trigger;
        public Fixer Init(CollisionFix host, CollisionFix target)
        {
            (this.Host, this.Target) = (host, target);
            this.Config = host.ColliderConfig;
            this.SetUp(host.Rigidbody);
            this.SetIgnoreCollisions(host);
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = host.ActivationRadius;
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(Host.manager.TryFindFixer(other, out var justHit))
            {
                if (justHit != Target)
                    justHit.SetIgnoreCollisions(this.collider);
                justHit.SetIgnoreCollisions(trigger);
            }
            else
            {
                Physics.IgnoreCollision(trigger, other);
            }
        }
    }

}
