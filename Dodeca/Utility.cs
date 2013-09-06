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
      float angle = (float)Math.Acos(d);
      Vector3 c = Vector3.Cross(from, to);
      c.Normalize();
      return Quaternion.CreateFromAxisAngle(c, angle);
    }

    internal static double AngularDistance(Vector3 from, Vector3 to) {
      Quaternion dist = ShortestArc(from, to);
      return Math.Acos(dist.W);
    }
  }
}