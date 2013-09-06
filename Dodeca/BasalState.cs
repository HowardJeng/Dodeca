namespace Dodeca {
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Content;
  using Microsoft.Xna.Framework.Input;

  internal enum Rotation {
    Right,
    Left
  }

  internal class BasalState : BaseState {
    public BasalState(SharedState shared)
      : base(shared) {
    }

    public BasalState(GraphicsDeviceManager gdm, ContentManager content)
      : base(gdm, content) {
    }

    public override IGameState KeyDown(Keys key) {
      switch (key) {
        case Keys.Escape:
          return null;
        case Keys.D1:
          return new OrientState(Shared, ActionMode.User, 0, this);
        case Keys.D2:
          return new OrientState(Shared, ActionMode.User, 1, this);
        case Keys.D3:
          return new OrientState(Shared, ActionMode.User, 2, this);
        case Keys.D4:
          return new OrientState(Shared, ActionMode.User, 3, this);
        case Keys.D5:
          return new OrientState(Shared, ActionMode.User, 4, this);
        case Keys.D6:
          return new OrientState(Shared, ActionMode.User, 5, this);
        case Keys.D7:
          return new OrientState(Shared, ActionMode.User, 6, this);
        case Keys.D8:
          return new OrientState(Shared, ActionMode.User, 7, this);
        case Keys.D9:
          return new OrientState(Shared, ActionMode.User, 8, this);
        case Keys.D0:
          return new OrientState(Shared, ActionMode.User, 9, this);
        case Keys.OemMinus:
          return new OrientState(Shared, ActionMode.User, 10, this);
        case Keys.OemPlus:
          return new OrientState(Shared, ActionMode.User, 11, this);
        case Keys.L:
          return new RotateState(Shared, ActionMode.User, Rotation.Left, DodecaModel.NearFace(ModelOrientation));
        case Keys.R:
          return new RotateState(Shared, ActionMode.User, Rotation.Right, DodecaModel.NearFace(ModelOrientation));
        case Keys.Z:
          if (this.Shared.MoveTracker.HasUndoMoves()) {
            Move m = this.Shared.MoveTracker.GetUndoMove();
            RotateState r = new RotateState(Shared, ActionMode.User, m.Direction, m.Face);
            OrientState o = new OrientState(Shared, ActionMode.Auto, m.Face, r);
            return o;
          }
          break;
        case Keys.Y:
          if (this.Shared.MoveTracker.HasRedoMoves()) {
            Move m = this.Shared.MoveTracker.GetRedoMove();
            RotateState r = new RotateState(Shared, ActionMode.User, m.Direction, m.Face);
            OrientState o = new OrientState(Shared, ActionMode.Auto, m.Face, r);
            return o;
          }
          break;          
      }
      return this;
    }

    public override IGameState MouseButtonDown(MouseButtons button, int x, int y) {
      if (button == MouseButtons.LeftButton) {
        return new TrackballState(Shared, x, y);
      } else if (button == MouseButtons.RightButton) {
        return new GestureState(Shared, x, y);
      }
      return this;
    }
  }
}