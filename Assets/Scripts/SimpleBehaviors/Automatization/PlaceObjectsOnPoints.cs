using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceObjectsOnPoints : MonoBehaviour
{
    [SerializeField] public IPointsSupplier CircleDefinition;
    public Transform ParentToFill;
    public GameObject ToPlace;

    public int Seed = 0;
    public bool RandomSeed = false;
    private int RealSeed => RandomSeed ? Random.Range(int.MinValue, int.MaxValue) : Seed;

    public void PlaceObjects() => PlaceObjects(CircleDefinition.IteratePoints());
    public void PlaceObjectsRandom(int count) => PlaceObjects(count, () => CircleDefinition.GetRandomPoint(new System.Random(RealSeed)));
    public void PlaceObjectsRandomVolume(int count) => PlaceObjects(count, () => CircleDefinition.GetRandomPointInVolume(new System.Random(RealSeed)));
    public void PlaceObjects(int count, System.Func<Vector3> supplier) => supplier.Repeat(count);

    private void PlaceObjects(IEnumerable<Vector3> points)
    {
        var random = new System.Random(RealSeed);

        Debug.Log("Running the event! - Parent count: {ParentToFill.childCount}");
        ClearParent();
        Debug.Log($"Parent count: {ParentToFill.childCount}");

        foreach (var v in CircleDefinition.IteratePoints())
        {
            var obj = ToPlace.InstantiateWithTransform(copyParent: false);
            obj.transform.position = v;
            obj.transform.SetParent(ParentToFill);
            foreach (var randomizer in obj.GetComponentsInChildren<IRandomizer>())
                randomizer.Randomize(random);
        }
        Debug.Log($"Finished - Parent count: {ParentToFill.childCount}");
    }

    public void ClearParent()
    {
        Debug.Log("Clearing the pallisade!");
        while (ParentToFill.childCount > 0)
        {
            foreach (Transform t in ParentToFill)
                Object.DestroyImmediate(t.gameObject);
        }
    }
}
