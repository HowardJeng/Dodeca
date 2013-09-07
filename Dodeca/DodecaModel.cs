namespace Dodeca {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  internal class CenterFacet : Facet {
    public override bool IsAdjacent(uint face) {
      return face == this.Face;
    }

    internal CenterFacet Init(CornerId cd, uint face, Quaternion q, uint rotationFace, Quaternion faceRotation) {
      base.Init(GetVertices(cd, face), face, q, rotationFace, faceRotation);
      return this;
    }

    private static VertexPositionNormalTexture[] GetVertices(CornerId cd, uint face) {
      VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[5];
      for (int i = 0; i < 5; i++) {
        vertices[i].Position = ((DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (i + 4) % 5]] +
                                 DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (i + 1) % 5]]) / 2) +
                                DodecaModel.FaceOffset(face);
        vertices[i].Normal = DodecaModel.Normal(face);
        vertices[i].TextureCoordinate = DodecaModel.ConvertTextureCoord(
                                          (DodecaModel.TexCoord((uint)(i + 4 + cd) % 5) +
                                           DodecaModel.TexCoord((uint)(i + 1 + cd) % 5)) / 2,
                                           face);
      }
      return vertices;
    }
  }

  internal class CornerFacet : Facet {
    private CornerId corner;

    public override bool IsAdjacent(uint face) {
      if (face == this.Face) { return true; }
      EdgeId cw = DodecaData.CWEdgeFromCorner(this.corner);
      EdgeId ccw = DodecaData.CCWEdgeFromCorner(this.corner);
      if (DodecaData.GetFaceAdjacency()[this.Face, (int)cw] == face) { return true; }
      if (DodecaData.GetFaceAdjacency()[this.Face, (int)ccw] == face) { return true; }
      return false;
    }

    internal CornerFacet Init(CornerData cd, uint face, CornerId corner, Quaternion q, uint rotationFace, Quaternion faceRotation) {
      this.corner = corner;
      base.Init(GetVertices(cd, face, corner), face, q, rotationFace, faceRotation);
      return this;
    }

    private static VertexPositionNormalTexture[] GetVertices(CornerData cd, uint face, CornerId corner) {
      VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
      {
        Vector3 a = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)corner]];
        Vector3 b = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(corner + 1) % 5]];
        Vector3 c = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(corner + 4) % 5]];
        vertices[0].Position = a;
        vertices[1].Position = (a + b) / 2;
        vertices[2].Position = (b + c) / 2;
        vertices[3].Position = (a + c) / 2;

        Vector3 offset1 = (vertices[0].Position - vertices[1].Position) * DodecaModel.PieceSeperation;
        Vector3 offset2 = (vertices[0].Position - vertices[3].Position) * DodecaModel.PieceSeperation;

        vertices[0].Position += DodecaModel.FaceOffset(face);
        vertices[1].Position += offset1 + DodecaModel.FaceOffset(face);
        vertices[2].Position += offset1 + offset2 + DodecaModel.FaceOffset(face);
        vertices[3].Position += offset2 + DodecaModel.FaceOffset(face);
      }
      {
        Vector2 a = DodecaModel.TexCoord((uint)cd.Corner);
        Vector2 b = DodecaModel.TexCoord((uint)(cd.Corner + 1) % 5);
        Vector2 c = DodecaModel.TexCoord((uint)(cd.Corner + 4) % 5);
        vertices[0].TextureCoordinate = DodecaModel.ConvertTextureCoord(a, cd.Face);
        vertices[1].TextureCoordinate = DodecaModel.ConvertTextureCoord((a + b) / 2, cd.Face);
        vertices[2].TextureCoordinate = DodecaModel.ConvertTextureCoord((b + c) / 2, cd.Face);
        vertices[3].TextureCoordinate = DodecaModel.ConvertTextureCoord((a + c) / 2, cd.Face);
      }
      vertices[0].Normal = vertices[1].Normal = vertices[2].Normal = vertices[3].Normal = DodecaModel.Normal(face);
      return vertices;
    }
  }

  internal class DodecaModel {
    // important angles
    // 36 degrees
    public const double Deg36 = Math.PI * 36.0 / 180.0;

    // 72 degrees
    public const double Deg72 = Math.PI * 72.0 / 180.0;

    public const float FaceSeperation = 0.01f;

    public const float PieceSeperation = 0.02f;

    public const double Radius = 1.0;

    // angle from midpoint of edge to center of face via origin
    public static readonly double Gamma = .5 * Math.Asin((2.0 * Math.Cos(Deg36)) / (1.0 + Math.Cos(Deg36)));

    // angle from vertex to center of face via origin
    public static readonly double Alpha = Math.Atan(Math.Tan(Gamma) / Math.Cos(Deg36));

    // angle from vertex to midpoint of edge via origin
    public static readonly double Beta = Math.Atan(Math.Tan(Deg36) * Math.Sin(Gamma));

    public static readonly Vector3[] Vertices = new Vector3[20] {
      SphericalToVector3(Radius, Alpha, 0.0 * Deg72),
      SphericalToVector3(Radius, Alpha, 1.0 * Deg72),
      SphericalToVector3(Radius, Alpha, 2.0 * Deg72),
      SphericalToVector3(Radius, Alpha, 3.0 * Deg72),
      SphericalToVector3(Radius, Alpha, 4.0 * Deg72),

      SphericalToVector3(Radius, Alpha + (2.0 * Beta), 0.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta), 1.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta), 2.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta), 3.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta), 4.0 * Deg72),

      SphericalToVector3(Radius, Alpha + (2.0 * Beta) + Math.PI, 4.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta) + Math.PI, 3.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta) + Math.PI, 2.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta) + Math.PI, 1.0 * Deg72),
      SphericalToVector3(Radius, Alpha + (2.0 * Beta) + Math.PI, 0.0 * Deg72),

      SphericalToVector3(Radius, Alpha + Math.PI, 4.0 * Deg72),
      SphericalToVector3(Radius, Alpha + Math.PI, 3.0 * Deg72),
      SphericalToVector3(Radius, Alpha + Math.PI, 2.0 * Deg72),
      SphericalToVector3(Radius, Alpha + Math.PI, 1.0 * Deg72),
      SphericalToVector3(Radius, Alpha + Math.PI, 0.0 * Deg72),
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Array is tightly packed")]
    private static readonly int[,] FaceVertices = new int[12, 5] {
      {  0,  1,  2,  3,  4 },
      {  0,  4,  9, 12,  5 },
      {  0,  5, 11,  6,  1 },
      { 10,  7,  2,  1,  6 },
      {  8,  3,  2,  7, 14 },
      {  8, 13,  9,  4,  3 },
      { 10,  6, 11, 16, 15 },
      { 17, 16, 11,  5, 12 },
      { 17, 12,  9, 13, 18 },
      {  8, 14, 19, 18, 13 },
      { 10, 15, 19, 14,  7 },
      { 17, 18, 19, 15, 16 }
    };

    private static readonly Vector3[] Normals = new Vector3[12] {
      SphericalToVector3(1.0, 0.0, 0.0),

      SphericalToVector3(1.0, 2.0 * Gamma, -1.0 * Deg36),
      SphericalToVector3(1.0, 2.0 * Gamma,  1.0 * Deg36),
      SphericalToVector3(1.0, 2.0 * Gamma,  3.0 * Deg36),
      SphericalToVector3(1.0, 2.0 * Gamma,  5.0 * Deg36),
      SphericalToVector3(1.0, 2.0 * Gamma,  7.0 * Deg36),

      SphericalToVector3(1.0, (2.0 * Gamma) + Math.PI,  7.0 * Deg36),
      SphericalToVector3(1.0, (2.0 * Gamma) + Math.PI,  5.0 * Deg36),
      SphericalToVector3(1.0, (2.0 * Gamma) + Math.PI,  3.0 * Deg36),
      SphericalToVector3(1.0, (2.0 * Gamma) + Math.PI,  1.0 * Deg36),
      SphericalToVector3(1.0, (2.0 * Gamma) + Math.PI, -1.0 * Deg36),

      SphericalToVector3(1.0, Math.PI, 0.0)
    };

    private static readonly Vector2[] TexCoords = new Vector2[5] {
      new Vector2((float)(.5 - (.5 * Math.Sin(0.0 * Deg72))), (float)(.5 - (.5 * Math.Cos(0.0 * Deg72)))),
      new Vector2((float)(.5 - (.5 * Math.Sin(1.0 * Deg72))), (float)(.5 - (.5 * Math.Cos(1.0 * Deg72)))),
      new Vector2((float)(.5 - (.5 * Math.Sin(2.0 * Deg72))), (float)(.5 - (.5 * Math.Cos(2.0 * Deg72)))),
      new Vector2((float)(.5 - (.5 * Math.Sin(3.0 * Deg72))), (float)(.5 - (.5 * Math.Cos(3.0 * Deg72)))),
      new Vector2((float)(.5 - (.5 * Math.Sin(4.0 * Deg72))), (float)(.5 - (.5 * Math.Cos(4.0 * Deg72))))
    };

    private DodecaData data;

    private Matrix starburstRotation;

    private float textureXOffset;

    public DodecaModel() {
      this.data.Init();
      this.starburstRotation = Matrix.Identity;
      this.textureXOffset = 0;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "Array is tightly packed")]
    internal static int[,] GetFaceVertices {
      get { return (int[,])FaceVertices.Clone(); }
    }

    public static Vector2 ConvertTextureCoord(Vector2 coord, uint face) {
      return (coord / 4) + new Vector2(0.25f * (face % 4), 0.25f * (face / 4));
    }

    public static Vector3 FaceOffset(uint face) {
      Debug.Assert(face < 12, "illegal face value");
      return Normals[face] * FaceSeperation;
    }

    public static Vector3 [] GetFacePickingVertices(uint face) {
      Vector3 offset = FaceOffset(face);
      return new Vector3[5] {
        Vertices[FaceVertices[face, 0]] + offset,
        Vertices[FaceVertices[face, 1]] + offset,
        Vertices[FaceVertices[face, 2]] + offset,
        Vertices[FaceVertices[face, 3]] + offset,
        Vertices[FaceVertices[face, 4]] + offset
      };
    }

    public static Quaternion GetFaceOrientation(uint face) {
      Debug.Assert(face < 12, "illegal face value");

      Vector3 v1 = Normals[0];
      Vector3 v2 = Normals[face];
      Quaternion faceOrient = Utility.ShortestArc(v2, v1); // quaternion to orient the face to the viewer

      Vector3 p1 = Vertices[FaceVertices[face, (int)CornerId.North]];
      Vector3 p2 = FaceCenter(face);

      Vector3 v3 = p1 - p2;
      v3.Normalize();

      v3 = Vector3.Transform(v3, faceOrient);

      Quaternion faceRotate = Utility.ShortestArc(v3, new Vector3(0, 1, 0)); // quaternion to have the correct up vector
      if (Utility.EffectivelyEqual(Vector3.Dot(v3, new Vector3(0, 1, 0)), -1)) {
        // ShortestArc doesn't care what axis is used to rotate 180 degrees,
        //   in this case it matters
        faceRotate = new Quaternion(new Vector3(0, 0, 1), MathHelper.Pi);
      }

      return faceRotate * faceOrient;
    }

    public static uint NearFace(Quaternion orientation) {
      return NearFace(Vector3.Transform(Normals[0], Quaternion.Inverse(orientation)));
    }

    public static uint NearFace(Vector3 vec) {
      uint[] indices = new uint[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
      return indices.OrderByDescending(i => Vector3.Dot(vec, Normals[i])).First();
    }

    public static Vector3 Normal(uint face) {
      return Normals[face];
    }

    public static Vector2 TexCoord(uint corner) {
      return TexCoords[corner];
    }

    public void RandomizeStarburstRotation(Random rng) {
      const double RandomProportion = 0.05;
      this.starburstRotation = Matrix.CreateFromAxisAngle(Vector3.UnitZ, (float)(rng.NextDouble() * Math.PI * 2)) *
                               Matrix.CreateScale((float)((rng.NextDouble() * RandomProportion) + (1 - RandomProportion)));
      this.textureXOffset = rng.Next(4) * .25f;
    }

    public void Render(GraphicsDevice device, Quaternion orientation, uint face, float angle) {
      Quaternion faceRotation = orientation * Quaternion.CreateFromAxisAngle(Normals[face], angle);

      List<Facet> facets = new List<Facet>();
      for (uint i = 0; i < 12; i++) {
        facets.Add(new CenterFacet().Init(this.data.GetCenter(i), i, orientation, face, faceRotation));
        foreach (CornerId corner in Enum.GetValues(typeof(CornerId))) {
          facets.Add(new CornerFacet().Init(this.data.GetCorner(i, corner), i, corner, orientation, face, faceRotation));
        }
        foreach (EdgeId edge in Enum.GetValues(typeof(EdgeId))) {
          facets.Add(new EdgeFacet().Init(this.data.GetEdge(i, edge), i, edge, orientation, face, faceRotation));
        }
      }

      List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
      List<short> indices = new List<short>();

      facets.Sort(new Comparison<Facet>((x, y) => {
        float xf = x.MinZ;
        float yf = y.MinZ;
        if (xf < yf) { return -1; }
        if (xf > yf) { return 1; }
        return 0;
      }));

      VertexPositionNormalTexture[] starburstVertices = new VertexPositionNormalTexture[4] {
        new VertexPositionNormalTexture(Vector3.Transform(new Vector3(-3, -3, 0), this.starburstRotation), Vector3.UnitZ, new Vector2(this.textureXOffset, .75f)),
        new VertexPositionNormalTexture(Vector3.Transform(new Vector3( 3, -3, 0), this.starburstRotation), Vector3.UnitZ, new Vector2(.25f + this.textureXOffset, .75f)),
        new VertexPositionNormalTexture(Vector3.Transform(new Vector3(-3,  3, 0), this.starburstRotation), Vector3.UnitZ, new Vector2(this.textureXOffset, 1)),
        new VertexPositionNormalTexture(Vector3.Transform(new Vector3( 3,  3, 0), this.starburstRotation), Vector3.UnitZ, new Vector2(.25f + this.textureXOffset, 1)),
      };

      short[] starburstIndices = new short[] { 0, 1, 2, 3, 2, 1 };

      foreach (var facet in facets) {
        facet.PushTriangles(vertices, indices);
      }

      VertexPositionNormalTexture[] vertexArray = vertices.ToArray();
      short[] indexArray = indices.ToArray();

      device.RasterizerState = RasterizerState.CullCounterClockwise;
      device.DepthStencilState = DepthStencilState.None;
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexArray, 0, vertices.Count,
                                       indexArray, 0, indices.Count / 3);
      device.RasterizerState = RasterizerState.CullClockwise;
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, starburstVertices, 0, 4,
                                       starburstIndices, 0, 2);
      device.DepthStencilState = DepthStencilState.Default;
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexArray, 0, vertices.Count,
                                       indexArray, 0, indices.Count / 3);
    }

    public void RotateLeft(uint face) {
      this.data.RotateLeft(face);
    }

    public void RotateRight(uint face) {
      this.data.RotateRight(face);
    }

    public static Vector3 FaceCenter(uint face) {
      return Normals[face] * (float)Math.Cos(Alpha);
    }

    private static Vector3 SphericalToVector3(double radius, double theta, double phi) {
      return new Vector3((float)(radius * Math.Sin(theta) * Math.Cos(phi)),
                         (float)(radius * Math.Sin(theta) * Math.Sin(phi)),
                         (float)(radius * Math.Cos(theta)));
    }
  }

  internal class EdgeFacet : Facet {
    private EdgeId edge;

    public override bool IsAdjacent(uint face) {
      if (face == this.Face) { return true; }
      if (DodecaData.GetFaceAdjacency()[this.Face, (uint)this.edge] == face) { return true; }
      return false;
    }

    internal EdgeFacet Init(EdgeData ed, uint face, EdgeId edge, Quaternion q, uint rotationFace, Quaternion faceRotation) {
      this.edge = edge;
      base.Init(GetVertices(ed, face, edge), face, q, rotationFace, faceRotation);
      return this;
    }

    private static VertexPositionNormalTexture[] GetVertices(EdgeData ed, uint face, EdgeId edge) {
      VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[3];
      {
        CornerId opposite = DodecaData.OppositeCornerFromEdge(edge);
        Vector3 a = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(opposite + 1) % 5]];
        Vector3 b = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(opposite + 2) % 5]];
        Vector3 c = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(opposite + 3) % 5]];
        Vector3 d = DodecaModel.Vertices[DodecaModel.GetFaceVertices[face, (int)(opposite + 4) % 5]];

        vertices[0].Position = (b + c) / 2;
        vertices[1].Position = (b + d) / 2;
        vertices[2].Position = (a + c) / 2;

        vertices[0].Position += DodecaModel.FaceOffset(face);
        vertices[1].Position += ((vertices[0].Position - vertices[1].Position) * DodecaModel.PieceSeperation)
                               + DodecaModel.FaceOffset(face);
        vertices[2].Position += ((vertices[0].Position - vertices[2].Position) * DodecaModel.PieceSeperation)
                               + DodecaModel.FaceOffset(face);
      }
      {
        CornerId opposite = DodecaData.OppositeCornerFromEdge(ed.Edge);
        Vector2 a = DodecaModel.TexCoord((uint)(opposite + 1) % 5);
        Vector2 b = DodecaModel.TexCoord((uint)(opposite + 2) % 5);
        Vector2 c = DodecaModel.TexCoord((uint)(opposite + 3) % 5);
        Vector2 d = DodecaModel.TexCoord((uint)(opposite + 4) % 5);

        vertices[0].TextureCoordinate = DodecaModel.ConvertTextureCoord((b + c) / 2, ed.Face);
        vertices[1].TextureCoordinate = DodecaModel.ConvertTextureCoord((b + d) / 2, ed.Face);
        vertices[2].TextureCoordinate = DodecaModel.ConvertTextureCoord((a + c) / 2, ed.Face);
      }
      vertices[0].Normal = vertices[1].Normal = vertices[2].Normal = DodecaModel.Normal(face);
      return vertices;
    }
  }

  internal abstract class Facet {
    private uint face;
    private float minZ;
    private VertexPositionNormalTexture[] vertices;

    public float MinZ {
      get { return this.minZ; }
    }

    protected uint Face {
      get { return this.face; }
    }

    public abstract bool IsAdjacent(uint rotationFace);

    public void PushTriangles(List<VertexPositionNormalTexture> verts, List<short> indices) {
      short baseIndex = (short)verts.Count;
      verts.AddRange(this.vertices);
      for (short i = 2; i < this.vertices.Length; ++i) {
        indices.Add(baseIndex);
        indices.Add((short)(baseIndex + i - 1));
        indices.Add((short)(baseIndex + i));
      }
    }

    protected virtual void Init(VertexPositionNormalTexture[] vertices, uint face, Quaternion q, uint rotationFace, Quaternion faceRotation) {
      this.face = face;
      bool adjacent = this.IsAdjacent(rotationFace);
      if (adjacent) {
        this.vertices = (from v in vertices
                         select new VertexPositionNormalTexture(Vector3.Transform(v.Position, faceRotation),
                                                                Vector3.Transform(v.Normal, faceRotation),
                                                                v.TextureCoordinate)).ToArray();
      } else {
        this.vertices = (from v in vertices
                         select new VertexPositionNormalTexture(Vector3.Transform(v.Position, q),
                                                                Vector3.Transform(v.Normal, q),
                                                                v.TextureCoordinate)).ToArray();
      }
      this.minZ = (from v in this.vertices select v.Position.Z).Min();
    }
  }
}