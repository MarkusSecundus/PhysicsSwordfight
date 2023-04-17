using JetBrains.Annotations;
using MarkusSecundus.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class RotationUtil
{
    public const float MaxDegree = 360;
    public const float MaxRadians = Mathf.PI*2f;

    public const float MaxAngle = MaxDegree;
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

public struct VectorField
{
    public float Value { get; }
    public FieldType Field { get; }
    public VectorField(float value) => (Value, Field) = (value, FieldType.UseProvidedValue);
    public VectorField(FieldType type)
    {
        if (type == FieldType.UseProvidedValue)
            throw new System.ArgumentException($"To use the type {nameof(FieldType.UseProvidedValue)}, use the other constructor that provides the value!");
        (Value, Field) = (default, type);
    }

    public enum FieldType : byte
    {
        UseOriginal=default, UseProvidedValue,
        X=100, Y, Z,
        R=200, G, B, A
    }

    public static implicit operator VectorField(float value) => new VectorField(value);
    public static implicit operator VectorField(float? value) => value == null? V.Null : new VectorField(value.Value);

    static VectorField()
    {
        if (default(VectorField).Field != FieldType.UseOriginal)
            throw new System.InvalidProgramException($"Assert failed: default({nameof(VectorField)}) must be equal to Null");
    }
}

public static class V
{
    public static readonly VectorField X = new VectorField(VectorField.FieldType.X), Y = new VectorField(VectorField.FieldType.Y), Z = new VectorField(VectorField.FieldType.Z), Null = default;
}
public static class C
{
    public static readonly VectorField R = new VectorField(VectorField.FieldType.R), G = new VectorField(VectorField.FieldType.G), B = new VectorField(VectorField.FieldType.B), A = new VectorField(VectorField.FieldType.A), Null = default;
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

[System.Serializable]
public struct TransformChain
{
    public Transform root, tip;

    public bool IsValid() => root!=null && tip != null && tip.IsChildOf(root);

    public Transform[] ToArray() => UnityEngine.Animations.Rigging.ConstraintsUtils.ExtractChain(root, tip);
}


public static class GameObjectUtils
{
    public static GameObject InstantiateWithTransform(this GameObject o, bool copyPosition = true, bool copyRotation = true, bool copyScale = true, bool copyParent = true)
    {
        var ret = GameObject.Instantiate(o);

        if (copyPosition) ret.transform.position = o.transform.position;
        if (copyRotation) ret.transform.rotation = o.transform.rotation;
        if (copyScale) ret.transform.localScale = o.transform.localScale;
        if (copyParent) ret.transform.parent = o.transform.parent;
        ret.SetActive(true);

        return ret;
    }

    public static GameObject FindTagInAncestors(this GameObject o, string tag)
    {
        for (; o != null; o = o?.transform?.parent?.gameObject)
            if (o.CompareTag(tag)) 
                return o;
        return null;
    }

    //IsChildOf is a terrible name for what it is actually checking
    public static bool IsDescendantOf(this Transform descendant, Transform parent) => descendant.IsChildOf(parent);

    private static readonly string UtilGameObjectRootTag = "UtilGameObjectRoot";
    public static Transform GetUtilObjectParent()
    {
        var ret = GameObject.FindWithTag(UtilGameObjectRootTag);
        if(ret == null)
        {
            ret = new GameObject("UtilityObjects");
            ret.tag = UtilGameObjectRootTag;
        }
        return ret.transform;
    }


    public static GameObject InstantiateUtilObject(string name, params System.Type[] componentsToAdd) 
        => GetUtilObjectParent().CreateChild(name, componentsToAdd);

    public static T GetUtilComponent<T>() where T: UnityEngine.Component
    {
        var parent = GetUtilObjectParent();
        if (parent.GetComponentInChildren<T>() is not null and var ret) 
            return ret;
        else 
            return parent.CreateChild(typeof(T).Name).AddComponent<T>();
    }

    public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, object toYield)
    {
        self.StartCoroutine(impl());
        IEnumerator impl()
        {
            yield return toYield;
            toPerform();
        }
    }

    public static GameObject CreateChild(this Transform father, string name, params System.Type[] componentsToAdd)
    {
        var ret = new GameObject(name, componentsToAdd);
        ret.transform.SetParent(father);
        ret.transform.ResetLocalPositionRotationScale();
        return ret;
    }

    public static void ResetLocalPositionRotationScale(this Transform t)
    {
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
        t.localPosition = Vector3.zero;
    }

    public static bool IsOrHasAncestor(this Transform self, Transform ancestor)
    {
        if (self == ancestor) return true;
        for (; self != null; self = self.parent) if (self == ancestor) return true;
        return false;
    }

    public static T IfNil<T>(this T self, T defaultValue) => self.IsNil() ? defaultValue : self;
    public static bool IsNotNil(this object self) => !self.IsNil();
    public static bool IsNil(this object self) => self == null || self.Equals(null);



    public static IEnumerable<T> GetAllComponents<T>(this Scene self, bool includeInactive=false)
    {
        var buffer = new List<T>();
        foreach(var o in self.GetRootGameObjects())
        {
            o.GetComponentsInChildren<T>(includeInactive, buffer);
            foreach (var y in buffer) yield return y;
        }
    }
}

public static class IndirectionUtils
{
    public struct IndirectMessage
    {
        static readonly Regex Format = new Regex(@"^(?<Callee>[a-zA-Z][a-zA-Z0-9]*)\.(?<MessageName>[a-zA-Z][a-zA-Z0-9]*)$");

        public string CalleeName { get; init; }
        public string Message { get; init; }

        public object Invoke(object callee, System.Action<string> logError = null)
        {
            var methodToInvoke = callee.GetType().GetMethod(Message);
            if (methodToInvoke == null)
                logError?.Invoke($"Called message '{Message}' does not exist on object '{callee}'({CalleeName})");
            else
                return methodToInvoke.Invoke(callee, System.Array.Empty<object>());
            return null;
        }
        public static IndirectMessage? Make(string calleeAndMessage, System.Action<string> logError=null)
        {
            var parsed = Format.Match(calleeAndMessage);
            if (!parsed.Success)
            {
                logError?.Invoke($"Invalid message declaration: '{calleeAndMessage}'");
                return null;
            }
            return new IndirectMessage
            {
                CalleeName = parsed.Groups["Callee"].Value,
                Message = parsed.Groups["MessageName"].Value,
            };
        }
    }
}

public static class SerializationUtils
{

    static readonly DefaultValDict<(string, System.Type), object> JsonDeserializerCache = new DefaultValDict<(string Path, System.Type Type), object>(
        k => JsonConvert.DeserializeObject(File.ReadAllText(k.Path), k.Type)
    );
    public static T JsonFromFileCached<T>(string path) => (T)JsonDeserializerCache[(path, typeof(T))];
}

public static class HelperExtensions
{

    public static void DisableAllUpdates(this NavMeshAgent self)
    {
        self.updatePosition = false;
        self.updateRotation = false;
        self.updateUpAxis = false;
    }
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


    public static double ComputeChainLength(this IEnumerable<Transform> self)
    {
        double ret = 0f;

        Transform last = null;
        foreach(var t in self)
        {
            if (last != null) 
                ret += t.position.Distance(last.position);
            last = t;
        }

        return ret;
    }

    public static Vector3 Normalized(this Vector3 v, out float magnitude)
    {
        magnitude = v.magnitude;
        if (magnitude > 1E-05f) //copypasted from decompiled builtin Vector3.Normalize()
            return v / magnitude;
        else return Vector3.zero;
    }

    public static float Magnitude(this Vector3 v, out Vector3 normalized)
    {
        normalized = v.Normalized(out var ret);
        return ret;
    }

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

    public static string MakeString<T>(this IEnumerable<T> self, string separator=", ")
    {
        using var it = self.GetEnumerator();

        if (!it.MoveNext()) return "";
        var ret = new StringBuilder().Append(it.Current.ToString());
        while (it.MoveNext()) ret = ret.Append(separator).Append(it.Current.ToString());

        return ret.ToString();
    }
    public static IEnumerable<T> RepeatList<T>(this IEnumerable<T> self, int repeatCount)
    {
        while (--repeatCount >= 0)
            foreach (var i in self) yield return i;
    }
    
    public static string ToStringPrecise(this Vector3 v)
    {
        if (v == Vector3.zero) return "<ZERO>";
        var magnitude = v.Magnitude(out var normalized);
        return $"{magnitude}*{normalized}";//$"({v.x};{v.y};{v.z})";
    }

    public static Vector3 With(this Vector3 self, VectorField x = default, VectorField y = default, VectorField z = default)
    {
        float FieldValue(VectorField field, float original) => field.Field switch
        {
            VectorField.FieldType.UseOriginal => original,
            VectorField.FieldType.UseProvidedValue => field.Value,
            VectorField.FieldType.X => self.x,
            VectorField.FieldType.Y => self.y,
            VectorField.FieldType.Z => self.z,
            _ => throw new System.ArgumentException($"Provided ")
        };
        return new Vector3(FieldValue(x,self.x), FieldValue(y, self.y), FieldValue(z, self.z));
    }

    public static Color With(this Color self, VectorField r = default, VectorField g = default, VectorField b = default, VectorField a = default)
    {
        float FieldValue(VectorField field, float original) => field.Field switch
        {
            VectorField.FieldType.UseOriginal => original,
            VectorField.FieldType.UseProvidedValue => field.Value,
            VectorField.FieldType.R => self.r,
            VectorField.FieldType.G => self.g,
            VectorField.FieldType.B => self.b,
            VectorField.FieldType.A => self.a,
            _ => throw new System.ArgumentException($"Provided ")
        };
        return new Color(FieldValue(r,self.r), FieldValue(g, self.g), FieldValue(b, self.b), FieldValue(a, self.a));
    }

    public static Quaternion WithEuler(this Quaternion self, VectorField x = default, VectorField y = default, VectorField z = default) 
        => Quaternion.Euler(self.eulerAngles.With(x, y, z));
    public static void Log(this string self, bool shouldLog = true)
    {
        if (shouldLog) Debug.Log(self);
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

    public static Color AsHSV(this Vector3 v) => Color.HSVToRGB(v.x, v.y, v.z);
    public static Vector3 AsVectorHSV(this Color c)
    {
        Color.RGBToHSV(c, out var h, out var s, out var v);
        return new Vector3(h, s, v);
    }

    public static float NextFloat(this System.Random self, float min, float max) => (float)(min + self.NextDouble() * (max - min));

    public static Vector3 NextVector3(this System.Random self, Vector3 min, Vector3 max)
        => new Vector3(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y), self.NextFloat(min.z, max.z));
    public static Vector2 NextVector2(this System.Random self, Vector2 min, Vector2 max) 
        => new Vector2(self.NextFloat(min.x, max.x), self.NextFloat(min.y, max.y));

    public static Vector2 NextVector2(this System.Random self, Rect area) => self.NextVector2(area.min, area.max);

    public static Color NextRGBA(this System.Random self, Color min, Color max)
        => new Color(self.NextFloat(min.r, max.r), self.NextFloat(min.g, max.g), self.NextFloat(min.b, max.b), self.NextFloat(min.a, max.a));
    public static Color NextHSVA(this System.Random self, Color min, Color max)
        => self.NextVector3(min.AsVectorHSV(), max.AsVectorHSV()).AsHSV().With(a: self.NextFloat(min.a, max.a));

    public static float Mod(this float f, float mod, out float div)
    {
        var d = System.Math.Floor(f / mod);
        div = (float)d;
        var ret = f - (d * mod);
        if (ret < 0f) ret += mod;
        return (float)ret;
    }
    public static float Mod(this float f, float mod) => f.Mod(mod, out _);

    public static Vector3 ClampFields(this Vector3 self, Vector3Interval i) 
        => new Vector3(Mathf.Clamp(self.x, i.Min.x, i.Max.x), Mathf.Clamp(self.y, i.Min.y, i.Max.y), Mathf.Clamp(self.z, i.Min.z, i.Max.z));

    


    //TODO: Fix this so that it handles correctly intervals like <180°; 210°> aka. <180°;-150°> etc.
    public static Vector3 ClampEuler(this Vector3 self, Vector3Interval i)
    {
        float Fix(float f) => (f%=360) >= 180f ? f-360 : f;
        return new Vector3(Fix(self.x), Fix(self.y), Fix(self.z)).ClampFields(i);

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

    public static TResult Maximum<TResult, TCompare>(this IEnumerable<TResult> self, System.Func<TResult, TCompare> selector) where TCompare: System.IComparable<TCompare>
    {
        using var it = self.GetEnumerator();
        if (!it.MoveNext())
        {
            throw new System.IndexOutOfRangeException("Searching for max of an empty collection!");
        }
        var max = it.Current; 
        var maxComparer = selector(max);

        while (it.MoveNext())
        {
            var toCompare = selector(it.Current);
            if (toCompare.CompareTo(maxComparer) > 0) (max, maxComparer) = (it.Current, toCompare);
        }
        return max;
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


    public static void DrawPlaneSegmentInterstepped(Plane plane, Vector3 center, LineDrawer<Vector3> drawLine, Vector2 diameter = default, int segments = 24)
    {
        if (diameter == default) diameter = new Vector2(1, 1);

        var step = diameter / (segments);

        var bs = plane.GetBase();

        Vector2 begin = -diameter / 2;

        for (int x = 0; x < segments-1; ++x)
        {
            for(int y = 0; y< segments-1; ++y)
            {
                var b = begin + step.MultiplyElems(x, y);
                var bx = b + step.x0();
                var by = b + step._0y();
                draw(b, bx);
                draw(b, by);
            }
        }

        void draw(Vector2 a, Vector2 b)
        {
            drawLine(bs.GetBasedVector(a) + center, bs.GetBasedVector(b) + center);
            //Debug.Log($"{a}:{b}");
        }
    }
}