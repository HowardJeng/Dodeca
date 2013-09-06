namespace Dodeca {
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  public class DodecaGame : Game {
    private StateHolder gameState;
    private GraphicsDeviceManager graphics;
    private KeyboardState oldKeyboardState = Keyboard.GetState();
    private MouseState oldMouseState = Mouse.GetState();

    public DodecaGame()
      : base() {
      this.graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      this.graphics.PreferredBackBufferWidth = 700;
      this.graphics.PreferredBackBufferHeight = 700;
      this.graphics.PreferMultiSampling = true;

      this.IsMouseVisible = true;
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(Color.Black);

      this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
      this.gameState.Render(this.graphics.GraphicsDevice);

      base.Draw(gameTime);
    }

    protected override void Initialize() {
      base.Initialize();
    }

    protected override void LoadContent() {
      this.gameState.SetState(new BasalState(this.graphics, Content));
    }

    protected override void UnloadContent() {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "No meaningful validation")]
    protected override void Update(GameTime gameTime) {
      if (this.gameState.Valid) { this.DispatchKeyboardEvents(); }
      if (this.gameState.Valid) { this.DispatchMouseEvents(); }
      if (!this.gameState.Update(gameTime.ElapsedGameTime)) { this.Exit(); }

      base.Update(gameTime);
    }

    private void DispatchKeyboardEvents() {
      KeyboardState newKeyBoardState = Keyboard.GetState();
      foreach (Keys key in Enum.GetValues(typeof(Keys))) {
        KeyState currentKeyState = newKeyBoardState[key];
        if (currentKeyState != this.oldKeyboardState[key]) {
          if (currentKeyState == KeyState.Up) {
            if (!this.gameState.KeyUp(key)) { this.Exit(); }
          } else {
            if (!this.gameState.KeyDown(key)) { this.Exit(); }
          }
        }
      }
      this.oldKeyboardState = newKeyBoardState;
    }

    private void DispatchMouseEvents() {
      Tuple<Func<MouseState, ButtonState>, MouseButtons>[] buttonInfo =
        new Tuple<Func<MouseState, ButtonState>, MouseButtons>[] {
          new Tuple<Func<MouseState, ButtonState>, MouseButtons>(s => s.LeftButton, MouseButtons.LeftButton),
          new Tuple<Func<MouseState, ButtonState>, MouseButtons>(s => s.MiddleButton, MouseButtons.MiddleButton),
          new Tuple<Func<MouseState, ButtonState>, MouseButtons>(s => s.RightButton, MouseButtons.RightButton)
        };
      MouseState newMouseState = Mouse.GetState();
      int mouseX = newMouseState.X;
      int mouseY = newMouseState.Y;
      foreach (var p in buttonInfo) {
        var currentButtonState = p.Item1(newMouseState);
        if (currentButtonState != p.Item1(this.oldMouseState)) {
          if (currentButtonState == ButtonState.Pressed) {
            if (!this.gameState.MouseButtonDown(p.Item2, mouseX, mouseY)) { this.Exit(); }
          } else {
            if (!this.gameState.MouseButtonUp(p.Item2, mouseX, mouseY)) { this.Exit(); }
          }
        }
      }
      if (mouseX != this.oldMouseState.X || mouseY != this.oldMouseState.Y) {
        if (!this.gameState.MouseMove(mouseX, mouseY, mouseX - this.oldMouseState.X, mouseY - this.oldMouseState.Y)) { this.Exit(); }
      }
      this.oldMouseState = newMouseState;
    }
  }
}