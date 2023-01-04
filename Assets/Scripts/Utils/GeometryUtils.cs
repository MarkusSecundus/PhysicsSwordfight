using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Sphere
{
	public Sphere(Vector3 center, float radius) => (Center, Radius) = (center, radius);

	public Vector3 Center;
	public float Radius;
}

[System.Serializable]
public class JointRotationHelper
{
    public ConfigurableJoint Joint { get; }
    public Space Space { get; }

	[SerializeField]
    internal Quaternion startRotation;

    public JointRotationHelper(ConfigurableJoint joint)
    {
        this.Space = joint.configuredInWorldSpace? Space.World : Space.Self;
        this.Joint = joint;
        startRotation = Space switch
        {
            Space.Self => joint.transform.localRotation,
            Space.World => joint.transform.rotation,
            _ => throw new ArgumentException($"Invalid value `{Space}` provided for {nameof(Space)}")
        };
		CurrentRotation = startRotation;
    }

	public Quaternion CurrentRotation { get; private set; }

    public void SetTargetRotation(Quaternion newTargetRotation)
        => ConfigurableJointExtensions.SetTargetRotationInternal(Joint, CurrentRotation = newTargetRotation, startRotation, Space);
}


/// <summary>
/// All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/>
/// </summary>
public static class ConfigurableJointExtensions
{
    public static JointRotationHelper MakeRotationHelper(this ConfigurableJoint self) => new JointRotationHelper(self);
    /// <summary>
    /// Sets a joint's targetRotation to match a given local rotation.
    /// The joint transform's local rotation must be cached on Start and passed into this method.
    /// </summary>
    public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
	{
		if (joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
		}
		SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
	}

	/// <summary>
	/// Sets a joint's targetRotation to match a given world rotation.
	/// The joint transform's world rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
	{
		if (!joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
		}
		SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
	}

	public static Quaternion ComputeTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = joint.axis;
		var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
		var up = Vector3.Cross(forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		if (space == Space.World)
		{
			resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
		}
		else
		{
			resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
		}

		// Transform back into joint space
		resultRotation *= worldToJointSpace;

		// Return our newly calculated rotation
		return resultRotation;
	}
    public static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		// Set target rotation to our newly calculated rotation
		joint.targetRotation = ComputeTargetRotationInternal(joint, targetRotation, startRotation, space);
	}


	/*
	 * TODO: implement!
	 * internal static void GetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = joint.axis;
		var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
		var up = Vector3.Cross(forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		if (space == Space.World)
		{
			resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
		}
		else
		{
			resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
		}

		// Transform back into joint space
		resultRotation *= worldToJointSpace;

		// Set target rotation to our newly calculated rotation
		joint.targetRotation = resultRotation;
	}*/
}



public static class MeshUtils
{

	public static void CreateFlatMeshData(Vector2 begin, Vector2 steps, Vector2Int stepsCount, out Vector3[] vertices, out int[] triangles)
	{
		vertices = new Vector3[stepsCount.x * stepsCount.y];
		{
			int i = 0, y = 0;
			for (float yf = begin.y; y < stepsCount.y; ++y, yf += steps.y)
			{
				int x = 0;
				for (float xf = begin.x; x < stepsCount.x; ++x, xf += steps.x)
					vertices[i++] = new Vector3(xf, yf, 0);
			}
		}

		triangles = new int[(stepsCount.x - 1) * (stepsCount.y - 1) * 12];

		{int i = 0;
			for (int z = 0; z < stepsCount.y - 1; ++z)
				for (int x = 0; x < stepsCount.x - 1; ++x)
				{
					triangles[i++] = ((z * stepsCount.x) + x);
					triangles[i++] = ((z * stepsCount.x) + x + stepsCount.x);
					triangles[i++] = ((z * stepsCount.x) + x + 1);
					triangles[i++] = ((z * stepsCount.x) + x + 1);
					triangles[i++] = ((z * stepsCount.x) + x + stepsCount.x);
					triangles[i++] = ((z * stepsCount.x) + x + 1 + stepsCount.x);


					triangles[i++] = ((z * stepsCount.x) + x + 1);
					triangles[i++] = ((z * stepsCount.x) + x + stepsCount.x);
					triangles[i++] = ((z * stepsCount.x) + x);

					triangles[i++] = ((z * stepsCount.x) + x + 1 + stepsCount.x);
					triangles[i++] = ((z * stepsCount.x) + x + stepsCount.x);
					triangles[i++] = ((z * stepsCount.x) + x + 1);
				}
		}
	}

	public static Mesh MeshFromData(Vector3[] vertices, int[] triangles)
    {
		var m = new Mesh();
		m.vertices = vertices;
		m.triangles = triangles;
		m.RecalculateNormals();
		return m;
    }


	public static Vector3[] AdjustHeights(this Vector3[] self, Func<float, float, float> f)
	{
		if (self == null) return self;

		for (int t = 0; t < self.Length; ++t)
		{
			var v = self[t];
			self[t] = new Vector3(v.x, v.y, f(v.x, v.y));
		}

		return self;
	}






}

public static class DebugDrawUtils
{
	public static void DrawRay(Ray r, Color col, float duration = 1f, float lengthMultiply = 100f)
	{
		Debug.DrawRay(r.origin - r.direction * lengthMultiply, r.origin + r.direction * lengthMultiply, col, duration);
	}
}



public static class MathUtils
{
	public static double Pow2(this double d) => d * d;
	public static float Pow2(this float d) => d * d;
}

public static class GeometryUtils
{
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
			throw new ArgumentException(nameof(sphere), $"Radius must not be zero!");

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

	public static bool IsNegligible(this float f) => Mathf.Abs(f) < Mathf.Epsilon;
	public static bool IsNegligible(this float f, float epsilon) => Mathf.Abs(f) < epsilon;
	public static bool IsCloseTo(this float f, float g) => (f-g).IsNegligible();

	public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
	public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);

	public static Vector3 MultiplyElems(this Vector3 a, float x, float y, float z) => new Vector3(a.x *x, a.y * y, a.z * z);
	public static Vector3 MultiplyElems(this Vector3 a, Vector3 b) => a.MultiplyElems(b.x, b.y, b.z);

	public static Vector2 MultiplyElems(this Vector2 a, float x, float y) => new Vector3(a.x *x, a.y * y);
	public static Vector2 MultiplyElems(this Vector2 a, Vector2 b) => a.MultiplyElems(b.x, b.y);


    public static Vector2 x0(this Vector2 v) => new Vector2(v.x, 0);
    public static Vector2 _0y(this Vector2 v) => new Vector2(0, v.y);

    public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);

    public static Vector3 xy0(this Vector2 v) => new Vector3(v.x, v.y, 0);
    public static Vector3 x0z(this Vector2 v) => new Vector3(v.x, 0, v.y);
    public static Vector3 _0yz(this Vector2 v) => new Vector3(0, v.x, v.y);
}


public static class PlaneExtensions
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




public static class PhysicsUtils
{

}

public static class CollectionsUtils
{
	public static IEnumerable<(T First, T Second)> AllCombinations<T>(this IReadOnlyList<T> l)
	{
		for (int t = 0; t < l.Count; ++t)
			for (int u = 0/*t+1*/; u < l.Count; ++u)
				yield return (l[t], l[u]);
	}
}