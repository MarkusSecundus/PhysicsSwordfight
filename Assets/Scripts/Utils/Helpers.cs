using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RotationUtil
{
    public const float MaxDegree = 360;
    public const float MaxRadians = Mathf.PI*2f;

    public const float MaxAngle = MaxDegree;
}


[System.Serializable]
public struct Vector3Interval
{
    public Vector3 Min, Max;
}

public struct TransformSnapshot
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public TransformSnapshot(Transform t)
    {
        (position, rotation, localScale) = (t.position, t.rotation, t.localScale);
    }
    public TransformSnapshot(Rigidbody r)
    {
        (position, rotation, localScale) = (r.position, r.rotation, default);
    }

    public void SetTo(Transform t)
    {
        (t.position, t.rotation) = (position, rotation);
        if (localScale != default) t.localScale = localScale;
    }
    public void SetTo(Rigidbody r)
    {
        (r.position, r.rotation) = (position, rotation);
    }
}

public static class Op
{
    public static T post_assign<T>(ref T variable, T newValue)
    {
        var ret = variable;
        variable = newValue;
        return ret;
    }
}

public struct Vector3Field
{
    public float Value { get; }
    public FieldType Field { get; }
    public Vector3Field(float value) => (Value, Field) = (value, FieldType.UseProvidedValue);
    public Vector3Field(FieldType type)
    {
        if (type == FieldType.UseProvidedValue)
            throw new System.ArgumentException($"To use the type {nameof(FieldType.UseProvidedValue)}, use the other constructor that provides the value!");
        (Value, Field) = (default, type);
    }

    public enum FieldType : byte
    {
        UseOriginal=default, UseProvidedValue, X, Y, Z
    }

    public static implicit operator Vector3Field(float value) => new Vector3Field(value);
    public static implicit operator Vector3Field(float? value) => value == null? V.Null : new Vector3Field(value.Value);

    static Vector3Field()
    {
        if (default(Vector3Field).Field != FieldType.UseOriginal)
            throw new System.InvalidProgramException($"Assert failed: default({nameof(Vector3Field)}) must be equal to Null");
    }
}

public static class V
{
    public static readonly Vector3Field X = new Vector3Field(Vector3Field.FieldType.X), Y = new Vector3Field(Vector3Field.FieldType.Y), Z = new Vector3Field(Vector3Field.FieldType.Z), Null = default;
}


public static class EnumUtil
{
    public static TEnum Parse<TEnum>(string name) => (TEnum)System.Enum.Parse(typeof(TEnum), name);

    public static TEnum[] GetValues<TEnum>() where TEnum : System.Enum => (TEnum[])System.Enum.GetValues(typeof(TEnum));
}


[System.Serializable]
public struct InspectableKeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}


public enum TransformationPolicy
{
    Direction, Point, Vector
}

public static class HelperExtensions
{
    public static IEnumerable<ContactPoint> IterateContacts(this Collision self)
    {
        for (int t = 0; t < self.contactCount; ++t) yield return self.GetContact(t);
    }

    public static Vector3 GetPositionRelativeTo(this Transform self, Transform relativeTo) 
        => (relativeTo == self)        ? Vector3.zero :
           (relativeTo == self.parent) ? self.localPosition :
              relativeTo.GlobalToLocal(self.position);
    public static Vector3 SetPositionRelativeTo(this Transform self, Transform relativeTo, Vector3 position)
    {
        if (relativeTo == self)
            return (position == Vector3.zero) ? position : throw new System.ArgumentException("Position relative to self cannot be anything other than zero", nameof(relativeTo));
        if (relativeTo == self.parent) return self.localPosition = position;
        return self.position = relativeTo.LocalToGlobal(position);
    }
    public static T Minimal<T, TComp>(this IEnumerable<T> self, System.Func<T, TComp> selector) where TComp: System.IComparable<TComp>
    {
        using var it = self.GetEnumerator();
        if (!it.MoveNext()) throw new System.ArgumentOutOfRangeException("Empty collection was provided!");

        var ret = it.Current;
        var min = selector(ret);

        while (it.MoveNext())
        {
            var cmp = selector(it.Current);
            if(cmp.CompareTo(min) < 0)
            {
                min = cmp;
                ret = it.Current;
            }
        }

        return ret;
    }

    public static Vector3 LocalToGlobal(this Transform self, Vector3 v) => self.TransformPoint(v);
    public static Vector3 GlobalToLocal(this Transform self, Vector3 v) => self.InverseTransformPoint(v);
    public static Ray GlobalToLocal(this Transform self, Ray r) => r.GenericTransform(self.GlobalToLocal);
    public static Ray LocalToGlobal(this Transform self, Ray r) => r.GenericTransform(self.LocalToGlobal);


