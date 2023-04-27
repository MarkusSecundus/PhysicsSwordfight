using JetBrains.Annotations;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using MathNet.Numerics.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;









public static class VectorHelpers
{
	public static readonly Vector3 NaNVector3 = new Vector3(float.NaN, float.NaN, float.NaN);
}


public static class AnimationHelpers
{
	public static AnimationCurve AnimationCurve01 => new AnimationCurve(
		new Keyframe { time=-0.1f, value=-0.1f, inTangent=0f, outTangent = 0f, inWeight=0, outWeight=1f/3f}, 
		new Keyframe { time=0.99f, value=-0.1f, inTangent=0f, outTangent = 0f, inWeight=1f/3f, outWeight=1}, 
		new Keyframe { time=1f, value=1.1f, inTangent=45f, outTangent = 45f, inWeight=1f/3f, outWeight=0}
	);
}

public static class GeometryHelpers
{
    public static Rect RectFromPoints(Vector2 a, Vector2 b)
    {
        if (a.x > b.x) (a.x, b.x) = (b.x, a.x);
        if (a.y > b.y) (a.y, b.y) = (b.y, a.y);
        return new Rect(a, b - a);
    }
    public static void SetRect(this RectTransform self, Rect toSet)
    {
        self.sizeDelta = toSet.size;
        self.position = toSet.center;
    }
    public static Rect GetRect(this RectTransform self)
    {
        if (self.parent == null) return self.rect;
        var (min, max) = (self.LocalToGlobal(self.rect.min), self.LocalToGlobal(self.rect.max));
        return RectFromPoints(min, max);
    }

    public static Rect AddSize(this Rect self, Vector2 toAdd)
        => new Rect(self.min, self.size + toAdd);
    public static Rect AddPosition(this Rect self, Vector2 toAdd)
        => new Rect(self.min + toAdd, self.size);

    public static Rect PositionsWherePlacingThisRectCoversTheWholeOfSmallerRect(this Rect biggerOne, Rect smallerRect)
	{
        Vector2 min = smallerRect.max - biggerOne.size / 2f, max = smallerRect.min + biggerOne.size / 2f;
        return RectFromPoints(min, max);
    }
	public static IEnumerable<Vector3> PointsOnCircle(int count = 1, Vector3 begin = default, Vector3 axis = default, bool includeBegin = true)
    {
        if (count < 1) throw new ArgumentException("Must be a positive number", nameof(count));
        Vector3 v = begin == default ? new Vector3(1, 0, 0) : begin;

        if (includeBegin)
            yield return v;

        var rot = Matrix4x4.Rotate(Quaternion.AngleAxis(NumericConstants.MaxDegree / count, axis == default ? Vector3.forward : axis));
        for (int t = 1; t < count; ++t)
            yield return v = rot * v;
    }

