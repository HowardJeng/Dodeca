namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Content;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  internal enum MouseButtons {
    LeftButton, MiddleButton, RightButton
  }

  internal interface IGameState {
    IGameState KeyDown(Keys key);

    IGameState KeyUp(Keys key);

    IGameState MouseButtonDown(MouseButtons button, int x, int y);

    IGameState MouseButtonUp(MouseButtons button, int x, int y);

    IGameState MouseMove(int newX, int newY, int deltaX, int deltaY);

    void Render(GraphicsDevice gdm);

    IGameState Update(TimeSpan frameTime);
  }

  internal struct StateHolder {
    private IGameState state;

    public bool Valid {
      get { return this.state != null; }
    }

    public bool KeyDown(Keys key) {
      if (this.Valid) { this.state = this.state.KeyDown(key); }
      return this.Valid;
    }

    public bool KeyUp(Keys key) {
      if (this.Valid) { this.state = this.state.KeyUp(key); }
      return this.Valid;
    }

    public bool MouseButtonDown(MouseButtons button, int x, int y) {
      if (this.Valid) { this.state = this.state.MouseButtonDown(button, x, y); }
      return this.Valid;
    }

    public bool MouseButtonUp(MouseButtons button, int x, int y) {
      if (this.Valid) { this.state = this.state.MouseButtonUp(button, x, y); }
      return this.Valid;
    }

    public bool MouseMove(int newX, int newY, int deltaX, int deltaY) {
      if (this.Valid) { this.state = this.state.MouseMove(newX, newY, deltaX, deltaY); }
      return this.Valid;
    }

    public void Render(GraphicsDevice device) {
      if (this.Valid) { this.state.Render(device); }
    }

    public void SetState(IGameState s) {
      this.state = s;
    }

    public bool Update(TimeSpan frameTime) {
      if (this.Valid) { this.state = this.state.Update(frameTime); }
      return this.Valid;
    }
  }

  internal class BaseState : IDisposable, IGameState {
    private bool disposed = false;
    private SharedState shared;

    public BaseState(SharedState state) {
      this.shared = state;
    }

    public BaseState(GraphicsDeviceManager gdm, ContentManager content) {
      this.shared = new SharedState(gdm, content);
    }

    ~BaseState() {
      this.Dispose(false);
    }

    protected DodecaModel Model {
      get { return this.shared.Model; }
    }

    protected Quaternion ModelOrientation {
      get { return this.shared.ModelOrientation; }
      set { this.shared.ModelOrientation = value; }
    }

    protected SharedState Shared {
      get { return this.shared; }
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public virtual IGameState KeyDown(Keys key) {
      return this;
    }

    public virtual IGameState KeyUp(Keys key) {
      return this;
    }

    public virtual IGameState MouseButtonDown(MouseButtons button, int x, int y) {
      return this;
    }

    public virtual IGameState MouseButtonUp(MouseButtons button, int x, int y) {
      return this;
    }

    public virtual IGameState MouseMove(int newX, int newY, int deltaX, int deltaY) {
      return this;
    }

    public virtual void Render(GraphicsDevice device) {
      this.shared.Render(device);
    }

    public virtual IGameState Update(TimeSpan frameTime) {
      this.shared.Update(frameTime);
      return this;
    }

    protected virtual void Dispose(bool disposing) {
      if (this.disposed) { return; }
      if (disposing) {
        if (!object.ReferenceEquals(null, this.shared)) {
          this.shared.Dispose();
          this.shared = null;
        }
      }
      this.disposed = true;
    }
  }
}