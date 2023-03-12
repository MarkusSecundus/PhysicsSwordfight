using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionFix : MonoBehaviour
{
    [SerializeField] private SwordDescriptor sword;

    [SerializeField] private float ColliderDepth = 1f;

    [SerializeField] private Vector3 triggerArea = new Vector3(3, 3, 3);

    private Creator trigger;

    public Transform opposite;

    private Dictionary<CollisionFix, Fixer> fixers = new Dictionary<CollisionFix, Fixer>();

    private void Start()
    {

    }

    Creator SetUpTrigger()
    {
        var triggerChild = new GameObject("CollisionFixer");
        triggerChild.transform.SetParent(gameObject.transform);
        triggerChild.transform.localPosition = Vector3.zero;
        return triggerChild.AddComponent<Creator>().Init(this);
    }

    Fixer SetUpCollider()
    {
        var ret = new GameObject($"{gameObject.name} {opposite.gameObject.name} - CollisionFix");
        ret.transform.SetParent(null);
        return ret.AddComponent<Fixer>().Init(this, opposite);
    } 



    void EnterEffectArea(CollisionFix instance, Collider collider) 
    {
    }
    void LeaveEffectArea(CollisionFix instance, Collider collider) 
    {
    
    }

    private class Creator : MonoBehaviour
    {
        public CollisionFix Base;
        private BoxCollider trigger;

        public Creator Init(CollisionFix baseScript)
        {
            Base = baseScript;
            gameObject.layer = ColliderLayers.CollisionFix;
            trigger = gameObject.AddComponent<BoxCollider>();
            trigger.size = Base.triggerArea;
            return this;
        }
        private void OnTriggerEnter(Collider other)
        {
            var colfix = other.GetComponentInParent<CollisionFix>();
            if (colfix == null)
            {
                Physics.IgnoreCollision(trigger, other);
                return;
            }
            Base.EnterEffectArea(colfix, other);
        }

        private void OnTriggerExit(Collider other)
        {
            var colfix = other.GetComponentInParent<CollisionFix>();
            if (colfix == null)
            {
                Physics.IgnoreCollision(trigger, other);
                return;
            }
            Base.LeaveEffectArea(colfix, other);
        }

    }

    private class Fixer : MonoBehaviour
    {
        public CollisionFix Base;

        public Transform target { get; set; }
        public new Collider collider { get; set; }

        public HashSet<Collider> EnteredColliders { get; } = new HashSet<Collider>();
        

        public Fixer Init(CollisionFix baseScript, Transform target)
        {
            (Base, this.target) = (baseScript, target);
            collider = SetUpCollider();
            return this;
        }

        Collider SetUpCollider()
        {
            var bladeLength = Base.sword.SwordTip.position.Distance(Base.sword.SwordAnchor.position);
            var sideLength = Base.ColliderDepth / Mathf.Sqrt(2f);

            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            gameObject.layer = ColliderLayers.CollisionFix;
            var c = gameObject.AddComponent<BoxCollider>();
            c.size = new Vector3(sideLength, bladeLength, sideLength);
            var triggerCol = gameObject.AddComponent<BoxCollider>();
            triggerCol.isTrigger = true;
            triggerCol.size = c.size * 2;

            return c;
        }


        private void FixedUpdate()
        {
            if (target.IsNotNil())
                UpdateColliderPosition(target.position);
        }
        private void UpdateColliderPosition(Vector3 opposite)
        {
            var fixer = collider.gameObject;
            Vector3 bladeAnchor = Base.sword.SwordAnchor.position, bladeTip = Base.sword.SwordTip.position;
            var bladeCenter = (bladeAnchor + bladeTip) * 0.5f;
            var bladeDirection = (bladeTip - bladeAnchor).normalized;
            var planeToContainCollider = new Plane(bladeDirection, bladeCenter);

            var direction = (bladeCenter - planeToContainCollider.ClosestPointOnPlane(opposite)).normalized;
            var position = bladeCenter + direction * Base.ColliderDepth * 0.5f;

            fixer.transform.position = position;
            fixer.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection) * Quaternion.LookRotation(direction, bladeDirection);
        }


        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collider entered");
            if (!collision.gameObject.transform.IsOrHasAncestor(target))
            {
                foreach(var c in collision.gameObject.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(this.collider, c, true);
                //collision.rigidbody?.AddForce(-collision.impulse);
                Debug.Log($"Disabled {collider.gameObject} - {collision.gameObject}");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Trigger entered");
            if (!other.gameObject.transform.IsOrHasAncestor(target))
            {
                foreach (var c in other.gameObject.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(this.collider, c, true);
                    //Physics.IgnoreCollision(this.triggerCollider, c, true);
                }
                Debug.Log($"Disabled {collider.gameObject} - {other.gameObject}");
            }
            else
            {
                foreach (var c in target.gameObject.GetComponentsInChildren<Collider>())
                    ;// Physics.IgnoreCollision(this.triggerCollider, c, true);
            }
        }
    }


















#if false
    private static int TargetLayer => ColliderLayers.Swords;

    private static ulong CollisionFixWaveCounter = 1;
    private ulong CollisionFixWave = 1;
    private static void UpdateCollisions()
    {
        ++CollisionFixWaveCounter;

        var currentScene = SceneManager.GetActiveScene();
        var instancesToUpdate = currentScene.GetAllComponents<CollisionFix>().Where(f=>f.isInitialized).ToList();

        var targetables = GetAllTargetableObjects(currentScene.GetRootGameObjects().Select(o => o.transform)).ToList();

        var colliderBuffer = new List<Collider>();


        foreach(var fix in instancesToUpdate)
        {
            foreach(var t in targetables.Where(t=>t.transform!=fix.opposite.transform /*&& t.LastUpdate < fix.CollisionFixWave*/))
            {
                t.GetComponentsInChildren<Collider>(colliderBuffer);
                foreach (var c in colliderBuffer) if (c.gameObject.layer == TargetLayer) Physics.IgnoreCollision(fix.fixCollider, c);
            }
            fix.CollisionFixWave = CollisionFixWaveCounter;
        }
        foreach (var t in targetables) t.LastUpdate = CollisionFixWaveCounter;
    }


    private static IEnumerable<TargetObjectMetadata> GetAllTargetableObjects(IEnumerable<Transform> root)
    {
        foreach(var o in root)
        {
            if (o.gameObject.layer == TargetLayer)
                yield return o.gameObject.GetComponent<TargetObjectMetadata>() ?? o.gameObject.AddComponent<TargetObjectMetadata>();
            else 
                foreach (var y in GetAllTargetableObjects(o.Cast<Transform>())) yield return y;
        }
    }

    private class TargetObjectMetadata : MonoBehaviour
    {
        public ulong LastUpdate { get; set; } = 0;
    }
#endif


}
