namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;

  internal class GestureState : BaseState {
    private Vector2 startPosition;
    private Vector2 faceCenter;
    private uint face;

    internal GestureState(SharedState shared, uint face, float screenX, float screenY)
      : base(shared) {
      this.face = face;
      this.startPosition = new Vector2(screenX, screenY);
      this.faceCenter = shared.FaceCenterScreenCoordinates(face);
    }

    public override IGameState MouseButtonUp(MouseButtons button, int x, int y) {
      if (button == MouseButtons.RightButton) {
        return new BasalState(this.Shared);
      }
      return this;
    }

    public override IGameState MouseMove(int newX, int newY, int deltaX, int deltaY) {
      Vector2 newCoordinate = new Vector2(newX, newY);
      Vector3 from = new Vector3(this.startPosition - this.faceCenter, 0);
      from.Normalize();
      Vector3 to = new Vector3(newCoordinate - this.faceCenter, 0);
      to.Normalize();

      Vector3 cross = Vector3.Cross(from, to);
      if (Math.Abs(cross.Z) < 0.1) { return this; }

      if (cross.Z > 0) {
        return new RotateState(Shared, ActionMode.User, Rotation.Right, face);
      } else {
        return new RotateState(Shared, ActionMode.User, Rotation.Left, face);
      }
    }
  }
}