    public static Ray GenericTransform(this Ray r, System.Func<Vector3, Vector3> transformPoints)
    {
        Vector3 a = transformPoints(r.origin), b = transformPoints(r.origin + r.direction);
        return new Ray(a, b - a);
    }


    public static Vector3 InverseTransform(this Transform self, Vector3 v, TransformationPolicy policy)
        => policy switch
        {
            TransformationPolicy.Direction => self.InverseTransformDirection(v),
            TransformationPolicy.Point => self.InverseTransformPoint(v),
            TransformationPolicy.Vector => self.InverseTransformVector(v),
            _ => throw new System.ArgumentException($"Invalid value of argument {nameof(v)}: '{v}'")
        };
    public static Vector3 Transform(this Transform self, Vector3 v, TransformationPolicy policy)
        => policy switch
        {
            TransformationPolicy.Direction => self.TransformDirection(v),
            TransformationPolicy.Point => self.TransformPoint(v),
            TransformationPolicy.Vector => self.TransformVector(v),
            _ => throw new System.ArgumentException($"Invalid value of argument {nameof(v)}: '{v}'")
        };

    public static bool HasNullElement<T>(this (T? a, T? b) self) where T : unmanaged
        => self.a == null || self.b == null;
    public static bool HasNullElement<T>(this (T a, T b) self) where T : class
        => self.a == null || self.b == null;

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> self) => self==null || self.Count <= 0;
    public static Vector3 Average<T>(this IEnumerable<T> self, System.Func<T, Vector3> selector)
    {
        var (ret, count) = (Vector3.zero, 0);
        foreach (var i in self)
        {
            ret += selector(i);
            ++count;
        }
        return count <= 0? Vector3.zero : ret / count;
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> self, System.Func<T, bool> predicate, System.Func<T> defaultSupplier)
    {
        foreach(var i in self)
            if (predicate(i)) return i;
        
        return defaultSupplier();
    }
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self)
    {
        var ret = new Dictionary<TKey, TValue>();
        ret.AddAll(self);
        return ret;
    }
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<InspectableKeyValuePair<TKey, TValue>> self)
        => self.Select(p => new KeyValuePair<TKey, TValue>(p.Key, p.Value)).ToDictionary();

    public static ICollection<T> AddAll<T>(this ICollection<T> self, IEnumerable<T> toAdd)
    {
        foreach (var it in toAdd) self.Add(it);
        return self;
    }

    public static T WithModified<T>(this T self, System.Action<T> modify) where T: struct
    {
        modify(self);
        return self;
    }
    public static Vector3 With(this Vector3 self, Vector3Field x = default, Vector3Field y = default, Vector3Field z = default)
    {
        float FieldValue(Vector3Field field, float original) => field.Field switch
        {
            Vector3Field.FieldType.UseOriginal => original,
            Vector3Field.FieldType.UseProvidedValue => field.Value,
            Vector3Field.FieldType.X => self.x,
            Vector3Field.FieldType.Y => self.y,
            Vector3Field.FieldType.Z => self.z,
            _ => throw new System.ArgumentException($"Provided ")
        };
        return new Vector3(FieldValue(x,self.x), FieldValue(y, self.y), FieldValue(z, self.z));
    }

    public static Quaternion WithEuler(this Quaternion self, Vector3Field x = default, Vector3Field y = default, Vector3Field z = default) 
        => Quaternion.Euler(self.eulerAngles.With(x, y, z));
    public static void Log(this string self, bool shouldLog = true)
    {
        if (shouldLog) Debug.Log(self);
    }
    public static GameObject InstantiateWithTransform(this GameObject o,bool copyPosition=true, bool copyRotation = true, bool copyScale = true, bool copyParent=true)
    {
        var ret = GameObject.Instantiate(o);

        if (copyPosition) ret.transform.position = o.transform.position;
        if (copyRotation) ret.transform.rotation = o.transform.rotation;
        if(copyScale) ret.transform.localScale = o.transform.localScale;
        if (copyParent) ret.transform.parent = o.transform.parent;
        ret.SetActive(true);

        return ret;
    }

    public static bool Any(this Vector3 a, Vector3 b, System.Func<float, float, bool> f)
        => f(a.x, b.x) || f(a.y, b.y) || f(a.z, b.z);


    public static bool IsNaN(this float f) => float.IsNaN(f);
    public static bool IsPositiveInfinity(this float f) => float.IsPositiveInfinity(f);
    public static bool IsNegativeInfinity(this float f) => float.IsNegativeInfinity(f);

    public static bool IsNormalNumber(this float f) => !f.IsNaN() && !f.IsNegativeInfinity() && !f.IsPositiveInfinity();

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

    public static T Peek<T>(this IReadOnlyList<T> self) => self[self.Count-1];
    public static bool TryPeek<T>(this IReadOnlyList<T> self, out T ret)
    {
        if (self.IsEmpty())
        {
            ret = default;
            return false;
        }
        else
        {
            ret = self.Peek();
            return true;
        }
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> self)
    {
        foreach (var collection
            in self)
            foreach (var item in collection) 
                yield return item;
    }

    public static Vector3 NextVector3(this System.Random self, Vector3 min, Vector3 max)
        => new Vector3((float)(min.x + self.NextDouble() * (max.x - min.x)), (float)(min.y + self.NextDouble() * (max.y - min.y)), (float)(min.z + self.NextDouble() * (max.z - min.z)));
    public static Vector2 NextVector2(this System.Random self, Vector2 min, Vector2 max) 
        => new Vector2((float)(min.x + self.NextDouble() * (max.x - min.x)), (float)(min.y + self.NextDouble() * (max.y - min.y)));


    public static float Mod(this float f, float mod, out float div)
    {
        var d = System.Math.Floor(f / mod);
        div = (float)d;
        var ret = f - (d * mod);
        if (ret < 0f) ret += mod;
        return (float)ret;
    }
    public static float Mod(this float f, float mod) => f.Mod(mod, out _);

    public static Vector3 Clamp(this Vector3 self, Vector3Interval i) 
        => new Vector3(Mathf.Clamp(self.x, i.Min.x, i.Max.x), Mathf.Clamp(self.y, i.Min.y, i.Max.y), Mathf.Clamp(self.z, i.Min.z, i.Max.z));

    


    //TODO: Fix this so that it handles correctly intervals like <180°; 210°> aka. <180°;-150°> etc.
    public static Vector3 ClampEuler(this Vector3 self, Vector3Interval i)
    {
        float Fix(float f) => (f%=360) >= 180f ? f-360 : f;
        return new Vector3(Fix(self.x), Fix(self.y), Fix(self.z)).Clamp(i);

        //attempt at correct implementation that doesn't work however
        //float Clamp(float f, float min, float max) => ClampModulo(f, min, max, 360);  
        //return new Vector3(Clamp(self.x, i.Min.x, i.Max.x), Clamp(self.y, i.Min.y, i.Max.y), Clamp(self.z, i.Min.z, i.Max.z));
        //float ClampModulo(float f, float min, float max, float modulo)
        //{
        //    (f,min,max) = (f.Mod(modulo), min.Mod(modulo), max.Mod(modulo));
        //    if (min <= max) return Mathf.Clamp(f, min, max);
        //    else return Mathf.Clamp(f, min, max + modulo).Mod(modulo);
        //}
    }
}


