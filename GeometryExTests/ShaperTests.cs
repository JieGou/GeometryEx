﻿using System;
using System.Collections.Generic;
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
            var polygon = 
                new Polygon
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
        public void Differences()
        {
            var poly1 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(12.0, 0.0),
                    new Vector3(12.0, 5.0),
                    new Vector3(0.0, 5.0)
                }
            );
            var poly2 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 6.0),
                    new Vector3(12.0, 6.0),
                    new Vector3(12.0, 12.0),
                    new Vector3(0.0, 12.0)
                }
            );
            var among = new List<Polygon>()
            {
                new Polygon(
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(7.0, 0.0),
                        new Vector3(7.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(0.0, 6.0),
                        new Vector3(3.0, 6.0),
                        new Vector3(3.0, 12.0),
                        new Vector3(0.0, 12.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(3.0, 4.0),
                        new Vector3(7.0, 4.0),
                        new Vector3(7.0, 9.0),
                        new Vector3(3.0, 9.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(5.0, 9.0),
                        new Vector3(8.0, 9.0),
                        new Vector3(8.0, 12.0),
                        new Vector3(5.0, 12.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(7.0, 2.0),
                        new Vector3(12.0, 2.0),
                        new Vector3(12.0, 7.0),
                        new Vector3(7.0, 7.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(9.0, 7.0),
                        new Vector3(12.0, 7.0),
                        new Vector3(12.0, 12.0),
                        new Vector3(9.0, 12.0)
                    }
                 ),
            };
            var polygons = Polygon.Difference(new List<Polygon> { poly1, poly2 }, among);
            Assert.Equal(4, polygons.Count);
            var matl = new Material("room", Palette.Aqua);
            var model = new Model();
            model.AddElement(new Space(poly1, 0.1, BuiltInMaterials.Concrete));
            model.AddElement(new Space(poly2, 0.1, BuiltInMaterials.Concrete));
            foreach (var shape in polygons)
            {
                model.AddElement(new Space(shape, 4.0, matl));
            }
            model.ToGlTF("../../../../GeometryExTests/output/Shaper.Differences.glb");
        }

        [Fact]
        public void CornerLines()
        {
            var thisLine = new Line(Vector3.Origin, new Vector3(4.0, 4.0));
            var thatLine = new Line(new Vector3(6.0, 0.0), new Vector3(6.0, 1.0));
            var lines = Shaper.CornerLines(thisLine, thatLine);
            Assert.True(lines.First().End.X.NearEqual(6.0));
            Assert.True(lines.Last().End.Y.NearEqual(6.0));

            thisLine = new Line(new Vector3(4.0, 4.0), Vector3.Origin);
            thatLine = new Line(new Vector3(6.0, 0.0), new Vector3(6.0, 1.0));
            lines = Shaper.CornerLines(thisLine, thatLine);
            Assert.True(lines.First().Start.X.NearEqual(6.0));
            Assert.True(lines.Last().End.Y.NearEqual(6.0));
        }

        [Fact]
        public void DifferencesTests()
        {
            for (var i = 0; i < 5; i++)
            {
                var ROTATE = ((int)(Shaper.RandomDouble(0.0, 360.0) * 100)) * 0.01;

                var polygon = Polygon.Rectangle(Vector3.Origin, new Vector3(100.0, 50.0)).Rotate(Vector3.Origin, ROTATE);
                var subtract = Polygon.Rectangle(Vector3.Origin, new Vector3(20.0, 20.0));
                var subtracts = new List<Polygon>();
                for (var j = 1; j < 5; j+=20)
                {
                    subtracts.Add(subtract.MoveFromTo(Vector3.Origin, new Vector3(j, 0.0)).Rotate(Vector3.Origin, ROTATE));
                }
                subtract = Polygon.Rectangle(new Vector3(0.0, 30.0), new Vector3(20.0, 50.0));
                for (var j = 1; j < 5; j+=20)
                {
                    subtracts.Add(subtract.MoveFromTo(Vector3.Origin, new Vector3(j, 0.0)).Rotate(Vector3.Origin, ROTATE));
                }
                var polygons = Shaper.Differences(polygon.ToList(), subtracts, 0.01);
                var matl = new Material("space", Palette.Aqua);
                var model = new Model();
                var count = 0;
                foreach (var shape in polygons)
                {
                    model.AddElement(new Space(shape, 0.1, BuiltInMaterials.Concrete));
                    count++;
                }
                foreach (var shape in subtracts)
                {
                    model.AddElement(new Space(shape, 4.0, matl));
                    count++;
                }
                var fileName = "../../../../GeometryExTests/output/Shaper.DifferenceTest" + ROTATE.ToString() + ".glb";
                model.ToGlTF(fileName);
            }
        }

        [Fact]
        public void FitWithin()
        {
            var within = ShapeMaker.U(Vector3.Origin, new Vector3(40.0, 40.0), 10.0);
            var fit = Polygon.Rectangle(new Vector3(-10.0, 20.0), new Vector3(50.0, 30.0));
            var polygons = Shaper.FitWithin(fit, within);

            Assert.Equal(2, polygons.Count);

            var vertices = polygons.First().Vertices.ToList();
            vertices.AddRange(polygons.Last().Vertices);

            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 10.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 10.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 30.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 30.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 40.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 40.0 && p.Y == 30.0);
        }
        
        [Fact]
        public void InQuadrant()
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
                        new Vector3(-5.0, 0.0),
                        new Vector3(-8.0, 0.0),
                        new Vector3(-8.0, 20.0),
                        new Vector3(-5.0, 20.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(-10.0, -1.0),
                        new Vector3(-20.0, -1.0),
                        new Vector3(-20.0, -3.0),
                        new Vector3(-10.0, -3.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(10.0, 0.0),
                        new Vector3(20.0, 0.0),
                        new Vector3(20.0, -3.0),
                        new Vector3(10.0, -3.0)
                    }
                )
            };
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.I));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.II));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.III));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.IV));
        }

        [Fact]
        public void Intersections()
        {
            var polygons = 
                new List<Polygon>
                {
                    new Polygon
                    (
                        new []
                        {
                            new Vector3(3.0, 1.0),
                            new Vector3(10.0, 1.0),
                            new Vector3(10.0, 5.0),
                            new Vector3(3.0, 5.0)
                        }
                    )
                };
            var inters =
                new List<Polygon>
                {
                    new Polygon
                    (
                        new []
                        {
                            new Vector3(7.0, 0.0),
                            new Vector3(8.0, 0.0),
                            new Vector3(8.0, 7.0),
                            new Vector3(7.0, 7.0)
                        }
                    ),
                    new Polygon
                    (
                        new []
                        {
                            new Vector3(1.0, 3.0),
                            new Vector3(5.0, 3.0),
                            new Vector3(5.0, 7.0),
                            new Vector3(1.0, 7.0)
                        }
                    )
                };
            var intersects = Shaper.Intersections(polygons, inters);
            Assert.Equal(2, intersects.Count);
        }
        //<image url="$(ProjectDir)\DocumentImages\MakePolygonNeedOptimization.png"/>
        [Fact]
        public void MakePolygon()
        {
            var points =
                new List<Vector3>
                {
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 4.0),
                    new Vector3(6.0, 11.0),
                    new Vector3(6.0, 7.0),
                    new Vector3(2.0, 7.0),
                    new Vector3(2.0, 4.0),
                    new Vector3(9.0, 0.0),
                    new Vector3(9.0, 4.0),
                    new Vector3(13.0, 7.0),
                    new Vector3(9.0, 7.0),
                    new Vector3(9.0, 11.0),
                    new Vector3(6.0, 4.0),
                    new Vector3(6.0, 0.0),
                };
            var polygon = Shaper.MakePolygon(points);
            Assert.Equal(9, polygon.Vertices.Count);
        }
        //<image url="$(ProjectDir)\DocumentImages\MakePolygonOrderedPoints.png"/>
        [Fact]
        public void MakePolygonNew()
        {
            var points =
                new List<Vector3>
                {
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 7.0),
                    new Vector3(9.0, 7.0),
                    new Vector3(9.0, 11.0),
                    new Vector3(6.0, 11.0),
                    new Vector3(6.0, 7.0),
                    new Vector3(2.0, 7.0),
                    new Vector3(2.0, 4.0),
                    new Vector3(6.0, 4.0),
                    new Vector3(6.0, 0.0),
                    new Vector3(9.0, 0.0),
                    new Vector3(9.0, 4.0),
                };
            var polygon = Shaper.MakePolygon(points);
            Assert.Equal(12, polygon.Vertices.Count);
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
        public void NearPolygons()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                );
            var nearPolygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                );
            var polygons = Shaper.NearPolygons(polygon, nearPolygon, true);
            Assert.Equal(32, polygons.Count);
        }

        [Fact]
        public void NonIntersecting()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                );
            var nearPolygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                );
            var polygons = Shaper.NearPolygons(polygon, nearPolygon, true);
            polygons = Shaper.NonIntersecting(polygon.ToList(), polygons);
            Assert.Equal(24, polygons.Count);
        }

        [Fact]
        public void PlaceOrthogonal()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(10.0, 0.0),
                    new Vector3(10.0, 6.0),
                    new Vector3(0.0, 6.0)
                }
            );
            var place = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(6.0, 0.0),
                    new Vector3(6.0, 3.0),
                    new Vector3(0.0, 3.0)
                }
            );

            // Horizontal polygon, northeast, minimum coord
            place = Shaper.PlaceOrthogonal(polygon, place, true, true);
            var points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 9.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 6.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 6.0) && Shaper.NearEqual(p.Y, 9.0));

            // Vertical polygon, northeast, minimum coord
            polygon = polygon.Rotate(Vector3.Origin, 90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, true, true);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 3.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 3.0) && Shaper.NearEqual(p.Y, 6.0));

            // Horizontal polygon, northeast, maximum coord
            polygon = polygon.Rotate(Vector3.Origin, -90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, true, false);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 10.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 10.0) && Shaper.NearEqual(p.Y, 9.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 4.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 4.0) && Shaper.NearEqual(p.Y, 9.0));

            // Vertical polygon, northeast, maximum coord
            polygon = polygon.Rotate(Vector3.Origin, 90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, true, false);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 10.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 3.0) && Shaper.NearEqual(p.Y, 10.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 4.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 3.0) && Shaper.NearEqual(p.Y, 4.0));

            // Horizontal polygon, southwest, minimum coord
            polygon = polygon.Rotate(Vector3.Origin, -90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, false, true);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 6.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 0.0) && Shaper.NearEqual(p.Y, -3.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 6.0) && Shaper.NearEqual(p.Y, -3.0));

            // Vertical polygon, southwast, minimum coord
            polygon = polygon.Rotate(Vector3.Origin, 90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, false, true);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -6.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -9.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -6.0) && Shaper.NearEqual(p.Y, 6.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -9.0) && Shaper.NearEqual(p.Y, 6.0));

            // Horizontal polygon, southwest, maximum coord
            polygon = polygon.Rotate(Vector3.Origin, -90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, false, false);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 4.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 10.0) && Shaper.NearEqual(p.Y, 0.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 4.0) && Shaper.NearEqual(p.Y, -3.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, 10.0) && Shaper.NearEqual(p.Y, -3.0));

            // Vertical polygon, southwest, maximum coord
            polygon = polygon.Rotate(Vector3.Origin, 90.0);
            place = Shaper.PlaceOrthogonal(polygon, place, false, false);
            points = place.Vertices;
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -6.0) && Shaper.NearEqual(p.Y, 10.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -9.0) && Shaper.NearEqual(p.Y, 10.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -6.0) && Shaper.NearEqual(p.Y, 4.0));
            Assert.Contains(points, p => Shaper.NearEqual(p.X, -9.0) && Shaper.NearEqual(p.Y, 4.0));
        }

        [Fact]
        public void PointsToLines()
        {
            var polygon = ShapeMaker.U(Vector3.Origin, new Vector3(40.0, 40.0), 10.0);
            var points = polygon.Vertices;
            Assert.Equal(8.0, Shaper.PointsToLines(points.ToList(), true).Count);
        }

        [Fact]
        public void RectangleByArea()
        {
            //var polygon = Shaper.RectangleByArea(136.5, 1.3877787807814457);

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

        [Fact]
        public void Simplify()
        {
            var points = new List<Vector3>()
            {
                Vector3.Origin,
                new Vector3(5.0, 1.0),
                new Vector3(7.0, 0.0),
                new Vector3(10.0, 2.0),
                new Vector3(15.0, 0.0),
                new Vector3(20.0, 2.0),
                new Vector3(25.0, 0.0),
            };
            points = Shaper.Simplify(points, 5.0);
            Assert.Equal(6, points.Count);
        }

        [Fact]
        public void SortRadial()
        {
            var points =
                new List<Vector3>
                {
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 4.0),
                    new Vector3(6.0, 11.0),
                    new Vector3(6.0, 7.0),
                    new Vector3(2.0, 7.0),
                    new Vector3(2.0, 4.0),
                    new Vector3(9.0, 0.0),
                    new Vector3(9.0, 4.0),
                    new Vector3(13.0, 7.0),
                    new Vector3(9.0, 7.0),
                    new Vector3(9.0, 11.0),
                    new Vector3(6.0, 4.0),
                    new Vector3(6.0, 0.0),
                };
            var sorted = Shaper.SortRadial(points, new Vector3(7.5, 5.5));
            Assert.Equal(12, sorted.Count());
            Assert.Equal(6.0, sorted.First().X);
            Assert.Equal(0.0, sorted.First().Y);
            Assert.Equal(9.0, sorted.Last().X);
            Assert.Equal(0.0, sorted.Last().Y);
        }
    }
}
