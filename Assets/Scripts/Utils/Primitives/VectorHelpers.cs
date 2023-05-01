using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Primitives
{
    /// <summary>
    /// Object symbolically representing a field of a vector. To be used in combination with <see cref="VectorHelpers.With(Vector3, VectorField, VectorField, VectorField)"/>.
    /// </summary>
    public struct VectorField
    {
        /// <summary>
        /// Concrete value of the field
        /// </summary>
        public float Value { get; }
        /// <summary>
        /// Kind of vector field this instance represents
        /// </summary>
        public FieldType Field { get; }
        /// <summary>
        /// Inits a <see cref="VectorField"/> instance that represents a concrete value
        /// </summary>
        /// <param name="value"></param>
        public VectorField(float value) => (Value, Field) = (value, FieldType.UseProvidedValue);
        /// <summary>
        /// Inits a <see cref="VectorField"/> instance that represents a reference to a vector field.
        /// </summary>
        /// <param name="type">Type of reference <c>this</c> will represent</param>
        /// <exception cref="System.ArgumentException">if <paramref name="type"/>is <see cref="FieldType.UseProvidedValue"/></exception>
        public VectorField(FieldType type)
        {
            if (type == FieldType.UseProvidedValue)
                throw new System.ArgumentException($"To use the type {nameof(FieldType.UseProvidedValue)}, use the other constructor that provides the value!");
            (Value, Field) = (default, type);
        }

        /// <summary>
        /// Types of fields the FieldType can refer
        /// </summary>
        public enum FieldType : byte
        {
            /// <summary>
            /// Use the original value of the vector field in mention
            /// </summary>
            UseOriginal = default, 
            /// <summary>
            /// Set the field to provided value
            /// </summary>
            UseProvidedValue,
            /// <summary>
            /// Set this field to the value of vector's X field
            /// </summary>
            X = 100,
            /// <summary>
            /// Set this field to the value of vector's Y field
            /// </summary>
            Y,
            /// <summary>
            /// Set this field to the value of vector's Z field
            /// </summary>
            Z,
            /// <summary>
            /// Set this field to the value of color's R field
            /// </summary>
            R = 200,
            /// <summary>
            /// Set this field to the value of color's G field
            /// </summary>
            G,
            /// <summary>
            /// Set this field to the value of color's B field
            /// </summary>
            B,
            /// <summary>
            /// Set this field to the value of color's A field
            /// </summary>
            A
        }

        /// <summary>
        /// Get a concrete vector field from given value
        /// </summary>
        /// <param name="value">Value to be assigned to the vector field</param>
        public static implicit operator VectorField(float value) => new VectorField(value);
        /// <summary>
        /// Get a concrete vector field from given value, or <see cref="VectorField.FieldType.UseOriginal"/> if the value is <c>null</c>
        /// </summary>
        /// <param name="value">Value to be assigned to the vector field</param>
        public static implicit operator VectorField(float? value) => value == null ? V.Null : new VectorField(value.Value);

        static VectorField()
        {
            if (default(VectorField).Field != FieldType.UseOriginal)
                throw new System.InvalidProgramException($"Assert failed: default({nameof(VectorField)}) must be equal to Null");
        }
    }

    /// <summary>
    /// Symbolic constants to be used as arguments for <see cref="VectorHelpers.With(Vector3, VectorField, VectorField, VectorField)"/>
    /// </summary>
    public static class V
    {
        /// <summary>
        /// Set this field to the value of vector's X field
        /// </summary>
        public static readonly VectorField X = new VectorField(VectorField.FieldType.X);
        /// <summary>
        /// Set this field to the value of vector's Y field
        /// </summary>
        public static readonly VectorField Y = new VectorField(VectorField.FieldType.Y);
        /// <summary>
        /// Set this field to the value of vector's Z field
        /// </summary>
        public static readonly VectorField Z = new VectorField(VectorField.FieldType.Z);
        /// <summary>
        /// Use the original value of the vector field in mention
        /// </summary>
        public static readonly VectorField Null = default;
    }
    /// <summary>
    /// Symbolic constants to be used as arguments for <see cref="VectorHelpers.With(Color, VectorField, VectorField, VectorField, VectorField)/>
    /// </summary>
    public static class C
    {
        /// <summary>
        /// Set this field to the value of color's R field
        /// </summary>
        public static readonly VectorField R = new VectorField(VectorField.FieldType.R);
        /// <summary>
        /// Set this field to the value of color's G field
        /// </summary>
        public static readonly VectorField G = new VectorField(VectorField.FieldType.G);
        /// <summary>
        /// Set this field to the value of color's B field
        /// </summary>
        public static readonly VectorField B = new VectorField(VectorField.FieldType.B);
        /// <summary>
        /// Set this field to the value of color's A field
        /// </summary>
        public static readonly VectorField A = new VectorField(VectorField.FieldType.A);
        /// <summary>
        /// Use the original value of the color field in mention
        /// </summary>
        public static readonly VectorField Null = default;
    }



    /// <summary>
    /// Static class containing convenience extensions methods for vector types.
    /// </summary>
    public static class VectorHelpers
    {
        /// <summary>
        /// Normalizes the vector and when it's doing it, provides the magnitude as well.
        /// </summary>
        /// <param name="v">Vector to normalize</param>
        /// <param name="magnitude">Computed magnitude of <paramref name="v"/></param>
        /// <returns>Normalized variant of <paramref name="v"/></returns>
        public static Vector3 Normalized(this Vector3 v, out float magnitude)
        {
            magnitude = v.magnitude;
            if (magnitude > 1E-05f) //copypasted from decompiled builtin Vector3.Normalize()
                return v / magnitude;
            else return Vector3.zero;
        }

        /// <summary>
        /// Computes the vector's magnitude and when it's doing it, provides its normalized value as well.
        /// </summary>
        /// <param name="v">Vector to obtain its magnitude.</param>
        /// <param name="normalized">Normalized variant of <paramref name="v"/></param>
        /// <returns>Computed magnitude of <paramref name="v"/></returns>
        public static float Magnitude(this Vector3 v, out Vector3 normalized)
        {
            normalized = v.Normalized(out var ret);
            return ret;
        }

        /// <summary>
        /// Custom formating function for better vector debugging
        /// </summary>
        public static string ToStringPrecise(this Vector3 v)
        {
            if (v == Vector3.zero) return "<ZERO>";
            var magnitude = v.Magnitude(out var normalized);
            return $"{magnitude}*{normalized}";//$"({v.x};{v.y};{v.z})";
        }

        /// <summary>
        /// Replace certain components of the provided vector.
        /// 
        /// <para>Fields can be replaced by provided value, nothing or even other fields of the original vector.</para>
        /// </summary>
        /// <param name="self">Base vector value</param>
        /// <param name="x">Instruction for field X replacement</param>
        /// <param name="y">Instruction for field Y replacement</param>
        /// <param name="z">Instruction for field Z replacement</param>
        /// <returns>Result vector</returns>
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
        /// <summary>
        /// Replace certain components of the provided color.
        /// 
        /// <para>Fields can be replaced by provided value, nothing or even other fields of the original color.</para>
        /// </summary>
        /// <param name="self">Base color value</param>
        /// <param name="r">Instruction for field R replacement</param>
        /// <param name="g">Instruction for field G replacement</param>
        /// <param name="b">Instruction for field B replacement</param>
        /// <param name="a">Instruction for field A replacement</param>
        /// <returns>Result color</returns>
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

        /// <summary>
        /// Replace certain components of the provided euler angle.
        /// 
        /// <para>Fields can be replaced by provided value, nothing or even other fields of the original euler angle.</para>
        /// </summary>
        /// <param name="self">Base angle value</param>
        /// <param name="x">Instruction for field X replacement</param>
        /// <param name="y">Instruction for field Y replacement</param>
        /// <param name="z">Instruction for field Z replacement</param>
        /// <returns>Result angle</returns>
        public static Quaternion WithEuler(this Quaternion self, VectorField x = default, VectorField y = default, VectorField z = default)
            => Quaternion.Euler(self.eulerAngles.With(x, y, z));

        /// <summary>
        /// Interpret 3D vector as HSV color
        /// </summary>
        /// <param name="v">Vector with H,S,V encoded in its x,y,z</param>
        /// <returns>Interpreted color</returns>
        public static Color AsHSV(this Vector3 v) => Color.HSVToRGB(v.x, v.y, v.z);

        /// <summary>
        /// Convert given <see cref="Color"/> to <see cref="Vector3"/> with its x,y,z members containing H,S,V values
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns>Vector with H,S,V encoded in its x,y,z</returns>
        public static Vector3 AsVectorHSV(this Color c)
        {
            Color.RGBToHSV(c, out var h, out var s, out var v);
            return new Vector3(h, s, v);
        }

        /// <summary>
        /// Clamp vector inside provided interval fieldwise
        /// </summary>
        /// <param name="self">Vector to be clamped</param>
        /// <param name="i">Interval for clamping</param>
        /// <returns>Clamped result vector</returns>
        public static Vector3 ClampFields(this Vector3 self, Interval<Vector3> i)
            => new Vector3(Mathf.Clamp(self.x, i.Min.x, i.Max.x), Mathf.Clamp(self.y, i.Min.y, i.Max.y), Mathf.Clamp(self.z, i.Min.z, i.Max.z));



        /// <summary>
        /// Clamps vector interpreted as degree euler angles in an euler angles interval
        /// </summary>
        /// <param name="self">Euler angle to be clamped</param>
        /// <param name="i">Interval for clamping</param>
        /// <returns>Clamped euler angle</returns>
        public static Vector3 ClampEuler(this Vector3 self, Interval<Vector3> i)
        {
            float Fix(float f) => (f %= 360) >= 180f ? f - 360 : f;
            return new Vector3(Fix(self.x), Fix(self.y), Fix(self.z)).ClampFields(i);

            //TODO: Fix this so that it handles correctly intervals like <180°; 210°> aka. <180°;-150°> etc.

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



        /// <summary>
        /// Fluent shortcut for <see cref="Vector3.Distance(Vector3, Vector3)"/>
        /// </summary>
        /// <returns>Distance between the two vectors</returns>
        public static float Distance(this Vector3 self, Vector3 b) => Vector3.Distance(self, b);

        /// <summary>
        /// Checks if any of the fields of the vector is <c>NaN</c>
        /// </summary>
        /// <param name="v">Vector to be checked</param>
        /// <returns><c>true</c> IFF any of the fields of the vector is NaN</returns>
        public static bool IsNaN(this Vector3 v) => v.x.IsNaN() || v.y.IsNaN() || v.z.IsNaN();

        /// <summary>
        /// Fluent shortcut for <see cref="Vector3.Dot"/>
        /// </summary>
        /// <returns>Dot product of the two vectors</returns>
        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        /// <summary>
        /// Fluent shortcut for <see cref="Vector3.Cross"/>
        /// </summary>
        /// <returns>Cross product of the two vectors</returns>
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);

        /// <summary>
        /// Multiply two vectors fieldwise
        /// </summary>
        /// <returns>Multiplication result of the two vectors</returns>
        public static Vector3 MultiplyElems(this Vector3 a, float x, float y, float z) => new Vector3(a.x * x, a.y * y, a.z * z);
        /// <summary>
        /// Multiply two vectors fieldwise
        /// </summary>
        /// <returns>Multiplication result of the two vectors</returns>
        public static Vector3 MultiplyElems(this Vector3 a, Vector3 b) => a.MultiplyElems(b.x, b.y, b.z);

        /// <summary>
        /// Multiply two vectors fieldwise
        /// </summary>
        /// <returns>Multiplication result of the two vectors</returns>
        public static Vector2 MultiplyElems(this Vector2 a, float x, float y) => new Vector3(a.x * x, a.y * y);
        /// <summary>
        /// Multiply two vectors fieldwise
        /// </summary>
        /// <returns>Multiplication result of the two vectors</returns>
        public static Vector2 MultiplyElems(this Vector2 a, Vector2 b) => a.MultiplyElems(b.x, b.y);

        /// <summary>
        /// Extract just x field of vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>Vector with x field of <paramref name="v"/></returns>
        public static Vector2 x0(this Vector2 v) => new Vector2(v.x, 0);
        /// <summary>
        /// Extract just y field of vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>Vector with y field of <paramref name="v"/></returns>
        public static Vector2 _0y(this Vector2 v) => new Vector2(0, v.y);

        /// <summary>
        /// Extract just xy fields of 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with xy fields of <paramref name="v"/></returns>
        public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
        /// <summary>
        /// Extract just xz fields of 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with xz fields of <paramref name="v"/></returns>
        public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
        /// <summary>
        /// Extract just yz fields of 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with yz fields of <paramref name="v"/></returns>
        public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);
        /// <summary>
        /// Extract xyx fields of 2D vector into 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with xyx fields of <paramref name="v"/></returns>
        public static Vector3 xyx(this Vector2 v) => new Vector3(v.x, v.y, v.x);

        /// <summary>
        /// Extract x0y fields of 2D vector into 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with x0y fields of <paramref name="v"/></returns>
        public static Vector3 x0y(this Vector2 v) => new Vector3(v.x, 0, v.y);
        /// <summary>
        /// Extract 0yz fields of 2D vector into 3D vector
        /// </summary>
        /// <param name="v">Vector to be extracted</param>
        /// <returns>3D Vector with 0yz fields of <paramref name="v"/></returns>
        public static Vector3 _0yz(this Vector2 v) => new Vector3(0, v.x, v.y);
    }
}
