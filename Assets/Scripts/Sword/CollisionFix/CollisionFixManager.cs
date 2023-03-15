using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFixManager : MonoBehaviour
{
    private readonly Dictionary<Collider, CollisionFix> fixByCollider = new Dictionary<Collider, CollisionFix>();

    private readonly HashSet<CollisionFix> active = new HashSet<CollisionFix>();
    private readonly Dictionary<UnorderedPair<CollisionFix>, CollisionFix.Fixer> fixers = new Dictionary<UnorderedPair<CollisionFix>, CollisionFix.Fixer>();


    public void Register(CollisionFix fix) 
    {
        if (!active.Add(fix)) throw new System.InvalidOperationException($"{fix.name} is already registered!");

        foreach(var f in active)
        {
            if (f == fix) continue;

            CreateFixer(fix, f);
        }

        foreach(var c in fix.AllColliders)
        {
            fixByCollider.Add(c, fix);
        }
    }

    public void Unregister(CollisionFix fix) 
    {
        if (!active.Contains(fix)) throw new System.InvalidOperationException($"{fix.name} is not registered!");

        foreach(var f in active)
        {
            if (f == fix) continue;

            if(fixers.ContainsKey(new UnorderedPair<CollisionFix>(f,fix)))
                DestroyFixer(fix, f);
        }

        foreach(var c in fix.AllColliders)  //not an ideal solution that might lead to memory leaks if some colliders were removed from the fixer after it was created
        {
            fixByCollider.Remove(c);
        }
    }



    private void CreateFixer(CollisionFix a, CollisionFix b)
    {
        var fixer = GameObjectUtils.InstantiateUtilObject($"fixer_{a.name}-{b.name}").AddComponent<CollisionFix.Fixer>().Init(a, b);
        fixers.Add(new UnorderedPair<CollisionFix>(a, b), fixer);
    }

    private void DestroyFixer(CollisionFix a, CollisionFix b)
    {
        var key = new UnorderedPair<CollisionFix>(a, b);
        var fixer = fixers[key];
        fixers.Remove(key);
        Object.Destroy(fixer.gameObject);
    }

}
