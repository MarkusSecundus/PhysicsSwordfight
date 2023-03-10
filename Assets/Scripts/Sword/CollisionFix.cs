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

    private Collider fixCollider;

    public Transform opposite;


    void Start()
    {
        fixCollider = SetUpCollider();
    }

    private void FixedUpdate()
    {
        if(opposite.IsNotNil())
            UpdateColliderPosition(opposite.position);
    }


    private bool isInitialized = false;
    Collider SetUpCollider()
    {
        var bladeLength = sword.SwordTip.position.Distance(sword.SwordAnchor.position);
        var sideLength = ColliderDepth / Mathf.Sqrt(2f);

        var ret = new GameObject($"{gameObject.name} {opposite.gameObject.name} - CollisionFix");
        ret.transform.SetParent(null);
        var rb = ret.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        var c = ret.AddComponent<BoxCollider>();
        c.gameObject.layer = ColliderLayers.CollisionFix;
        c.size = new Vector3(sideLength, bladeLength, sideLength);
        var updater = ret.AddComponent<ColliderUpdater>();
        updater.target = this.opposite;
        updater.collider = c;
        //this.isInitialized = true;

        return c;
    } 

    private void UpdateColliderPosition(Vector3 opposite)
    {
        var fixer = fixCollider.gameObject;
        Vector3 bladeAnchor = sword.SwordAnchor.position, bladeTip = sword.SwordTip.position;
        var bladeCenter = (bladeAnchor + bladeTip) * 0.5f;
        var bladeDirection = (bladeTip - bladeAnchor).normalized;
        var planeToContainCollider = new Plane(bladeDirection, bladeCenter);

        var direction =  (bladeCenter - planeToContainCollider.ClosestPointOnPlane(opposite)).normalized;
        var position = bladeCenter + direction * ColliderDepth*0.5f; 

        fixer.transform.position = position;
        fixer.transform.rotation = Quaternion.AngleAxis(45f, bladeDirection)* Quaternion.LookRotation(direction, bladeDirection);
    }



    private class ColliderUpdater : MonoBehaviour
    {
        public Transform target { get; set; }
        public Collider collider { get; set; }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.transform.IsOrHasAncestor(target))
            {
                foreach(var c in collision.gameObject.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(this.collider, c, true);
                //collision.rigidbody?.AddForce(-collision.impulse);
                Debug.Log($"Disabled {collider.gameObject} - {collision.gameObject}");
            }
        }
    }


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
}
