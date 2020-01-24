﻿using System.Collections.Generic;
using System.Linq;
using Xunit;
using Elements;
using Elements.Geometry;
using Elements.Serialization.glTF;
using GeometryEx;

namespace GeometryExTests
{
    public class ShaperTests
    {
        [Fact]
        public void AdjacentArea()
        {
            var adjTo =
                new Polygon(
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    });
            var polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.N);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, 9.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 9.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.S);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, -5.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, -5.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.W);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(-5.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(-5.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, 4.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.E);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(4.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(9.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(9.0, 4.0), polygon.Vertices);
        }

        [Fact]
        public void AxisQuad()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    new Vector3(3.0, 1.0),
                    new Vector3(6.0, 1.0),
                    new Vector3(1.0, 6.0),
                    new Vector3(1.0, 3.0)
                }
            );
            var axis = Shaper.AxisQuad(polygon);
            var start = new Vector3(2.0, 2.0);
            var end = new Vector3(3.5, 3.5);

            Assert.Equal(axis.Start.X, start.X);
            Assert.Equal(axis.Start.Y, start.Y);
            Assert.Equal(axis.End.X, end.X);
            Assert.Equal(axis.End.Y, end.Y);
        }

        [Fact]
        public void ExpandToArea()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(1.0, 1.0),
                    new Vector3(8.0, 1.0),
                    new Vector3(8.0, 8.0),
                    new Vector3(1.0, 8.0)
                }
            );
            var among = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(1.0, 3.0),
                        new Vector3(2.0, 3.0),
                        new Vector3(2.0, 6.0),
                        new Vector3(1.0, 6.0),
                    })
            };
            polygon = polygon.ExpandtoArea(20.0, 0.1, Orient.C, within, among);
            var spaces = new List<Space>
            {
                new Space(polygon, 3.0, new Material("blue", Palette.Blue)),
                new Space(within, 0.1, new Material("aqua", Palette.Aqua)),
                new Space(among[0], 3.0, new Material("yellow", Palette.Aqua)),
                new Space(among[1], 3.0, new Material("green", Palette.Green))
            };
            var model = new Model();
            foreach (Space space in spaces)
            {
                model.AddElement(space);
            }
            model.ToGlTF("../../../../expandToArea.glb");
        }

        [Fact]
        public void FitTo()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(1.0, 1.0),
                    new Vector3(8.0, 1.0),
                    new Vector3(8.0, 8.0),
                    new Vector3(1.0, 8.0)
                }
            );
            var among = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(1.0, 3.0),
                        new Vector3(2.0, 3.0),
                        new Vector3(2.0, 6.0),
                        new Vector3(1.0, 6.0),
                    })
            };
            polygon = Shaper.FitTo(polygon, within, among).First();
            var spaces = new List<Space>
            {
                new Space(polygon, 3.0, new Material("blue", new Color(0.0f, 0.0f, 1.0f, 0.6f))),
                new Space(within, 0.1, new Material("aqua", new Color(0.3f, 0.7f, 0.7f, 0.6f))),
                new Space(among[0], 3.0, new Material("yellow", new Color(1.0f, 0.9f, 0.1f, 0.6f))),
                new Space(among[1], 3.0, new Material("green", new Color(0.0f, 1.0f, 0.0f, 0.6f)))
            };
            var model = new Model();
            foreach (Space space in spaces)
            {
                model.AddElement(space);
            }
            model.ToGlTF("../../../../FitTo.glb");
        }

        [Fact]
        public void Merge()
        {
            var polygons = new List<Polygon>
            {
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(10.0, 0.0),
                        new Vector3(20.0, 0.0),
                        new Vector3(20.0, 3.0),
                        new Vector3(10.0, 3.0)
                    }
                )
            };
            var merged = Shaper.Merge(polygons);
            Assert.Equal(2, merged.Count);
        }

        [Fact]
        public void PointWithin()
        {
            var polygons = new List<Polygon>
            {
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 9.0),
                        new Vector3(0.0, 9.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(52.0, 0.0),
                        new Vector3(60.0, 0.0),
                        new Vector3(60.0, 6.0),
                        new Vector3(52.0, 6.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(24.0, 33.0),
                        new Vector3(32.0, 33.0),
                        new Vector3(32.0, 36.0),
                        new Vector3(24.0, 36.0)
                    }
                )
            };
            var point1 = new Vector3(-1.1, -1.1);
            var point2 = new Vector3(26.2, 34.2);
            var point3 = new Vector3(54.5, 3.3);

            Assert.False(Shaper.PointWithin(point1, polygons));
            Assert.True(Shaper.PointWithin(point2, polygons));
            Assert.True(Shaper.PointWithin(point3, polygons));
        }

        [Fact]
        public void PolygonC()
        {
            var polygon = Shaper.PolygonC(new Vector3(2.0, 2.0), new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 7.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 7.0);
        }

        [Fact]
        public void PolygonE()
        {
            var polygon = Shaper.PolygonE(new Vector3(2.0, 2.0), new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 7.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 7.0);
        }

        [Fact]
        public void PolygonF()
        {
            var polygon = Shaper.PolygonF(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void PolygonH()
        {
            var polygon = Shaper.PolygonH(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void PolygonL()
        {
            var polygon = Shaper.PolygonL(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void PolygonT()
        {
            var polygon = Shaper.PolygonT(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 4.0);
        }

        [Fact]
        public void PolygonU()
        {
            var polygon = Shaper.PolygonU(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void PolygonX()
        {
            var polygon = Shaper.PolygonX(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
        }

        [Fact]
        public void RectangleByArea()
        {
            var polygon = Shaper.RectangleByArea(9.0, 1.0);

            Assert.Equal(9.0, polygon.Area());
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 3.0);
            Assert.Contains(polygon.Vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(polygon.Vertices, p => p.X == 3.0 && p.Y == 0.0);
        }

        [Fact]
        public void RectangleByRatio()
        {
            var polygon = Shaper.RectangleByRatio(2.0);

            Assert.Equal(2.0, polygon.Area());
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 2.0);
        }
    }
}
