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
    public void PlaceObjects(int count, System.Func<Vector3> supplier) => PlaceObjects(supplier.Repeat(count));

    public void ClearAndPlaceObjects()
    {
        ClearParent();
        PlaceObjects(CircleDefinition.IteratePoints());
    }
    private void PlaceObjects(IEnumerable<Vector3> points, bool shouldClear=false)
    {
        var random = new System.Random(RealSeed);

        foreach (var v in points)
        {
            var obj = ToPlace.InstantiateWithTransform();
            obj.transform.position = v + obj.transform.localPosition;
            obj.transform.SetParent(ParentToFill);
            obj.SetActive(true);
            foreach (var randomizer in obj.GetComponentsInChildren<IRandomizer>())
                randomizer.Randomize(random);
        }
    }

    public void ClearParent()
    {
        while (ParentToFill.childCount > 0)
        {
            foreach (Transform t in ParentToFill)
                Object.DestroyImmediate(t.gameObject);
        }
    }
}
