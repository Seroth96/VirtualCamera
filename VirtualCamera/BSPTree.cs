using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;
using System.Windows.Shapes;
using System.Windows.Media;

namespace VirtualCamera
{
   public class Polygon3
    {
        public List<Vector4> Points = new List<Vector4>();
        public Polygon3(List<Vector4> points)
        {
            Points = points;
        }
        public Polygon3()
        {

        }

        public int NumVertices()
        {
            return Points.Count();
        }
    }
    public class Plane
    {
        private Vector4 iloczynWektorowy(Vector4 A, Vector4 B)
        {
            Vector4 x = new Vector4();

            x.X = (A.Y * B.Z) - (A.Z * B.Y);
            x.Y = (A.Z * B.X) - (A.X * B.Z); 
            x.Z = (A.X * B.Y) - (A.Y * B.X);
            return x;
        }
        public Plane(Vector4 A, Vector4 B, Vector4 C)
        {
            var AB = B - A;
            var AC = C - A;
            Normal = iloczynWektorowy(AB, AC);
            D = Normal.X * (-A.X) + Normal.Y * (-A.Y) + Normal.Z * (-A.Z);
        }
        public Vector4 Normal { get; set; }
        public float D { get; set; }

        internal Result ClassifyPolygon(Polygon3 poly)
        {
            List<double> results = new List<double>();
            foreach (var point in poly.Points)
            {
                var x = (Normal.X * point.X + Normal.Y * point.Y + Normal.Z * point.Z + D);
                if (Math.Floor(x) == 0)
                {
                    results.Add(0);
                }
                else
                {
                    results.Add(x);
                }
            }

            if (results.Any(result => result  <= 0))
            {
                if (results.Any(result => result >= 0))
                {
                    if (results.Any(result => result == 0))
                    {
                        if (results.Any(result => result != 0))
                        {
                            if (results.Any(result => result < 0))
                            {
                                if (results.Any(result => result > 0))
                                {
                                    return Result.SPANNING; //0-+ 0+- +0- +-0 -0+ -+0
                                }
                                else
                                {
                                    return Result.IN_BACK_OF; //0-- -00 --0 00- -0- 0-0
                                }
                            }
                            else
                            {
                                return Result.IN_FRONT_OF; //0++ +00 ++0 00+ +0+ 0+0
                            }                            
                        }
                        else
                        {
                            return Result.COINCIDENT; //000
                        }
                    }
                    else
                    {
                        return Result.SPANNING; //++- +-- --+ +-+ -+- -++
                    }
                }
                else
                {
                    return Result.IN_BACK_OF;//---
                }
            }
            else
            {
                return Result.IN_FRONT_OF; //+++
            }
        }

        internal float ClassifyPoint(Vector4 ptA)
        {
            return Normal.X * ptA.X + Normal.Y * ptA.Y + Normal.Z * ptA.Z + D;
        }
    }

    public class BSPTree
    {
        public int MAXPTS = 10;
        public Plane partition;
        public List<Polygon3> polygons = new List<Polygon3>();
        public BSPTree back;
        public BSPTree front;

        

        public void BuildBSPTree(ref BSPTree tree, List<Polygon3> polygons)
        {           
            Polygon3 root = polygons.First();
            polygons.RemoveAt(0);
            tree.partition = new Plane(root.Points[0], root.Points[1], root.Points[2]);
            tree.polygons.Add(root);

            List<Polygon3> frontlist = new List<Polygon3>();
            List<Polygon3> backlist = new List<Polygon3>();
            while (polygons.Count > 0)
            {
                var poly = polygons.First();
                polygons.RemoveAt(0);
                var result =  tree.partition.ClassifyPolygon(poly);
                switch (result)
                {
                    case Result.COINCIDENT:
                        tree.polygons.Add(poly);
                        break;
                    case Result.IN_BACK_OF:
                        backlist.Add(poly);
                        break;
                    case Result.IN_FRONT_OF:
                        frontlist.Add(poly);
                        break;
                    case Result.SPANNING:
                        Polygon3 front_piece = new Polygon3(), back_piece = new Polygon3();
                        Split_Polygon(poly, tree.partition, ref front_piece, ref back_piece);
                        backlist.Add(back_piece);
                        frontlist.Add(front_piece);
                        break;
                }
            }
            if (!(frontlist.Count() == 0))
            {
                tree.front = new BSPTree();
                BuildBSPTree(ref tree.front, frontlist);
            }
            if (!(backlist.Count() == 0))
            {
                tree.back = new BSPTree();
                BuildBSPTree(ref tree.back, backlist);
            }
        }

        private void Split_Polygon(Polygon3 poly, Plane part, ref Polygon3 front, ref Polygon3 back)
        {
            int count = poly.NumVertices(),
                out_c = 0, in_c = 0;
            Vector4 ptA, ptB;
            double sideA, sideB;
            Vector4[] outpts = new Vector4[MAXPTS],
                inpts = new Vector4[MAXPTS];
            List<Vector4> outptsl = new List<Vector4>(),
                inptsl = new List<Vector4>();
            ptA = poly.Points.Last();
            sideA = part.ClassifyPoint(ptA);
            for (short i = -1; ++i < count;)
            {
                ptB = poly.Points.ElementAt(i);
                sideB = part.ClassifyPoint(ptB);
                if (sideB > 0)
                {
                    if (sideA < 0)
                    {
                        // compute the intersection point of the line
                        // from point A to point B with the partition
                        // plane. This is a simple ray-plane intersection.
                        Vector4 v = ptB - ptA;
                        float sect = -part.ClassifyPoint(ptA) / Vector4.Distance(part.Normal,v);
                        outptsl.Add(ptA + (v * sect));
                        inptsl.Add(ptA + (v * sect));
                        in_c++;
                        out_c++;
                    }
                    //outpts[out_c++] = ptB;
                    outptsl.Add(ptB);
                    out_c++;
                }
                else if (sideB < 0)
                {
                    if (sideA > 0)
                    {
                        // compute the intersection point of the line
                        // from point A to point B with the partition
                        // plane. This is a simple ray-plane intersection.
                        Vector4 v = ptB - ptA;
                        float sect = -part.ClassifyPoint(ptA) / Vector4.Distance(part.Normal, v);
                        //outpts[out_c++] = inpts[in_c++] = ptA + (v * sect);
                        outptsl.Add(ptA + (v * sect));
                        inptsl.Add(ptA + (v * sect));
                        in_c++;
                        out_c++;
                    }
                    inptsl.Add(ptB);
                    in_c++;
                }
                else
                {
                   // outpts[out_c++] = inpts[in_c++] = v;
                    outptsl.Add(ptB);
                    inptsl.Add(ptB);
                    in_c++;
                    out_c++;
                }
                ptA = ptB;
                sideA = sideB;
            }
            front = new Polygon3();
            front.Points = outptsl.ToList();
            back = new Polygon3();
            back.Points = inptsl.ToList();
        }

        

    }

    public enum Result
    {
        COINCIDENT,
        IN_BACK_OF,
        IN_FRONT_OF,
        SPANNING
    }


}
