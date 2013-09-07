namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Content;
  using Microsoft.Xna.Framework.Graphics;

  internal class SharedState : IDisposable {
    private bool disposed = false;
    private BasicEffect effect;
    private DodecaModel model;
    private Quaternion modelOrientation;
    private MoveTracker moveTracker = new MoveTracker();
    private Matrix projectionMatrix;
    private Random rng = new Random();
    private Texture2D texture;
    private TimeSpan timeCount;
    private Trackball trackball;
    private Matrix viewMatrix;
    private Viewport viewport;

    internal SharedState(GraphicsDeviceManager gdm, ContentManager content) {
      GraphicsDevice device = gdm.GraphicsDevice;
      this.effect = new BasicEffect(device);

      this.viewport = device.Viewport;
      float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
      Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(30f), aspectRatio, 0.01f, 20f, out this.projectionMatrix);
      this.viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 4), Vector3.Zero, Vector3.Up);

      this.texture = (Texture2D)content.Load<Texture>("faces");
      this.model = new DodecaModel();
      this.ModelOrientation = DodecaModel.GetFaceOrientation(0);

      this.trackball = new Trackball(this.projectionMatrix, this.viewMatrix, device.Viewport);
    }

    ~SharedState() {
      this.Dispose(false);
    }

    internal DodecaModel Model { 
      get { return this.model; }
    }

    internal Quaternion ModelOrientation { 
      get { return this.modelOrientation; } 
      set { this.modelOrientation = value; }
    }

    internal MoveTracker MoveTracker {
      get { return this.moveTracker; }
    }

    internal Trackball Trackball { 
      get { return this.trackball; }
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public Vector2 FaceCenterScreenCoordinates(uint face) {
      Matrix model = Matrix.CreateFromQuaternion(this.modelOrientation);
      Vector3 v = this.viewport.Project(DodecaModel.FaceCenter(face), this.projectionMatrix, this.viewMatrix, model);
      return new Vector2(v.X, v.Y);
    }

    public uint PickFace(int screenX, int screenY) {
      Vector3 near = this.viewport.Unproject(new Vector3(screenX, screenY, 0), this.projectionMatrix, this.viewMatrix, Matrix.Identity);
      Vector3 far = this.viewport.Unproject(new Vector3(screenX, screenY, 1), this.projectionMatrix, this.viewMatrix, Matrix.Identity);

      Vector3 direction = far - near;
      Ray ray = new Ray(near, direction);

      Matrix modelMatrix = Matrix.CreateFromQuaternion(this.modelOrientation);
      float bestDistance = float.MaxValue;
      uint bestFace = uint.MaxValue;
      for (uint i = 0; i < 12; i++) {
        Vector3[] faceVertices = DodecaModel.GetFacePickingVertices(i);
        Vector3[] transformedVertices = new Vector3[faceVertices.Length];
        Vector3.Transform(faceVertices, ref modelMatrix, transformedVertices);

        for (uint j = 1; j <= 3; j++) {
          float? intersection = Utility.RayTriangleIntersection(ray, transformedVertices[0], transformedVertices[j], transformedVertices[j + 1]);
          if (intersection != null) {
            if ((float)intersection < bestDistance) {
              bestDistance = (float)intersection;
              bestFace = i;
            }
          }
        }
      }
      return bestFace;
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