using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceObjectsOnPoints : MonoBehaviour
{
    [SerializeField]public IPointsSupplier CircleDefinition;
    public Transform ParentToFill;
    public GameObject ToPlace;

    public int Seed = 0;

    public Vector2 ScaleMin = Vector2.one, ScaleMax = Vector2.one;
    public Vector3 RotationMin = Vector3.one, RotationMax = Vector3.one;
    public Vector3 PlaceOffsetMin = Vector3.one, PlaceOffsetMax = Vector3.one;



    public void PlaceObjects()
    {
        var random = new System.Random(Seed);

        Debug.Log("Running the event! - Parent count: {ParentToFill.childCount}");
        ClearParent();
        Debug.Log($"Parent count: {ParentToFill.childCount}");

        foreach (var v in CircleDefinition.IteratePoints())
        {
            var obj = ToPlace.InstantiateWithTransform(copyParent: false);
            var scale = random.NextVector2(ScaleMin, ScaleMax).xyx();
            var rotation = random.NextVector3(RotationMin, RotationMax);
            obj.transform.localScale = obj.transform.localScale.MultiplyElems(scale);
            obj.transform.localRotation *= Quaternion.Euler(rotation);
            obj.transform.SetParent(ParentToFill);
            obj.transform.position = v + random.NextVector3(PlaceOffsetMin, PlaceOffsetMax);
        }
        Debug.Log($"Finished - Parent count: {ParentToFill.childCount}");
    }

    public void ClearParent()
    {
        if (ParentToFill.childCount > 0)
        {
            foreach (Transform t in ParentToFill)
                Object.DestroyImmediate(t.gameObject);
        }
    }
}
