namespace Dodeca {
  using System;
  using System.Diagnostics;

  // Which part of a face a given corner is; also used to determine
  //   facing for the center piece. When used as a orientation,
  //   it indicates which corner the first vertex of the center
  //   piece is closest to.
  public enum CornerId { 
    North,     
    Northeast, 
    Southeast, 
    Southwest, 
    Northwest 
  }

  // Which part of a face a given edge piece is
  public enum EdgeId {
    Northeast, 
    Southeast,
    South,     
    Southwest, 
    Northwest 
  }

  public struct CornerData {
    private uint data;

    public CornerData(uint data) {
      Debug.Assert(data < 60, "data value out of range");
      this.data = data;
    }

    public CornerData(CornerId corner, uint face) {
      this.data = (face * 5) + (uint)corner;
    }

    public CornerId Corner {
      get { return (CornerId)(this.data % 5); }
    }

    public uint Face {
      get { return this.data / 5; }
    }

    public uint Data {
      get { return this.data; }
    }

    public static bool operator !=(CornerData left, CornerData right) {
      return !(left == right);
    }

    public static bool operator ==(CornerData left, CornerData right) {
      return left.data == right.data;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is CornerData) {
        CornerData other = (CornerData)obj;
        return this == other;
      }
      return false;
    }

    public override int GetHashCode() {
      return (int)this.data;
    }
  }

  public struct DodecaData {
    // Array that describes which faces are adjacent to a given
    //   face. FaceAdjacency[F, E] is the face
    //   adjacent to F on edge E.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Array is packed")]
    private static readonly uint[,] FaceAdjacency = new uint[12, 5] {
      {  2,  3,  4,  5,  1 },
      {  0,  5,  8,  7,  2 },
      {  1,  7,  6,  3,  0 },
      { 10,  4,  0,  2,  6 },
      {  5,  0,  3, 10,  9 },
      {  9,  8,  1,  0,  4 },
      {  3,  2,  7, 11, 10 },
      { 11,  6,  2,  1,  8 },
      {  7,  1,  5,  9, 11 },
      {  4, 10, 11,  8,  5 },
      {  6, 11,  9,  4,  3 },
      {  8,  9, 10,  6,  7 }
    };

    private FaceData f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return", Justification = "Array is packed")]
    public static uint[,] GetFaceAdjacency() {
      return (uint[,])FaceAdjacency.Clone();
    }

    public static bool operator !=(DodecaData left, DodecaData right) {
      return !(left == right);
    }

    public static bool operator ==(DodecaData left, DodecaData right) {
      return left.f1 == right.f1 &&
             left.f2 == right.f2 &&
             left.f3 == right.f3 &&
             left.f4 == right.f4 &&
             left.f5 == right.f5 &&
             left.f6 == right.f6 &&
             left.f7 == right.f7 &&
             left.f8 == right.f8 &&
             left.f9 == right.f9 &&
             left.f10 == right.f10 &&
             left.f11 == right.f11 &&
             left.f12 == right.f12;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is DodecaData) {
        DodecaData other = (DodecaData)obj;
        return this == other;
      }
      return false;
    }

    public CornerId GetCenter(uint face) {
      switch (face) {
        case 0:
          return this.f1.Center;
        case 1:
          return this.f2.Center;
        case 2:
          return this.f3.Center;
        case 3:
          return this.f4.Center;
        case 4:
          return this.f5.Center;
        case 5:
          return this.f6.Center;
        case 6:
          return this.f7.Center;
        case 7:
          return this.f8.Center;
        case 8:
          return this.f9.Center;
        case 9:
          return this.f10.Center;
        case 10:
          return this.f11.Center;
        case 11:
          return this.f12.Center;
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "Missing face");
      }
    }

    public CornerData GetCorner(uint face, CornerId corner) {
      switch (face) {
        case 0:
          return this.f1.GetCorner(corner);
        case 1:
          return this.f2.GetCorner(corner);
        case 2:
          return this.f3.GetCorner(corner);
        case 3:
          return this.f4.GetCorner(corner);
        case 4:
          return this.f5.GetCorner(corner);
        case 5:
          return this.f6.GetCorner(corner);
        case 6:
          return this.f7.GetCorner(corner);
        case 7:
          return this.f8.GetCorner(corner);
        case 8:
          return this.f9.GetCorner(corner);
        case 9:
          return this.f10.GetCorner(corner);
        case 10:
          return this.f11.GetCorner(corner);
        case 11:
          return this.f12.GetCorner(corner);
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "Missing face");
      }
    }

    public EdgeData GetEdge(uint face, EdgeId edge) {
      switch (face) {
        case 0:
          return this.f1.GetEdge(edge);
        case 1:
          return this.f2.GetEdge(edge);
        case 2:
          return this.f3.GetEdge(edge);
        case 3:
          return this.f4.GetEdge(edge);
        case 4:
          return this.f5.GetEdge(edge);
        case 5:
          return this.f6.GetEdge(edge);
        case 6:
          return this.f7.GetEdge(edge);
        case 7:
          return this.f8.GetEdge(edge);
        case 8:
          return this.f9.GetEdge(edge);
        case 9:
          return this.f10.GetEdge(edge);
        case 10:
          return this.f11.GetEdge(edge);
        case 11:
          return this.f12.GetEdge(edge);
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "Missing face");
      }
    }

    public override int GetHashCode() {
      int r = this.f1.GetHashCode();
      r = (r * 397) ^ this.f2.GetHashCode();
      r = (r * 397) ^ this.f3.GetHashCode();
      r = (r * 397) ^ this.f4.GetHashCode();
      r = (r * 397) ^ this.f5.GetHashCode();
      r = (r * 397) ^ this.f6.GetHashCode();
      r = (r * 397) ^ this.f7.GetHashCode();
      r = (r * 397) ^ this.f8.GetHashCode();
      r = (r * 397) ^ this.f9.GetHashCode();
      r = (r * 397) ^ this.f10.GetHashCode();
      r = (r * 397) ^ this.f11.GetHashCode();
      r = (r * 397) ^ this.f12.GetHashCode();
      return r;
    }

    public void Init() {
      this.f1 = new FaceData(0);
      this.f2 = new FaceData(1);
      this.f3 = new FaceData(2);
      this.f4 = new FaceData(3);
      this.f5 = new FaceData(4);
      this.f6 = new FaceData(5);
      this.f7 = new FaceData(6);
      this.f8 = new FaceData(7);
      this.f9 = new FaceData(8);
      this.f10 = new FaceData(9);
      this.f11 = new FaceData(10);
      this.f12 = new FaceData(11);
    }

    public void RotateLeft(uint face) {
      this.RotateCornersLeft(face, CornerId.North,
                             face, CornerId.Northeast,
                             face, CornerId.Southeast,
                             face, CornerId.Southwest,
                             face, CornerId.Northwest);

      this.RotateEdgesLeft(face, EdgeId.Northeast,
                           face, EdgeId.Southeast,
                           face, EdgeId.South,
                           face, EdgeId.Southwest,
                           face, EdgeId.Northwest);

      CornerId cd = this.GetCenter(face);
      this.SetCenter(face, (CornerId)((uint)(cd + 4) % 5));

      this.RotateCornersLeft(FaceAdjacency[face, (int)EdgeId.Northeast], CornerId.North,
                             FaceAdjacency[face, (int)EdgeId.Southeast], CornerId.Southwest,
                             FaceAdjacency[face, (int)EdgeId.South], CornerId.Southeast,
                             FaceAdjacency[face, (int)EdgeId.Southwest], CornerId.Northwest,
                             FaceAdjacency[face, (int)EdgeId.Northwest], CornerId.Northeast);

      this.RotateEdgesLeft(FaceAdjacency[face, (int)EdgeId.Northeast], EdgeId.Northwest,
                           FaceAdjacency[face, (int)EdgeId.Southeast], EdgeId.South,
                           FaceAdjacency[face, (int)EdgeId.South], EdgeId.Southeast,
                           FaceAdjacency[face, (int)EdgeId.Southwest], EdgeId.Southwest,
                           FaceAdjacency[face, (int)EdgeId.Northwest], EdgeId.Northeast);

      this.RotateCornersLeft(FaceAdjacency[face, (int)EdgeId.Northeast], CornerId.Northwest,
                             FaceAdjacency[face, (int)EdgeId.Southeast], CornerId.Southeast,
                             FaceAdjacency[face, (int)EdgeId.South], CornerId.Northeast,
                             FaceAdjacency[face, (int)EdgeId.Southwest], CornerId.Southwest,
                             FaceAdjacency[face, (int)EdgeId.Northwest], CornerId.North);
    }

    public void RotateRight(uint face) {
      this.RotateCornersRight(face, CornerId.North,
                              face, CornerId.Northeast,
                              face, CornerId.Southeast,
                              face, CornerId.Southwest,
                              face, CornerId.Northwest);

      this.RotateEdgesRight(face, EdgeId.Northeast,
                            face, EdgeId.Southeast,
                            face, EdgeId.South,
                            face, EdgeId.Southwest,
                            face, EdgeId.Northwest);

      CornerId cd = this.GetCenter(face);
      this.SetCenter(face, (CornerId)((uint)(cd + 1) % 5));

      this.RotateCornersRight(FaceAdjacency[face, (int)EdgeId.Northeast], CornerId.North,
                              FaceAdjacency[face, (int)EdgeId.Southeast], CornerId.Southwest,
                              FaceAdjacency[face, (int)EdgeId.South], CornerId.Southeast,
                              FaceAdjacency[face, (int)EdgeId.Southwest], CornerId.Northwest,
                              FaceAdjacency[face, (int)EdgeId.Northwest], CornerId.Northeast);

      this.RotateEdgesRight(FaceAdjacency[face, (int)EdgeId.Northeast], EdgeId.Northwest,
                            FaceAdjacency[face, (int)EdgeId.Southeast], EdgeId.South,
                            FaceAdjacency[face, (int)EdgeId.South], EdgeId.Southeast,
                            FaceAdjacency[face, (int)EdgeId.Southwest], EdgeId.Southwest,
                            FaceAdjacency[face, (int)EdgeId.Northwest], EdgeId.Northeast);

      this.RotateCornersRight(FaceAdjacency[face, (int)EdgeId.Northeast], CornerId.Northwest,
                              FaceAdjacency[face, (int)EdgeId.Southeast], CornerId.Southeast,
                              FaceAdjacency[face, (int)EdgeId.South], CornerId.Northeast,
                              FaceAdjacency[face, (int)EdgeId.Southwest], CornerId.Southwest,
                              FaceAdjacency[face, (int)EdgeId.Northwest], CornerId.North);
    }

    public void SetCenter(uint face, CornerId cd) {
      switch (face) {
        case 0:
          this.f1.Center = cd;
          break;
        case 1:
          this.f2.Center = cd;
          break;
        case 2:
          this.f3.Center = cd;
          break;
        case 3:
          this.f4.Center = cd;
          break;
        case 4:
          this.f5.Center = cd;
          break;
        case 5:
          this.f6.Center = cd;
          break;
        case 6:
          this.f7.Center = cd;
          break;
        case 7:
          this.f8.Center = cd;
          break;
        case 8:
          this.f9.Center = cd;
          break;
        case 9:
          this.f10.Center = cd;
          break;
        case 10:
          this.f11.Center = cd;
          break;
        case 11:
          this.f12.Center = cd;
          break;
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "Missing face");
      }
    }

    public void SetCorner(uint face, CornerId corner, CornerData cd) {
      switch (face) {
        case 0:
          this.f1.SetCorner(corner, cd);
          break;
        case 1:
          this.f2.SetCorner(corner, cd);
          break;
        case 2:
          this.f3.SetCorner(corner, cd);
          break;
        case 3:
          this.f4.SetCorner(corner, cd);
          break;
        case 4:
          this.f5.SetCorner(corner, cd);
          break;
        case 5:
          this.f6.SetCorner(corner, cd);
          break;
        case 6:
          this.f7.SetCorner(corner, cd);
          break;
        case 7:
          this.f8.SetCorner(corner, cd);
          break;
        case 8:
          this.f9.SetCorner(corner, cd);
          break;
        case 9:
          this.f10.SetCorner(corner, cd);
          break;
        case 10:
          this.f11.SetCorner(corner, cd);
          break;
        case 11:
          this.f12.SetCorner(corner, cd);
          break;
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "illegal face value");
      }
    }

    public void SetEdge(uint face, EdgeId edge, EdgeData ed) {
      switch (face) {
        case 0:
          this.f1.SetEdge(edge, ed);
          break;
        case 1:
          this.f2.SetEdge(edge, ed);
          break;
        case 2:
          this.f3.SetEdge(edge, ed);
          break;
        case 3:
          this.f4.SetEdge(edge, ed);
          break;
        case 4:
          this.f5.SetEdge(edge, ed);
          break;
        case 5:
          this.f6.SetEdge(edge, ed);
          break;
        case 6:
          this.f7.SetEdge(edge, ed);
          break;
        case 7:
          this.f8.SetEdge(edge, ed);
          break;
        case 8:
          this.f9.SetEdge(edge, ed);
          break;
        case 9:
          this.f10.SetEdge(edge, ed);
          break;
        case 10:
          this.f11.SetEdge(edge, ed);
          break;
        case 11:
          this.f12.SetEdge(edge, ed);
          break;
        default:
          Debug.Assert(false, "illegal face value");
          throw new ArgumentOutOfRangeException("face", "illegal face value");
      }
    }

    internal static EdgeId CCWEdgeFromCorner(CornerId corner) {
      Debug.Assert((uint)corner < 5, "invalid corner value");
      return (EdgeId)((uint)(corner + 4) % 5);
    }

    internal static EdgeId CWEdgeFromCorner(CornerId corner) {
      Debug.Assert((uint)corner < 5, "invalid corner value");
      return (EdgeId)corner;
    }

    internal static CornerId OppositeCornerFromEdge(EdgeId edge) {
      Debug.Assert((uint)edge < 5, "invalid edge value");
      return (CornerId)((uint)(edge + 3) % 5);
    }

    private void RotateCornersLeft(uint face1, CornerId c1, 
                                   uint face2, CornerId c2, 
                                   uint face3, CornerId c3, 
                                   uint face4, CornerId c4, 
                                   uint face5, CornerId c5) {
      CornerData cd1 = this.GetCorner(face1, c1);
      CornerData cd2 = this.GetCorner(face2, c2);
      CornerData cd3 = this.GetCorner(face3, c3);
      CornerData cd4 = this.GetCorner(face4, c4);
      CornerData cd5 = this.GetCorner(face5, c5);

      this.SetCorner(face5, c5, cd4);
      this.SetCorner(face4, c4, cd3);
      this.SetCorner(face3, c3, cd2);
      this.SetCorner(face2, c2, cd1);
      this.SetCorner(face1, c1, cd5);
    }

    private void RotateCornersRight(uint face1, CornerId c1,
                                    uint face2, CornerId c2,
                                    uint face3, CornerId c3,
                                    uint face4, CornerId c4,
                                    uint face5, CornerId c5) {
      CornerData cd1 = this.GetCorner(face1, c1);
      CornerData cd2 = this.GetCorner(face2, c2);
      CornerData cd3 = this.GetCorner(face3, c3);
      CornerData cd4 = this.GetCorner(face4, c4);
      CornerData cd5 = this.GetCorner(face5, c5);

      this.SetCorner(face5, c5, cd1);
      this.SetCorner(face4, c4, cd5);
      this.SetCorner(face3, c3, cd4);
      this.SetCorner(face2, c2, cd3);
      this.SetCorner(face1, c1, cd2);
    }

    private void RotateEdgesLeft(uint face1, EdgeId e1,
                                 uint face2, EdgeId e2,
                                 uint face3, EdgeId e3,
                                 uint face4, EdgeId e4,
                                 uint face5, EdgeId e5) {
      EdgeData ed1 = this.GetEdge(face1, e1);
      EdgeData ed2 = this.GetEdge(face2, e2);
      EdgeData ed3 = this.GetEdge(face3, e3);
      EdgeData ed4 = this.GetEdge(face4, e4);
      EdgeData ed5 = this.GetEdge(face5, e5);

      this.SetEdge(face5, e5, ed4);
      this.SetEdge(face4, e4, ed3);
      this.SetEdge(face3, e3, ed2);
      this.SetEdge(face2, e2, ed1);
      this.SetEdge(face1, e1, ed5);
    }

    private void RotateEdgesRight(uint face1, EdgeId e1,
                                  uint face2, EdgeId e2,
                                  uint face3, EdgeId e3,
                                  uint face4, EdgeId e4,
                                  uint face5, EdgeId e5) {
      EdgeData ed1 = this.GetEdge(face1, e1);
      EdgeData ed2 = this.GetEdge(face2, e2);
      EdgeData ed3 = this.GetEdge(face3, e3);
      EdgeData ed4 = this.GetEdge(face4, e4);
      EdgeData ed5 = this.GetEdge(face5, e5);

      this.SetEdge(face5, e5, ed1);
      this.SetEdge(face4, e4, ed5);
      this.SetEdge(face3, e3, ed4);
      this.SetEdge(face2, e2, ed3);
      this.SetEdge(face1, e1, ed2);
    }
  }

  public struct EdgeData {
    private uint data;

    public EdgeData(uint data) {
      Debug.Assert(data < 60, "data out of range");
      this.data = data;
    }

    public EdgeData(EdgeId edge, uint face) {
      this.data = (face * 5) + (uint)edge;
    }

    public uint Data {
      get { return this.data; }
    }

    public EdgeId Edge {
      get { return (EdgeId)(this.data % 5); }
    }

    public uint Face {
      get { return this.data / 5; }
    }

    public static bool operator !=(EdgeData left, EdgeData right) {
      return !(left == right);
    }

    public static bool operator ==(EdgeData left, EdgeData right) {
      return left.data == right.data;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is EdgeData) {
        EdgeData other = (EdgeData)obj;
        return this == other;
      }
      return false;
    }

    public override int GetHashCode() {
      return (int)this.data;
    }
  }

  public struct FaceData {
    private ulong data;

    public FaceData(uint face) {
      this.data = 0;
      this.Center = CornerId.North;
      foreach (CornerId c in Enum.GetValues(typeof(CornerId))) {
        this.SetCorner(c, new CornerData(c, face));
      }
      foreach (EdgeId e in Enum.GetValues(typeof(EdgeId))) {
        this.SetEdge(e, new EdgeData(e, face));
      }
    }

    public CornerId Center {
      get { 
        return (CornerId)(this.data >> 60); 
      }

      set {
        this.data &= ~(0x7UL << 60);
        this.data |= (ulong)value << 60;
      }
    }

    public static bool operator !=(FaceData left, FaceData right) {
      return !(left == right);
    }

    public static bool operator ==(FaceData left, FaceData right) {
      return left.data == right.data;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is FaceData) {
        FaceData other = (FaceData)obj;
        return this == other;
      }
      return false;
    }

    public CornerData GetCorner(CornerId corner) {
      int shift = (int)corner * 6;
      return new CornerData((uint)((this.data >> shift) & 0x3f));
    }

    public EdgeData GetEdge(EdgeId edge) {
      int shift = ((int)edge * 6) + 30;
      return new EdgeData((uint)((this.data >> shift) & 0x3f));
    }

    public override int GetHashCode() {
      return (int)((this.data >> 32) * 397 ^ (this.data & 0xffffffff));
    }

    public void SetCorner(CornerId corner, CornerData cd) {
      int shift = (int)corner * 6;
      this.data &= ~(0x3FUL << shift);
      this.data |= (ulong)cd.Data << shift;
    }

    public void SetEdge(EdgeId edge, EdgeData ed) {
      int shift = ((int)edge * 6) + 30;
      this.data &= ~(0x3FUL << shift);
      this.data |= (ulong)ed.Data << shift;
    }
  }
}