using System;
using System.Linq;
using System.Collections.Generic;
using Elements.Geometry;

// TODO remove this whole file after upgrading to Elements 9.2
namespace GeometryEx
{
    public static class TemporaryVector3Extensions
    {
        public static Vector3 Rounded(this Vector3 point, double precision)
        {
            return new Vector3(Math.Round(point.X / precision) * precision, Math.Round(point.Y / precision) * precision, Math.Round(point.Z / precision) * precision);
        }

        public static int GetHashCode(this Vector3 point, double precision)
        {
            var alt = 17;
            var rounded = point.Rounded(precision);
            alt = alt * 23 + rounded.X.GetHashCode();
            alt = alt * 23 + rounded.Y.GetHashCode();
            alt = alt * 23 + rounded.Z.GetHashCode();
            return alt;
        }
    }


    public class Vector3Comparer : EqualityComparer<Vector3>
    {
        private double _precision = Vector3.EPSILON;
        /// <summary>
        /// Construct a Vector3Comparer, specify a tolerance if you want something other than the default Vector3 tolerance.
        /// </summary>
        public Vector3Comparer(double precision = Vector3.EPSILON)
        {
            _precision = precision;
        }

        /// <summary>
        /// Are the two Vector3 geometrically equal within the tolerance.
        /// </summary>
        public override bool Equals(Vector3 x, Vector3 y)
        {
            return x.IsAlmostEqualTo(y, _precision);
        }

        /// <summary>
        /// Get a hashcode for the Vector3 object.
        /// </summary>
        public override int GetHashCode(Vector3 vector)
        {
            return vector.GetHashCode();
        }
    }
    public class LineComparer : IEqualityComparer<Line>
    {
        private double _precision = 5;
        private bool _directionIndependent = true;

        public LineComparer(double precision = Vector3.EPSILON, bool directionIndependent = true)
        {
            _precision = precision;
            _directionIndependent = directionIndependent;
        }

        public bool Equals(Line x, Line y)
        {
            return (x.Start.IsAlmostEqualTo(y.Start, _precision) && x.End.IsAlmostEqualTo(y.End, _precision))
                    || (_directionIndependent
                        && (x.Start.IsAlmostEqualTo(y.End) && x.End.IsAlmostEqualTo(y.Start)));
        }

        public int GetHashCode(Line obj)
        {
            // If the direction doesn't matter, then we always sort the ends by distance from origin just to have a consistent basis
            if (_directionIndependent
                && Math.Abs(obj.Start.X) + Math.Abs(obj.Start.Y) + Math.Abs(obj.Start.Z) >
                   Math.Abs(obj.End.X) + Math.Abs(obj.End.Y) + Math.Abs(obj.End.Z))
            {
                obj = obj.Reversed();
            }
            int hash = 17;
            hash = hash * 23 + obj.Start.Rounded(_precision).GetHashCode();
            hash = hash * 23 + obj.End.Rounded(_precision).GetHashCode();
            return hash;
        }
    }

    public class TriangleComparer : IEqualityComparer<Triangle>
    {
        private bool _rotationIndependent = false;
        private double _precision = Vector3.EPSILON;
        public TriangleComparer(bool rotationIndependent = false, double precision = Vector3.EPSILON)
        {
            _rotationIndependent = rotationIndependent;
            _precision = precision;
        }
        public bool Equals(Triangle x, Triangle y)
        {
            if (_rotationIndependent)
            {
                return x.IsEqualTo(y);
            }
            else
            {
                return x.Vertices[0].Position.IsAlmostEqualTo(y.Vertices[0].Position, _precision)
                    && x.Vertices[1].Position.IsAlmostEqualTo(y.Vertices[1].Position, _precision)
                    && x.Vertices[2].Position.IsAlmostEqualTo(y.Vertices[2].Position, _precision);
            }
        }

        public int GetHashCode(Triangle obj)
        {
            var vertices = obj.Vertices.ToList();
            if (_rotationIndependent)
            {
                vertices = obj.Vertices.OrderBy(v => Math.Abs(v.Position.X) + Math.Abs(v.Position.Y) + Math.Abs(v.Position.Z)).ToList();
            }

            var alt = 17;
            alt = alt * 23 + vertices[0].Position.GetHashCode(_precision);
            alt = alt * 23 + vertices[1].Position.GetHashCode(_precision);
            alt = alt * 23 + vertices[2].Position.GetHashCode(_precision);

            return alt;
        }
    }
}