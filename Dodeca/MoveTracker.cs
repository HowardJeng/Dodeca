namespace Dodeca {
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;

  internal struct Move {
    internal Move(uint face, Rotation direction)
      : this() {
      this.Face = face;
      this.Direction = direction;
    }

    internal Rotation Direction { get; set; }

    internal uint Face { get; set; }

    public static Move operator -(Move move) {
      return new Move(move.Face, (Rotation)(1 - move.Direction));
    }

    public static bool operator !=(Move left, Move right) {
      return left.Face != right.Face || left.Direction != right.Direction;
    }

    public static bool operator ==(Move left, Move right) {
      return left.Face == right.Face && left.Direction == right.Direction;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is CenterData) {
        Move other = (Move)obj;
        return this == other;
      }
      return false;
    }

    public override int GetHashCode() {
      return (int)this.Face * 397 ^ (int)this.Direction;
    }
  }

  internal class MoveTracker {
    private List<Move> redoMoves = new List<Move>();

    private List<Move> undoMoves = new List<Move>();

    internal void AddMove(Move move) {
      var redoCount = this.redoMoves.Count;
      var undoCount = this.undoMoves.Count;
      if (this.undoMoves.Count == 0) {
        this.undoMoves.Add(-move);
        if (redoCount != 0) {
          if (this.redoMoves[redoCount - 1] == move) {
            this.redoMoves.RemoveAt(redoCount - 1);
          } else {
            this.redoMoves.Clear();
          }
        }
      } else {
        if (this.redoMoves.Count == 0) {
          if (this.undoMoves[undoCount - 1] == move) {
            this.undoMoves.RemoveAt(undoCount - 1);
            this.redoMoves.Add(-move);
          } else {
            this.undoMoves.Add(-move);
          }
        } else {
          if (this.undoMoves[undoCount - 1] == move) {
            Debug.Assert(this.redoMoves[redoCount - 1] != move, "Last undo and redo moves should be different");
            this.undoMoves.RemoveAt(undoCount - 1);
            this.redoMoves.Add(-move);
          } else {
            this.undoMoves.Add(-move);
            if (this.redoMoves[redoCount - 1] == move) {
              this.redoMoves.RemoveAt(redoCount - 1);
            } else {
              this.redoMoves.Clear();
            }
          }
        }
      }
    }

    internal void AddMove(uint f, Rotation d) {
      this.AddMove(new Move(f, d));
    }

    internal void Clear() {
      this.undoMoves.Clear();
      this.redoMoves.Clear();
    }

    internal Move GetRedoMove() {
      Debug.Assert(this.HasRedoMoves(), "Illegal to get an redo move if there are not redo moves to get");
      return this.redoMoves.Last();
    }

    internal Move GetUndoMove() {
      Debug.Assert(this.HasUndoMoves(), "Illegal to get an undo move if there are not undo moves to get");
      return this.undoMoves.Last();
    }

    internal bool HasRedoMoves() {
      return this.redoMoves.Count != 0;
    }

    internal bool HasUndoMoves() {
      return this.undoMoves.Count != 0;
    }
  }
}