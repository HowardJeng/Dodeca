namespace Dodeca {
  using System;
  using System.Collections.Generic;
  using System.Linq;

#if WINDOWS || LINUX
  public static class Program {
    [STAThread]
    public static void Main() {
      using (var game = new DodecaGame()) {
        game.Run();
      }
    }
  }
#endif
}
