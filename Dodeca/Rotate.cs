namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  internal class RotateState : BaseState {
    private float currentAngle;
    private Rotation direction;
    private uint face;
    private float goalAngle;
    private IGameState next;
    private TimeSpan timeLeft;

    public RotateState(SharedState shared, ActionMode mode, Rotation direction, uint face, IGameState next)
      : base(shared) {
      shared.MoveTracker.AddMove(face, direction);

      this.currentAngle = 0;
      if (direction == Rotation.Left) {
        this.goalAngle = (float)DodecaModel.Deg72;
      } else {
        this.goalAngle = (float)-DodecaModel.Deg72;
      }
      this.face = face;
      this.direction = direction;
      if (mode == ActionMode.Auto) {
        this.timeLeft = TimeSpan.FromSeconds(0.2);
      } else {
        this.timeLeft = TimeSpan.FromSeconds(0.5);
      }
      this.next = next;
    }

    public RotateState(SharedState shared, ActionMode mode, Rotation direction, uint face)
      : this(shared, mode, direction, face, new BasalState(shared)) {
    }

    public override void Render(GraphicsDevice device) {
      this.Shared.Render(device, this.face, this.currentAngle);
    }

    public override IGameState Update(TimeSpan frameTime) {
      if (frameTime > this.timeLeft) {
        if (this.direction == Rotation.Left) {
          Model.RotateLeft(this.face);
        } else {
          Model.RotateRight(this.face);
        }
        return this.next;
      }
      TimeSpan newTimeLeft = this.timeLeft - frameTime;
      this.currentAngle = ((this.goalAngle * frameTime.Ticks) + (this.currentAngle * newTimeLeft.Ticks)) / this.timeLeft.Ticks;
      this.timeLeft = newTimeLeft;
      return this;
    }
  }
}