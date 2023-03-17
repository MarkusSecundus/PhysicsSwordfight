using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CollisionFix : MonoBehaviour
{
    [SerializeField]
    ContrarianCollider.Configuration ColliderConfig = ContrarianCollider.Configuration.Default;

    [SerializeField] private Vector3 TriggerArea = new Vector3(4, 4, 4);

    public IReadOnlyList<Collider> AllColliders { get; private set; }
    private Rigidbody Rigidbody { get; set; }
    private SwordDescriptor SwordDescriptor { get; set; }

    private CollisionFixManager manager;


    private void Awake()
    {
        this.Rigidbody = GetComponent<Rigidbody>();
        this.SwordDescriptor = GetComponent<SwordDescriptor>();
        this.AllColliders = GetComponentsInChildren<Collider>();

        //transform.CreateChild("trigger").AddComponent<CallbackedTrigger>()
        //    .Add<BoxCollider>(c => c.size = TriggerArea)
        //    .Init(ColliderLayers.CollisionFix, onEnter: AreaEntered, onExit: AreaExited);

        manager = GameObjectUtils.GetUtilComponent<CollisionFixManager>();
        manager.Register(this);
    }

    private void OnDestroy()
    {
        manager.Unregister(this);
    }


    void AreaEntered(Collider collider)
    {
        Debug.Log($"Entered!");
    }
    void AreaExited(Collider collider)
    {
        Debug.Log($"left!");
    }

    private void SetIgnoreCollisions(Collider collider, bool ignoreCollisions=true)
    {
        foreach (var c in AllColliders) Physics.IgnoreCollision(c, collider, ignoreCollisions);
    }

    public class Fixer : ContrarianColliderBase
    {
        public SwordDescriptor Host, Target;

        protected override ScaledRay GetHost() => Host.SwordBladeAsRay();
        protected override ScaledRay GetTarget() => Target.SwordBladeAsRay();
    
        public Fixer Init(CollisionFix host, CollisionFix target)
        {
            (this.Host, this.Target) = (host.SwordDescriptor, target.SwordDescriptor);
            this.Config = host.ColliderConfig;
            this.SetUp(host.Rigidbody);
            host.SetIgnoreCollisions(this.collider);
            return this;
        }
    }

}