public static class DrawHelpers
{
    public delegate void LineDrawer<TVect>(TVect lineBegin, TVect lineEnd);

    public static void DrawDirectedLine(this LineDrawer<Vector3> self, Vector3 begin, Vector3 direction)
        => self(begin, begin + direction);

    public static void DrawWireCircle(float radius, int segments, LineDrawer<Vector2> drawLine)
    {
        Vector2 v = new Vector2(radius,0);
        foreach(Vector2 w in GeometryUtils.PointsOnCircle(segments, v))
        {
            drawLine(v,  w);
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

        int computeCircleSegments(float radius) => (int)Mathf.Max(6, Mathf.Ceil(Mathf.Sqrt(radius) * 36f));
        int computeSphereCircles(float radius) => (int)Mathf.Max(2, Mathf.Ceil(Mathf.Sqrt(radius) * 18f));
    }


    public static void DrawPlaneSegment(Plane plane, Vector3 center, LineDrawer<Vector3> drawLine, Vector2 diameter=default, int segments=24)
    {
        if (diameter == default) diameter = new Vector2(1, 1);

        var step = diameter / (segments);

        var bs = plane.GetBase();

        Debug.DrawLine(center, center + plane.normal, Color.red);
        Debug.DrawLine(center, center + bs.X, Color.white);
        Debug.DrawLine(center, center + bs.Y, Color.yellow);

        Vector2 begin = -diameter / 2;

        for(int x = 1; x< segments; ++x)
        {
            var b = begin + step.x0() * x;
            var e = begin + step.x0() * x + step._0y() * segments;
            draw(b, e);
        }

        for(int y = 1; y< segments; ++y)
        {
            var b = begin + step._0y() * y;
            var e = begin + step._0y() * y + step.x0() * segments;
            draw(b, e);
        }

        void draw(Vector2 a, Vector2 b)
        {
            drawLine(bs.GetBasedVector(a) + center, bs.GetBasedVector(b) + center);
            //Debug.Log($"{a}:{b}");
        }
    }
}