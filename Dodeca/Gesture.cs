namespace Dodeca {
  using Microsoft.Xna.Framework;

  internal class GestureState : BaseState {
    private Vector3 startPosition;

    internal GestureState(SharedState shared, float screenX, float screenY)
      : base(shared) {
      this.startPosition = shared.Trackball.TrackballCoordinate(screenX, screenY);
    }

    public override IGameState MouseButtonUp(MouseButtons button, int x, int y) {
      if (button == MouseButtons.RightButton) {
        return new BasalState(this.Shared);
      }
      return this;
    }

    public override IGameState MouseMove(int newX, int newY, int deltaX, int deltaY) {
      Vector3 newCoordinate = Shared.Trackball.TrackballCoordinate(newX, newY);
      float distance = (newCoordinate - this.startPosition).Length();
      if (distance < 0.1f) { 
        return this;
      }
      uint face = DodecaModel.NearFace(Shared.ModelOrientation);
      Quaternion arc = Utility.ShortestArc(this.startPosition, newCoordinate);
      if (arc.Z < 0) {
        return new RotateState(Shared, ActionMode.User, Rotation.Right, face);
      } else {
        return new RotateState(Shared, ActionMode.User, Rotation.Left, face);
      }
    }
  }
}