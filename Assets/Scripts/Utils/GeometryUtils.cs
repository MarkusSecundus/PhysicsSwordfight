using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class JointRotationHelper
{
    public ConfigurableJoint Joint { get; }
    public Space Space { get; }

	[SerializeField]
    internal Quaternion startRotation;

    public JointRotationHelper(ConfigurableJoint joint, Space space)
    {
        this.Space = space;
        this.Joint = joint;
        startRotation = space switch
        {
            Space.Self => joint.transform.localRotation,
            Space.World => joint.transform.rotation,
            _ => throw new ArgumentException($"Invalid value `{space}` provided for {nameof(space)}")
        };
    }

    public void SetTargetRotation(Quaternion newTargetRotation)
        => ConfigurableJointExtensions.SetTargetRotationInternal(Joint, newTargetRotation, startRotation, Space);
}


/// <summary>
/// All credit goes to mstevenson <see href="https://gist.github.com/mstevenson/4958837"/>
/// </summary>
public static class ConfigurableJointExtensions
{
    public static JointRotationHelper MakeRotationHelper(this ConfigurableJoint self, Space space) => new JointRotationHelper(self, space);
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

	internal static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
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
	}
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


    public static (double? t1, double? t2) IntersectSphere_GetParameter(this Ray self, Vector3 centre, float radius)
    {
        if (self.direction == Vector3.zero)
            throw new ArgumentException(nameof(self), $"Direction must not be zero!");
		if (radius == 0)
			throw new ArgumentException(nameof(radius), $"Radius must not be zero!");

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

	public static (Vector3? First, Vector3? Second) IntersectSphere(this Ray self, Vector3 centre, float radius)
		=> self.GetPointsFromParameters(self.IntersectSphere_GetParameter(centre, radius));




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
		=> self.GetPointsFromParameters(self.IntersectSphere_GetParameter(shift, divider));



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

	public static Vector3 GetRayPointWithLeastDistance(this Ray self, Vector3 v)
		=> self.GetPoint(self.GetRayPointWithLeastDistance_GetParameter(v));

    public static Ray TransformRay(this Transform self, Ray r) 
		=> new Ray(self.TransformPoint(r.origin), self.TransformDirection(r.direction));

    public static Ray InverseTransformRay(this Transform self, Ray r)
		=> new Ray(self.InverseTransformPoint(r.origin), self.InverseTransformDirection(r.direction));




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