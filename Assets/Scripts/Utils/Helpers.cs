using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public static class EnumUtil
{
    public static TEnum Parse<TEnum>(string name) => (TEnum)System.Enum.Parse(typeof(TEnum), name);
}

public static class HelperExtensions
{
    public static GameObject InstantiateWithTransform(this GameObject o,bool copyPosition=true, bool copyRotation = true, bool copyScale = true)
    {
        var ret = GameObject.Instantiate(o);

        if (copyPosition) ret.transform.position = o.transform.position;
        if (copyRotation) ret.transform.rotation = o.transform.rotation;
        if(copyScale) ret.transform.localScale = o.transform.localScale;
        ret.SetActive(true);

        return ret;
    }

    public static bool Any(this Vector3 a, Vector3 b, System.Func<float, float, bool> f)
        => f(a.x, b.x) || f(a.y, b.y) || f(a.z, b.z);



    public static IEnumerable<GameObject> TransformChildren(this GameObject self) => self.transform.Cast<Transform>().Select(t=>t.gameObject);

    public static void StartPlaying(this AudioSource self, AudioClip clip, bool forceAnew=true)
    {
        if (!forceAnew && self.isPlaying && self.clip == clip) return;
        self.Stop();
        self.clip = clip;
        self.Play();
    }

    public static T RandomElement<T>(this IReadOnlyList<T> self)
        => self[Random.Range(0, self.Count)];

    public static bool IsNotNullNotEmpty<T>(this IReadOnlyList<T> self)
        => self != null && self.Count > 0;


    public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, float delay)
    {
        self.StartCoroutine(impl());

        IEnumerator<YieldInstruction> impl()
        {
            yield return new WaitForSeconds(delay);
            toPerform();
        }
    }
}
