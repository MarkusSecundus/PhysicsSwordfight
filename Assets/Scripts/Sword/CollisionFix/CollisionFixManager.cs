using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using AttachedFixersTable = System.Collections.Generic.Dictionary<MarkusSecundus.PhysicsSwordfight.Sword.Collisions.CollisionFix, MarkusSecundus.PhysicsSwordfight.Sword.Collisions.CollisionFix.Fixer>;
using ColliderToParentTable = System.Collections.Generic.Dictionary<UnityEngine.Collider, MarkusSecundus.PhysicsSwordfight.Sword.Collisions.CollisionFix>;

namespace MarkusSecundus.PhysicsSwordfight.Sword.Collisions
{

    public partial class CollisionFix
    {
        readonly AttachedFixersTable attachedFixers = new AttachedFixersTable();

        class CollisionFixManager : MonoBehaviour
        {
            private readonly ColliderToParentTable fixByCollider = new ColliderToParentTable();

            private readonly HashSet<CollisionFix> active = new HashSet<CollisionFix>();


            public bool TryFindFixer(Collider c, out CollisionFix parent) => fixByCollider.TryGetValue(c, out parent);

            private bool TryFindFixer(CollisionFix a, CollisionFix b, out CollisionFix.Fixer ret) => TryFindFixer(a, b, out ret, out _, out _);
            private bool TryFindFixer(CollisionFix a, CollisionFix b, out CollisionFix.Fixer ret, out AttachedFixersTable containingTable, out CollisionFix key)
                => (containingTable = a.attachedFixers).TryGetValue(key = b, out ret)
                || (containingTable = b.attachedFixers).TryGetValue(key = a, out ret);

            public void Register(CollisionFix fix)
            {
                if (!active.Add(fix)) throw new System.InvalidOperationException($"{fix.name} is already registered!");


                foreach (var c in fix.AllColliders)
                {
                    fixByCollider.Add(c, fix);
                }
            }

            public void Unregister(CollisionFix fix)
            {
                if (!active.Contains(fix)) throw new System.InvalidOperationException($"{fix.name} is not registered!");

                foreach (var f in active)
                {
                    if (f == fix) continue;

                    DestroyFixer(fix, f);
                }

                foreach (var c in fix.AllColliders)  //not an ideal solution that might lead to memory leaks if some colliders were removed from the fixer after it was created
                {
                    fixByCollider.Remove(c);
                }
            }

            public void EnableFixer(CollisionFix a, CollisionFix b)
            {
                if (TryFindFixer(a, b, out _))
                    return;

                if (Random.Range(0, 2) == 0) (a, b) = (b, a);

                var fixer = GameObjectHelpers.InstantiateUtilObject($"fixer_{a.name}-{b.name}").AddComponent<CollisionFix.Fixer>().Init(a, b);
                a.attachedFixers.Add(b, fixer);
            }

            public void DisableFixer(CollisionFix a, CollisionFix b)
            {
                if (TryFindFixer(a, b, out var fixer))
                {
                    fixer.gameObject.SetActive(false);
                }
            }

            private void DestroyFixer(CollisionFix a, CollisionFix b)
            {
                if (TryFindFixer(a, b, out var fixer, out var containingDict, out var key))
                {
                    containingDict.Remove(key);
                    Object.Destroy(fixer.gameObject);
                }
            }
        }
    }

}