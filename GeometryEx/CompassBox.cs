﻿using System;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Maintains a set of points on the orthogonal bounding box of a supplied Polygon corresponding to four divisions of each side.
    /// </summary>
    public class CompassBox
    {
        /// <summary>
        /// Vector3 location identifier corresponding to the center of the box perimeter.
        /// </summary>
        public Vector3 C { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint of the maximum Y side of the box perimeter.
        /// </summary>
        public Vector3 N { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the NW and N points of the box perimeter.
        /// </summary>
        public Vector3 NNW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the mimimum X and maximum Y corner of the box perimeter.
        /// </summary>
        public Vector3 NW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the NW and W points of the box perimeter.
        /// </summary>
        public Vector3 WNW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint of the minimum X side of the box perimeter.
        /// </summary>
        public Vector3 W { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the SW and W points of the box perimeter.
        /// </summary>
        public Vector3 WSW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the mimimum X and Y corner of the box perimeter.
        /// </summary>
        public Vector3 SW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the SW and S points of the box perimeter.
        /// </summary>
        public Vector3 SSW { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint of the minimum Y side of the box perimeter.
        /// </summary>
        public Vector3 S { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the SE and S points of the box perimeter.
        /// </summary>
        public Vector3 SSE { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the maximum X and minimum Y corner of the box perimeter.
        /// </summary>
        public Vector3 SE { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the SE and E points of the box perimeter.
        /// </summary>
        public Vector3 ESE { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint of the maximum X side of the box perimeter.
        /// </summary>
        public Vector3 E { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the NE and E points of the box perimeter.
        /// </summary>
        public Vector3 ENE { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the maximum X and Y corner of the box perimeter.
        /// </summary>
        public Vector3 NE { get; }

        /// <summary>
        /// Vector3 location identifier corresponding to the midpoint between the NE and N  points of the box perimeter.
        /// </summary>
        public Vector3 NNE { get; }

        /// <summary>
        /// Returns a list of all Topobox points except for the center(C).
        /// </summary>
        /// <returns></returns>
        public List<Vector3> Compass
        {
            get
            {
                return new List<Vector3>
                {
                    SW, SSW, S, SSE, SE, ESE, E, ENE, NE, NNE, N, NNW, NW, WNW, W, WSW
                };
            }
        }

        /// <summary>
        /// Returns the x : y aspect ratio of the bounding box.
        /// </summary>
        public double AspectRatio
        {
            get
            {
                return SizeX / SizeY;
            }
        }

        /// <summary>
        /// Returns a Polygon of the SW, SE, NE, and NW points.
        /// </summary>
        public Polygon Box
        {
            get
            {
                return Shaper.MakePolygon(new List<Vector3>() { SW, SE, NE, NW });
            }
        }

        /// <summary>
        /// X and Y dimensions of the CompassBox perimeter.
        /// </summary>
        public double SizeX { get; }
        public double SizeY { get; }

        /// <summary>
        /// Constructor creates a new mathematical bounding box from the supplied Polygon and populates all orientation points.
        /// </summary>
        /// <returns>
        /// A new TopoBox.
        /// </returns>
        public CompassBox(Polygon polygon)
        {
            if (polygon == null)
            {
                return;
            }
            var vertices = new List<Vector3>(polygon.Vertices);
            vertices.Sort((a, b) => a.X.CompareTo(b.X));
            var minX = vertices[0].X;
            vertices.Sort((a, b) => b.X.CompareTo(a.X));
            var maxX = vertices[0].X;
            vertices.Sort((a, b) => a.Y.CompareTo(b.Y));
            var minY = vertices[0].Y;
            vertices.Sort((a, b) => b.Y.CompareTo(a.Y));
            var maxY = vertices[0].Y;

            SizeX = Math.Abs(maxX - minX);
            SizeY = Math.Abs(maxY - minY);

            C = new Vector3(minX + (SizeX * 0.5), minY + (SizeY * 0.5));
            N = new Vector3(minX + (SizeX * 0.5), maxY);
            NNW = new Vector3(minX + (SizeX * 0.25), maxY);
            NW = new Vector3(minX, maxY);
            WNW = new Vector3(minX, minY + (SizeY * 0.75));
            W = new Vector3(minX, minY + (SizeY * 0.5));
            WSW = new Vector3(minX, minY + (SizeY * 0.25));
            SW = new Vector3(minX, minY);
            SSW = new Vector3(minX + (SizeX * 0.25), minY);
            S = new Vector3(minX + (SizeX * 0.5), minY);
            SSE = new Vector3(minX + (SizeX * 0.75), minY);
            SE = new Vector3(maxX, minY);
            ESE = new Vector3(maxX, minY + (SizeY * 0.25));
            E = new Vector3(maxX, minY + (SizeY * 0.5));
            ENE = new Vector3(maxX, minY + (SizeY * 0.75));
            NE = new Vector3(maxX, maxY);
            NNE = new Vector3(minX + (SizeX * 0.75), maxY);
        }

        /// <summary>
        /// Returns the requested bounding box location by orientation.
        /// </summary>
        /// <param name="orient">Location identifier of the desired point.</param>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 PointBy(Orient orient)
        {
            switch(orient)
            {
                case Orient.C: return C;
                case Orient.N: return N;
                case Orient.NNW: return NNW;
                case Orient.NW: return NW;
                case Orient.WNW: return WNW;
                case Orient.W: return W;
                case Orient.WSW: return WSW;
                case Orient.SW: return SW;
                case Orient.SSW: return SSW;
                case Orient.S: return S;
                case Orient.SSE: return SSE;
                case Orient.SE: return SE;
                case Orient.ESE: return ESE;
                case Orient.E: return E;
                case Orient.ENE: return ENE;
                case Orient.NE: return NE;
                case Orient.NNE: return NNE;
            }
            return new Vector3(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns the reciprocal bounding box location by orientation.
        /// </summary>
        /// <param name="orient">The Orient value to find the reciprocal point.</param>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 PointOpposite(Orient orient)
        {
            switch (orient)
            {
                case Orient.C: return C;
                case Orient.N: return S;
                case Orient.NNW: return SSE;
                case Orient.NW: return SE;
                case Orient.WNW: return ESE;
                case Orient.W: return E;
                case Orient.WSW: return ENE;
                case Orient.SW: return NE;
                case Orient.SSW: return NNE;
                case Orient.S: return N;
                case Orient.SSE: return NNW;
                case Orient.SE: return NW;
                case Orient.ESE: return WNW;
                case Orient.E: return W;
                case Orient.ENE: return WSW;
                case Orient.NE: return SW;
                case Orient.NNE: return SSW;
            }
            return new Vector3(double.NaN, double.NaN);
        }
    }
}
