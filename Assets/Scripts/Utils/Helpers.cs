using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public static class RotationUtil
{
    public const float MaxDegree = 360;
    public const float MaxRadians = Mathf.PI*2f;

    public const float MaxAngle = MaxDegree;
}

public static class EnumUtil
{
    public static TEnum Parse<TEnum>(string name) => (TEnum)System.Enum.Parse(typeof(TEnum), name);
}

public static class HelperExtensions
{
    public static void Log(this string self, bool shouldLog = true)
    {
        if (shouldLog) Debug.Log(self);
    }
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

    public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector3 xy0(this Vector2 v) => new Vector3(v.x, v.y, 0);
    public static Vector3 x0z(this Vector2 v) => new Vector3(v.x, 0, v.y);
}


public static class DrawHelpers
{
    public delegate void LineDrawer<TVect>(TVect lineBegin, TVect lineEnd);

    public static void DrawWireCircle(float radius, int segments, LineDrawer<Vector2> drawLine)
    {
        var rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, RotationUtil.MaxAngle / segments));
        Vector3 v = new Vector3(radius,0, 0);
        for(int t = 0; t < segments;++t)
        {
            Vector3 w = rot * v;
            Assert.AreEqual(0, v.z);
            Assert.AreEqual(0, w.z);
            drawLine(v.xy(),  w.xy());
            v = w;
        }
    }

    public static void DrawWireSphere(Vector3 center, float radius, LineDrawer<Vector3> drawLine, int? segments = null, int? circles = null)
    {
        int segm = segments ?? computeCircleSegments(radius);
        int circs = circles ?? computeSphereCircles(radius);

        var rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, RotationUtil.MaxAngle / 2 / circs));
        DrawWireCircle(radius, segm, (v, w) =>
        {
            Vector3 a = v.x0z(), b= w.x0z();
            for (int t = 0; t < circs; ++t)
            {
                drawLine(a + center, b + center);
                a = rot * a;
                b = rot * b;
            }
        });
    }

    private static int computeCircleSegments(float radius) => (int) Mathf.Max(6, Mathf.Ceil(Mathf.Sqrt(radius)*36f));
    private static int computeSphereCircles(float radius) => (int) Mathf.Max(2, Mathf.Ceil(Mathf.Sqrt(radius) *18f));
}