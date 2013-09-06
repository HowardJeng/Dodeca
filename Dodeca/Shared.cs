namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Content;
  using Microsoft.Xna.Framework.Graphics;

  internal class SharedState : IDisposable {
    private bool disposed = false;
    private BasicEffect effect;
    private Matrix projectionMatrix;
    private Random rng = new Random();
    private Texture2D texture;
    private TimeSpan timeCount;
    private Matrix viewMatrix;
    private MoveTracker moveTracker = new MoveTracker();

    internal SharedState(GraphicsDeviceManager gdm, ContentManager content) {
      GraphicsDevice device = gdm.GraphicsDevice;
      this.effect = new BasicEffect(device);

      float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
      Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(30f), aspectRatio, 0.01f, 20f, out this.projectionMatrix);
      this.viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 4), Vector3.Zero, Vector3.Up);

      this.texture = (Texture2D)content.Load<Texture>("faces");
      this.Model = new DodecaModel();
      this.ModelOrientation = DodecaModel.GetFaceOrientation(0);

      Trackball = new Trackball(this.projectionMatrix, this.viewMatrix, device.Viewport);
    }

    ~SharedState() {
      this.Dispose(false);
    }

    internal DodecaModel Model { get; set; }

    internal Quaternion ModelOrientation { get; set; }

    internal MoveTracker MoveTracker {
      get { return this.moveTracker; }
    }

    internal Trackball Trackball { get; set; }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public void Render(GraphicsDevice device) {
      this.Render(device, 0, 0);
    }

    public void Render(GraphicsDevice device, uint face, float angle) {
      this.effect.Projection = this.projectionMatrix;
      this.effect.View = this.viewMatrix;
      this.effect.World = Matrix.Identity;

      this.effect.AmbientLightColor = new Vector3(1, 1, 1);
      this.effect.LightingEnabled = true;

      this.effect.TextureEnabled = true;
      this.effect.Texture = this.texture;

      foreach (EffectPass pass in this.effect.CurrentTechnique.Passes) {
        pass.Apply();
        this.Model.Render(device, this.ModelOrientation, face, angle);
      }
    }

    public void Update(TimeSpan frameTime) {
      this.timeCount -= frameTime;
      if (this.timeCount < TimeSpan.Zero) {
        this.timeCount = TimeSpan.FromSeconds((this.rng.NextDouble() * 0.6) + 0.05);
        this.Model.RandomizeStarburstRotation(this.rng);
      }
    }

    protected virtual void Dispose(bool disposing) {
      if (this.disposed) { return; }
      if (disposing) {
        if (!object.ReferenceEquals(null, this.effect)) {
          this.effect.Dispose();
          this.effect = null;
        }
      }
      this.disposed = true;
    }
  }
}