	public static Vector3 GetPointOnCircle(float angle_radians)
	{
		return new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians));
	}


    public static (double? x1, double? x2) SolveQuadraticEquation(double a, double b, double c)
	{
		double D = b * b - 4 * a * c;

		if (D < 0) return (null, null);


		if (D == 0)
		{
			double t = -b / (2 * a);
			return (t, null);
		}
		//D > 0
		double SqrtD = Math.Sqrt(D);
		double x1 = (-b + SqrtD) / (2 * a),
			   x2 = (-b - SqrtD) / (2 * a);
		return (x1, x2);
	}


    public static (double? t1, double? t2) IntersectSphere_GetParameter(this Ray self, Sphere sphere)
    {
		var (radius, centre) = (sphere.Radius, sphere.Center);

        if (self.direction == Vector3.zero)
            throw new ArgumentException(nameof(self), $"Direction must not be zero!");
		if (radius == 0)
			return (null, null);

		Vector3 o = self.origin - centre, d = self.direction;
        double a = (double)d.x * d.x + (double)d.y * d.y + (double)d.z*d.z; //non-zero if the direction is non-zero
        double b = 2 * ((double)o.x * d.x + (double)o.y * d.y + (double)o.z * d.z);
        double c = (double)o.x * o.x + (double)o.y * o.y + (double)o.z * o.z - radius * radius;

		return SolveQuadraticEquation(a, b, c);
    }

	public static Vector3 GetPoint(this Ray self, double t)
		=> self.origin + (float)t * self.direction;

	public static (Vector3? First, Vector3? Second) GetPointsFromParameters(this Ray self, (double? t1, double? t2) parameters)
	{
		(var t1, var t2) = parameters;

		return (
			t1 == null ? (Vector3?)null : self.GetPoint(t1.Value),
			t2 == null ? (Vector3?)null : self.GetPoint(t2.Value)
			);
	}

	public static (Vector3? First, Vector3? Second) IntersectSphere(this Ray self, Sphere sphere)
		=> self.GetPointsFromParameters(self.IntersectSphere_GetParameter(sphere));




	public static (double? t1, double? t2) IntersectParaboloid_GetParameter(this Ray self, Vector3 shift, float divider)
	{
		if (self.direction == Vector3.zero)
			throw new ArgumentException(nameof(self), $"Direction must not be zero!");
		if (divider == 0)
			throw new ArgumentException(nameof(divider), $"Divider must not be zero!");

		Vector3 o = self.origin - shift, d = self.direction;

		double a = (double)d.x * d.x + (double)d.y * d.y;
		double b = 2 * ((double)o.x * d.x + (double)o.y * d.y) - (double)divider * d.z;
		double c = (double)o.x * o.x + (double)o.y * o.y - (double)divider * o.z;

        return SolveQuadraticEquation(a,b,c);
    }


	public static (Vector3? First, Vector3? Second) IntersectParaboloid(this Ray self, Vector3 shift, float divider)
		=> self.GetPointsFromParameters(self.IntersectSphere_GetParameter(new Sphere(shift, divider)));




	public static double PointDistanceFromRay(this Ray self, Vector3 v)
    {
		Vector3 o = self.origin, s = self.direction;

		double dst = Vector3.Cross(s, o - v).sqrMagnitude / s.sqrMagnitude;

		return dst;
    }

	private struct ShortestRayConnectionResult
	{
		public Vector3 resultDirection;
		public double t1, t2, t3;
	}

	private static ShortestRayConnectionResult GetShortestRayConnection_impl(ScaledRay self, ScaledRay other)
    {
        ///
        /// We are searching for a line that connects self and other in the shortest way possible. We know such line must be orthogonal to both self and other -> self.direction = self.direction.Cross(other.direction)
        /// Wow the only thing we need to find is result.origin.
        /// Lets assume we want result.origin to lie on self - then 
        ///		result.origin = self.origin + t1*self.direction for some t1
        /// Also result.origin + t3*result.direction = other.origin + t2*other.direction for some t3, t2 (the intersection of result with other)
        /// by substituting result.origin for the first line, we get:
        /// self.origin + t1*self.direction + t3*result.direction = other.origin + t2*other.direction
        /// By solving this system of linear equations (3 equations memberwise for xs, ys and zs) we obtain the 3 parameters that give us the result
        /// Default shape of the equation system is 
        ///		t1*self.direction + t2*(-other.direction) + t3*result.direction = -self.origin + other.origin
        /// 

        var resultDirection = self.direction.Cross(other.direction);

        Vector3 a = self.direction, b = -other.direction, c = resultDirection, d = -self.origin + other.origin;

        var equationParams = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { a.x, b.x, c.x},
            { a.y, b.y, c.y},
            { a.z, b.z, c.z}
        });
        var constants = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(new double[] { d.x, d.y, d.z });

        var solution = equationParams.Solve(constants);

        double t1 = solution[0], t2 = solution[1], t3 = solution[2];

		return new ShortestRayConnectionResult { resultDirection = resultDirection, t1 = t1, t2 = t2, t3 = t3 };
    }
	public static ScaledRay GetShortestRayConnection(this ScaledRay self, ScaledRay other)
	{
		var result = GetShortestRayConnection_impl(self, other);

        var resultOrigin = self.origin + (float)result.t1 * self.direction;
        var resultEnd = other.origin + (float)result.t2 * other.direction;
        return ScaledRay.FromPoints(resultOrigin, resultEnd);
    }

	//Same as GetShortestRayConnection(..) but considers the ray to represent finite line segments (that have a beginning and an end)
	public static ScaledRay GetShortestScaledRayConnection(this ScaledRay self, ScaledRay other)
	{
		var result = GetShortestRayConnection_impl(self, other);

        var resultOrigin = self.origin + Mathf.Clamp01((float)result.t1) * self.direction;
		var resultEnd = other.origin + Mathf.Clamp01((float)result.t2) * other.direction;
		return ScaledRay.FromPoints(resultOrigin, resultEnd);
	}


	public static double GetRayPointWithLeastDistance_GetParameter(this Ray self, Vector3 v)
    {
		return -Vector3.Dot((self.origin - v), self.direction) / self.direction.magnitude.Pow2();
    }

	public static float GetDistance(this Ray self, Vector3 v)
		=> self.GetRayPointWithLeastDistance(v).Distance(v);
	public static Vector3 GetRayPointWithLeastDistance(this Ray self, Vector3 v)
		=> self.GetPoint(self.GetRayPointWithLeastDistance_GetParameter(v));

    public static Ray TransformRay(this Transform self, Ray r) 
		=> new Ray(self.TransformPoint(r.origin), self.TransformDirection(r.direction));

    public static Ray InverseTransformRay(this Transform self, Ray r)
		=> new Ray(self.InverseTransformPoint(r.origin), self.InverseTransformDirection(r.direction));


	/// <summary>
	/// Computes angles of a triangle (described by the lengths of its sides) using the cosine theorem
	/// </summary>
	/// <returns></returns>
	public static (float a, float b, float c) GetTriangleAngles_sss(float lengthA, float lengthB, float lengthC)
	{
		var cosA = getCos(lengthA, lengthB, lengthC);
		var cosB = getCos(lengthB, lengthC, lengthA);
		var cosC = getCos(lengthC, lengthA, lengthB);

		return (Mathf.Acos(cosA)*Mathf.Rad2Deg, Mathf.Acos(cosB) * Mathf.Rad2Deg, Mathf.Acos(cosC) * Mathf.Rad2Deg);

		float getCos(float a, float b, float c) => (b * b + c * c - a * a) / (2 * b * c);
    }

	public static float GetAngularDifference(this Vector3 a, Vector3 b)
	{
		return GetTriangleAngles_sss(a.magnitude, b.magnitude, a.Distance(b)).c;
	}


	/// <summary>
	/// According to cosine lemma
	/// </summary>
	/// <returns>Angle in radians</returns>
	public static double ComputeTriangleAngle_sss(double oppositeSide, double adjacent1, double adjacent2)
    {
		if (adjacent1 == 0) throw new ArgumentException("Adjacent sides cannot be zero!", nameof(adjacent1));
		if (adjacent2 == 0) throw new ArgumentException("Adjacent sides cannot be zero!", nameof(adjacent2));

		double cos = (adjacent1 * adjacent1 + adjacent2 * adjacent2 - oppositeSide * oppositeSide) / (2 * adjacent1 * adjacent2);


		return Math.Acos(cos);
    }

	public static float Distance(this Vector3 self, Vector3 b) => Vector3.Distance(self, b);

	public static bool IsNegligible(this float f, float? epsilon=null) => Mathf.Abs(f) < (epsilon?? Mathf.Epsilon);
	public static bool IsCloseTo(this float f, float g, float? epsilon=null) => (f-g).IsNegligible(epsilon);
}


