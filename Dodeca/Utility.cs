namespace Dodeca {
  using System;
  using System.Diagnostics;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  internal static class Utility {
    internal static bool EffectivelyZero(float value) {
      return Math.Abs(value) < 0.00001f;
    }

    internal static bool EffectivelyEqual(float value1, float value2) {
      if ((Math.Abs(value1) > 1.0) || (Math.Abs(value2) > 1.0)) {
        if (EffectivelyZero(value2)) { return false; }
        return EffectivelyZero((value1 / value2) - 1.0f);
      }
      return EffectivelyZero(value1 - value2);
    }

    internal static Quaternion ShortestArc(Vector3 from, Vector3 to) {
      Debug.Assert(EffectivelyEqual(from.LengthSquared(), 1), "from should be normalized");
      Debug.Assert(EffectivelyEqual(to.LengthSquared(), 1), "to should be normalized");
      float d = Vector3.Dot(from, to);
      if (EffectivelyEqual(d, 1)) {
        return new Quaternion(0, 0, 0, 1);
      }
      if (EffectivelyEqual(d, -1)) {
        // need to create some vector perpendicular to both.
        // try cross product with (0, 0, 1) first
        Vector3 p = Vector3.Cross(from, Vector3.UnitZ);
        if (EffectivelyZero(p.LengthSquared())) {
          // from and to parallel to (0, 0, 1) so try (1, 0, 0)
          p = Vector3.Cross(from, Vector3.UnitX);
          Debug.Assert(!EffectivelyZero(p.LengthSquared()), "generated axis should be non-zero");
        }
        p.Normalize();
        return Quaternion.CreateFromAxisAngle(p, MathHelper.Pi);
      }
      Vector3 c = Vector3.Cross(from, to);
      float angle = (float)Math.Atan2(c.Length(), d);
      c.Normalize();
      return Quaternion.CreateFromAxisAngle(c, angle);
    }

    internal static double AngularDistance(Vector3 from, Vector3 to) {
      return AngularDistance(ShortestArc(from, to));
    }

    internal static double AngularDistance(Quaternion q) {
      return Math.Atan2(new Vector3(q.X, q.Y, q.Z).Length(), q.W) * 2.0;
    }

    internal static float? RayTriangleIntersection(Ray ray, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
      // Moller-Trumbore 1997 ray-triangle test
      Vector3 e1 = vertex2 - vertex1;
      Vector3 e2 = vertex3 - vertex1;

      Vector3 p = Vector3.Cross(ray.Direction, e2);
      float determinant = Vector3.Dot(e1, p);

      // ray is parallel to triangle plane
      if (Utility.EffectivelyZero(determinant)) return null;

      Vector3 t = ray.Position - vertex1;
      float u = Vector3.Dot(t, p) / determinant;
      // intersection is outside area defined by first edge
      if (u < 0 || u > 1) return null;

      Vector3 q = Vector3.Cross(t, e1);

      float v = Vector3.Dot(ray.Direction, q) / determinant;
      // intersection outside triangle
      if (v < 0 || (u + v) > 1) return null;

      float rayDistance = Vector3.Dot(e2, q) / determinant;
      if (rayDistance < 0) return null;
      return rayDistance;
    }
  }
}