using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
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
            UseOriginal = default, UseProvidedValue,
            X = 100, Y, Z,
            R = 200, G, B, A
        }

        public static implicit operator VectorField(float value) => new VectorField(value);
        public static implicit operator VectorField(float? value) => value == null ? V.Null : new VectorField(value.Value);

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



    public static class VectorHelpers
    {
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
            return new Vector3(FieldValue(x, self.x), FieldValue(y, self.y), FieldValue(z, self.z));
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
            return new Color(FieldValue(r, self.r), FieldValue(g, self.g), FieldValue(b, self.b), FieldValue(a, self.a));
        }

        public static Quaternion WithEuler(this Quaternion self, VectorField x = default, VectorField y = default, VectorField z = default)
            => Quaternion.Euler(self.eulerAngles.With(x, y, z));
        public static bool Any(this Vector3 a, Vector3 b, System.Func<float, float, bool> f)
            => f(a.x, b.x) || f(a.y, b.y) || f(a.z, b.z);


        public static Color AsHSV(this Vector3 v) => Color.HSVToRGB(v.x, v.y, v.z);
        public static Vector3 AsVectorHSV(this Color c)
        {
            Color.RGBToHSV(c, out var h, out var s, out var v);
            return new Vector3(h, s, v);
        }

        public static Vector3 ClampFields(this Vector3 self, Interval<Vector3> i)
            => new Vector3(Mathf.Clamp(self.x, i.Min.x, i.Max.x), Mathf.Clamp(self.y, i.Min.y, i.Max.y), Mathf.Clamp(self.z, i.Min.z, i.Max.z));




        //TODO: Fix this so that it handles correctly intervals like <180°; 210°> aka. <180°;-150°> etc.
        public static Vector3 ClampEuler(this Vector3 self, Interval<Vector3> i)
        {
            float Fix(float f) => (f %= 360) >= 180f ? f - 360 : f;
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




        public static bool IsCloseTo(this Vector3 v, Vector3 w, float? epsilon = null) => v.x.IsCloseTo(w.x, epsilon) && v.y.IsCloseTo(w.y, epsilon) && v.z.IsCloseTo(w.z, epsilon);
        public static float Distance(this Vector3 self, Vector3 b) => Vector3.Distance(self, b);

        public static bool IsNaN(this Vector3 v) => v.x.IsNaN() || v.y.IsNaN() || v.z.IsNaN();

        public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);

        public static Vector3 MultiplyElems(this Vector3 a, float x, float y, float z) => new Vector3(a.x * x, a.y * y, a.z * z);
        public static Vector3 MultiplyElems(this Vector3 a, Vector3 b) => a.MultiplyElems(b.x, b.y, b.z);

        public static Vector2 MultiplyElems(this Vector2 a, float x, float y) => new Vector3(a.x * x, a.y * y);
        public static Vector2 MultiplyElems(this Vector2 a, Vector2 b) => a.MultiplyElems(b.x, b.y);


        public static Vector2 x0(this Vector2 v) => new Vector2(v.x, 0);
        public static Vector2 _0y(this Vector2 v) => new Vector2(0, v.y);

        public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
        public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
        public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);

        public static Vector3 xy0(this Vector2 v) => new Vector3(v.x, v.y, 0);
        public static Vector3 xyx(this Vector2 v) => new Vector3(v.x, v.y, v.x);
        public static Vector3 x0z(this Vector2 v) => new Vector3(v.x, 0, v.y);
        public static Vector3 _0yz(this Vector2 v) => new Vector3(0, v.x, v.y);
    }
}