public static class PlaneHelpers
{
	private static readonly Matrix4x4 rotate90x = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
	private static readonly Matrix4x4 rotate90y = Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0));
	private static readonly Matrix4x4 rotate90z = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
	public static (Vector3 X, Vector3 Y) GetBase(this Plane plane)
	{
		Vector3 v1;
		if((v1 = new Vector3(plane.normal.y, -plane.normal.x, 0)) == Vector3.zero && (v1 = new Vector3(plane.normal.z, 0, -plane.normal.x)) == Vector3.zero )
            v1 = new Vector3(0, -plane.normal.z, plane.normal.y);

		v1 = v1.normalized;
		/*v1 = plane.ClosestPointOnPlane(Vector3.one) - plane.GetShift();
		v1 = plane.normal.Cross(v1).normalized;*/

		/*if (!(v1 = rotate90x * plane.normal).Dot(plane.normal).IsNegligible() && !(v1 = rotate90y * plane.normal).Dot(plane.normal).IsNegligible())
			v1 = rotate90z * plane.normal;
		if (!v1.Dot(plane.normal).IsNegligible()) v1 = Vector3.Cross(plane.normal, v1);// Debug.Log($"Cross: {v1.Dot(plane.normal)}");*/
        //v1 = rotate90x * plane.normal;
        return (v1, Vector3.Cross(plane.normal, v1).normalized);
    }

	public static Vector3 GetBasedVector(this (Vector3 X, Vector3 Y) coordsBase, Vector2 v)
	{
		return v.x * coordsBase.X + v.y * coordsBase.Y;
	}

	public static Vector3 GetShift(this Plane self)
		=> self.ClosestPointOnPlane(Vector3.zero);//throw new NotImplementedException("Plane shift is yet to be implemented!");//self.normal * self.distance; //DOESN'T WORK!!!!


    public static Plane GetTangentialPlane(this Sphere sphere, Vector3 point)
    {
        //if (!point.Distance(sphere.Center).IsCloseTo(sphere.Radius)) throw new ArgumentException($"The point doesn't lay on the sphere");
        return new Plane(point - sphere.Center, point);
    }

	public static Vector3 ProjectPoint(this Sphere sphere, Vector3 point)
	{
		var direction = (point - sphere.Center).normalized;
		return sphere.Center + direction * sphere.Radius;
	}
}



