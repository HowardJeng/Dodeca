namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Input;

  internal enum ActionMode {
    User,
    Auto
  }

  internal class OrientState : BaseState {
    private Quaternion goalOrientation;
    private ActionMode mode;
    private IGameState next;
    private TimeSpan timeLeft;

    public OrientState(SharedState shared, ActionMode mode, uint face, IGameState next)
      : base(shared) {
      this.next = next;
      this.mode = mode;
      this.SetGoalOrientation(face);
    }

    public override IGameState KeyDown(Keys key) {
      if (this.mode == ActionMode.Auto) { return this; }
      switch (key) {
        case Keys.Escape:
          return null;
        case Keys.D1:
          this.SetGoalOrientation(0);
          break;
        case Keys.D2:
          this.SetGoalOrientation(1);
          break;
        case Keys.D3:
          this.SetGoalOrientation(2);
          break;
        case Keys.D4:
          this.SetGoalOrientation(3);
          break;
        case Keys.D5:
          this.SetGoalOrientation(4);
          break;
        case Keys.D6:
          this.SetGoalOrientation(5);
          break;
        case Keys.D7:
          this.SetGoalOrientation(6);
          break;
        case Keys.D8:
          this.SetGoalOrientation(7);
          break;
        case Keys.D9:
          this.SetGoalOrientation(8);
          break;
        case Keys.D0:
          this.SetGoalOrientation(9);
          break;
        case Keys.OemMinus:
          this.SetGoalOrientation(10);
          break;
        case Keys.OemPlus:
          this.SetGoalOrientation(11);
          break;
      }
      return this;
    }

    public override IGameState Update(TimeSpan frameTime) {
      Shared.Update(frameTime);
      if (frameTime > this.timeLeft) {
        this.ModelOrientation = this.goalOrientation;
        return this.next;
      }
      this.ModelOrientation = Quaternion.Slerp(this.ModelOrientation, 
                                               this.goalOrientation, 
                                               (float)frameTime.Ticks / this.timeLeft.Ticks);
      this.timeLeft -= frameTime;
      return this;
    }

    private void SetGoalOrientation(uint face) {
      Vector3 from = Vector3.Transform(DodecaModel.Normal(face), ModelOrientation);
      Vector3 to = Vector3.UnitZ;

      Quaternion q = Utility.ShortestArc(from, to);
      this.goalOrientation = q * this.ModelOrientation;
      this.goalOrientation.Normalize();

      double distance = Math.Acos(q.W);
      if (this.mode == ActionMode.User) {
        this.timeLeft = TimeSpan.FromSeconds(distance * 1.0);
      } else {
        this.timeLeft = TimeSpan.FromSeconds(distance * 0.5);
      }
    }
  }
}