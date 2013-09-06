namespace Dodeca {
  using System;
  using System.Diagnostics;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  internal class Trackball {
    private Vector3 center;

    private float radius;

    internal Trackball(Matrix projection, Matrix view, Viewport viewport) {
      this.center = viewport.Project(Vector3.Zero, projection, view, Matrix.Identity);
      Vector3 edge = viewport.Project(Vector3.UnitX, projection, view, Matrix.Identity);
      this.radius = (this.center - edge).Length();
    }

    internal Vector3 TrackballCoordinate(float screenX, float screenY) {
      Vector3 virtCoord;
      virtCoord.X = (screenX - this.center.X) / this.radius;
      virtCoord.Y = -(screenY - this.center.Y) / this.radius; // invert y
      virtCoord.Z = 0;

      float radiusSquared = virtCoord.LengthSquared();
      if (radiusSquared < 1) {
        virtCoord.Z = (float)Math.Sqrt(1 - radiusSquared);
      } else {
        virtCoord.Normalize();
      }
      Debug.Assert(Utility.EffectivelyEqual(virtCoord.LengthSquared(), 1), "virtual coordinate must be normalized");
      return virtCoord;
    }
  }

  internal class TrackballState : BaseState {
    private Vector3 lastPosition;

    public TrackballState(SharedState shared, int x, int y)
      : base(shared) {
      this.lastPosition = this.Shared.Trackball.TrackballCoordinate(x, y);
    }

    public override IGameState MouseButtonUp(MouseButtons button, int x, int y) {
      if (button == MouseButtons.LeftButton) {
        return new BasalState(Shared);
      }
      return this;
    }

    public override IGameState MouseMove(int newX, int newY, int deltaX, int deltaY) {
      Vector3 virtualCoordinate = Shared.Trackball.TrackballCoordinate(newX, newY);

      Quaternion arc = Utility.ShortestArc(this.lastPosition, virtualCoordinate);
      this.ModelOrientation = arc * this.Shared.ModelOrientation;
      this.ModelOrientation.Normalize();
      this.lastPosition = virtualCoordinate;

      return this;
    }
  }
}