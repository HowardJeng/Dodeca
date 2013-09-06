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

  public struct CenterData {
    private CornerId facing;

    public CenterData(CornerId facing) {
      this.facing = facing;
    }

    public CornerId Facing {
      get { return this.facing; }
    }

    public static bool operator !=(CenterData left, CenterData right) {
      return !(left == right);
    }

    public static bool operator ==(CenterData left, CenterData right) {
      return left.facing == right.facing;
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) { return false; }
      if (ReferenceEquals(this, obj)) { return true; }
      if (obj is CenterData) {
        CenterData other = (CenterData)obj;
        return this == other;
      }
      return false;
    }

    public override int GetHashCode() {
      return (int)this.facing;
    }
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

    public CenterData GetCenter(uint face) {
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

      CenterData cd = this.GetCenter(face);
      this.SetCenter(face, new CenterData((CornerId)((uint)(cd.Facing + 4) % 5)));

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

      CenterData cd = this.GetCenter(face);
      this.SetCenter(face, new CenterData((CornerId)((uint)(cd.Facing + 1) % 5)));

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

    public void SetCenter(uint face, CenterData cd) {
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
    private CornerData c1, c2, c3, c4, c5;
    private CenterData center;
    private EdgeData   e1, e2, e3, e4, e5;

    public FaceData(uint face) {
      this.center = new CenterData(CornerId.North);
      this.c1 = new CornerData(CornerId.North, face);
      this.c2 = new CornerData(CornerId.Northeast, face);
      this.c3 = new CornerData(CornerId.Southeast, face);
      this.c4 = new CornerData(CornerId.Southwest, face);
      this.c5 = new CornerData(CornerId.Northwest, face);
      this.e1 = new EdgeData(EdgeId.Northeast, face);
      this.e2 = new EdgeData(EdgeId.Southeast, face);
      this.e3 = new EdgeData(EdgeId.South, face);
      this.e4 = new EdgeData(EdgeId.Southwest, face);
      this.e5 = new EdgeData(EdgeId.Northwest, face);
    }

    public CenterData Center {
      get { return this.center; }
      set { this.center = value; }
    }

    public static bool operator !=(FaceData left, FaceData right) {
      return !(left == right);
    }

    public static bool operator ==(FaceData left, FaceData right) {
      return left.center == right.center &&
             left.c1 == right.c1 &&
             left.c2 == right.c2 &&
             left.c3 == right.c3 &&
             left.c4 == right.c4 &&
             left.c5 == right.c5 &&
             left.e1 == right.e1 &&
             left.e2 == right.e2 &&
             left.e3 == right.e3 &&
             left.e4 == right.e4 &&
             left.e5 == right.e5;
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
      switch (corner) {
        case CornerId.North:
          return this.c1;
        case CornerId.Northeast:
          return this.c2;
        case CornerId.Southeast:
          return this.c3;
        case CornerId.Southwest:
          return this.c4;
        case CornerId.Northwest:
          return this.c5;
        default:
          Debug.Assert(false, "illegal corner value");
          throw new ArgumentOutOfRangeException("corner", "illegal corner value");
      }
    }

    public EdgeData GetEdge(EdgeId edge) {
      switch (edge) {
        case EdgeId.Northeast:
          return this.e1;
        case EdgeId.Southeast:
          return this.e2;
        case EdgeId.South:
          return this.e3;
        case EdgeId.Southwest:
          return this.e4;
        case EdgeId.Northwest:
          return this.e5;
        default:
          Debug.Assert(false, "illegal edge value");
          throw new ArgumentOutOfRangeException("edge", "illegal edge value");
      }
    }

    public override int GetHashCode() {
      int r = this.center.GetHashCode();
      r = (r * 397) ^ this.c1.GetHashCode();
      r = (r * 397) ^ this.c2.GetHashCode();
      r = (r * 397) ^ this.c3.GetHashCode();
      r = (r * 397) ^ this.c4.GetHashCode();
      r = (r * 397) ^ this.c5.GetHashCode();
      r = (r * 397) ^ this.e1.GetHashCode();
      r = (r * 397) ^ this.e2.GetHashCode();
      r = (r * 397) ^ this.e3.GetHashCode();
      r = (r * 397) ^ this.e4.GetHashCode();
      r = (r * 397) ^ this.e5.GetHashCode();
      return r;
    }

    public void SetCorner(CornerId corner, CornerData cd) {
      switch (corner) {
        case CornerId.North:
          this.c1 = cd;
          break;
        case CornerId.Northeast:
          this.c2 = cd;
          break;
        case CornerId.Southeast:
          this.c3 = cd;
          break;
        case CornerId.Southwest:
          this.c4 = cd;
          break;
        case CornerId.Northwest:
          this.c5 = cd;
          break;
        default:
          Debug.Assert(false, "illegal corner value");
          throw new ArgumentOutOfRangeException("corner", "illegal corner value");
      }
    }

    public void SetEdge(EdgeId edge, EdgeData ed) {
      switch (edge) {
        case EdgeId.Northeast:
          this.e1 = ed;
          break;
        case EdgeId.Southeast:
          this.e2 = ed;
          break;
        case EdgeId.South:
          this.e3 = ed;
          break;
        case EdgeId.Southwest:
          this.e4 = ed;
          break;
        case EdgeId.Northwest:
          this.e5 = ed;
          break;
        default:
          Debug.Assert(false, "illegal edge value");
          throw new ArgumentOutOfRangeException("edge", "illegal edge value");
      }
    }
  }
}