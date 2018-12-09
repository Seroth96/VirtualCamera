using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualCamera
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Line> Lines { get; set; }
        public Camera camera { get; set; }
        public List<Vector4> DummyCube { get; set; }
        public List<Vector4> DummyLine { get; set; }
        public Matrix4x4 view { get; set; }
        public List<Polygon3> Polygons { get; set; }
        public Matrix4x4 VP { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            camera = new Camera(new Vector3(0, 0, -10F), new Vector3(0,0,-1));

            DummyCube = new List<Vector4>() {
                new Vector4(-1, -1, -1, 1),
                new Vector4(-1, 1, -1, 1),
                new Vector4(1, 1, -1, 1),
                new Vector4(1, -1, -1, 1),
                new Vector4(-1, -1, 1, 1),
                new Vector4(-1, 1, 1, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(1, -1, 1, 1), 
             };

            DummyLine = new List<Vector4>()
            {
                new Vector4(-1, 1, 1, 1),
                new Vector4(-1, 1, -32, 1)
            };

            camera.UpdateView();

            setLines();
        }

        private void setLines()
        {
            Polygons = new List<Polygon3>();
            VP = camera.ProjectionMatrix * camera.View;
            myCanvas.Children.Clear();

            //row 1
            CreateCube(DummyCube, VP, new Vector4(-3, 0, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(0, 0, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 0, 10, 0));

            CreateCube(DummyCube, VP, new Vector4(0, 0, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(-3, 0, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 0, 13, 0));

            CreateCube(DummyCube, VP, new Vector4(-3, 0, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(0, 0, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 0, 16, 0));

            //row 2
            CreateCube(DummyCube, VP, new Vector4(0, -3, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(-3, -3, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, -3, 10, 0));

            CreateCube(DummyCube, VP, new Vector4(-3, -3, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(0, -3, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, -3, 13, 0));

            CreateCube(DummyCube, VP, new Vector4(-3, -3, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(0, -3, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, -3, 16, 0));

            //row 3
            CreateCube(DummyCube, VP, new Vector4(0, 3, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(-3, 3, 10, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 3, 10, 0));

            CreateCube(DummyCube, VP, new Vector4(-3, 3, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(0, 3, 13, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 3, 13, 0));

            CreateCube(DummyCube, VP, new Vector4(-3, 3, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(0, 3, 16, 0));
            CreateCube(DummyCube, VP, new Vector4(-6, 3, 16, 0));
            //DrawLineFromLocal(DummyLine, new Vector4(-2, 0, 40, 0), VP);
            //DrawLineFromLocal(DummyLine, new Vector4(4, 0, 40, 0), VP);

            BSPTree tree = new BSPTree();
            tree.BuildBSPTree(ref tree, Polygons);
            Draw_BSP_Tree(ref tree, new Vector4 (camera.Position,0));
        }

        private void DrawLineFromLocal(List<Vector4> line, Vector4 translate, Matrix4x4 VP)
        {
            var newPoints = new List<Vector4>();
            for (int i = 0; i < line.Count(); i++)
            {
                var tmp = line[i] + translate;
                //tmp = tmp.Multiply(VP);
                //if (tmp.W != 0)
                //{
                //    tmp /= tmp.W;
                //}
                newPoints.Add(
                    new Vector4(tmp.X,
                        tmp.Y, tmp.Z, tmp.W));
            }
            //var start = line.First() + translate;
            //start = start.Multiply(VP);
            //if (start.W != 0)
            //{
            //    start /= start.W;
            //}

            //var end = line.Last() + translate;
            //end = end.Multiply(VP);
            //if (end.W != 0)
            //{
            //    end /= end.W;
            //}

            Polygons.AddRange(
            new List<Polygon3> {
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        (newPoints[0] + newPoints[1])/2,
                        newPoints[1]
                    }
                )
            }.AsEnumerable()
            );
            //DrawLine(start, end);
        }

        private void CreateCube(List<Vector4> points, Matrix4x4 VP, Vector4 translate)
        {
            var newPoints = new List<Vector4>();
            for (int i = 0; i < points.Count(); i++)
            {
                var tmp = points[i] + translate;
                //tmp = tmp.Multiply(VP);
                //if (tmp.W != 0)
                //{
                //    tmp /= tmp.W;
                //}
                newPoints.Add(
                    new Vector4(tmp.X,
                        tmp.Y, tmp.Z, tmp.W));
            }
            Polygons.AddRange(
                new List<Polygon3> {
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        newPoints[1],
                        newPoints[2]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        newPoints[2],
                        newPoints[3]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[1],
                        newPoints[5],
                        newPoints[6]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[1],
                        newPoints[2],
                        newPoints[6]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[4],
                        newPoints[5],
                        newPoints[6]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[4],
                        newPoints[6],
                        newPoints[7]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        newPoints[4],
                        newPoints[3]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[4],
                        newPoints[3],
                        newPoints[7]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        newPoints[4],
                        newPoints[5]
                    }
                ),
                new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[0],
                        newPoints[1],
                        newPoints[5]
                    }
                ),
                    new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[3],
                        newPoints[2],
                        newPoints[6]
                    }
                ),
                    new Polygon3(
                    new List<Vector4>
                    {
                        newPoints[3],
                        newPoints[7],
                        newPoints[6]
                    }
                )
            }.AsEnumerable()
            );
        }
        void Draw_BSP_Tree(ref BSPTree tree, Vector4 eye)
        {
            if (tree == null)
            {
                return;
            }
            float result = tree.partition.ClassifyPoint(eye);
            //float result = 1;
            if (result > 0)
            {
                Draw_BSP_Tree(ref tree.back, eye);
                Draw_Polygon_List(tree.polygons);
                Draw_BSP_Tree(ref tree.front, eye);
            }
            else if (result < 0)
            {
                Draw_BSP_Tree(ref tree.front, eye);
                Draw_Polygon_List(tree.polygons);
                Draw_BSP_Tree(ref tree.back, eye);
            }
            else // result is 0
            {
                // the eye point is on the partition plane...
                Draw_BSP_Tree(ref tree.back, eye);
                Draw_BSP_Tree(ref tree.front, eye);
            }
        }

        private void DrawLine(Vector4 Start, Vector4 End)
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Black;
            myLine.X1 = 400 * Start.X + 400;
            myLine.X2 = 400 * End.X + 400;
            myLine.Y1 = 800 - (400 * Start.Y + 400);
            myLine.Y2 = 800 - (400 * End.Y + 400);
            myLine.StrokeThickness = 2;
            myCanvas.Children.Add(myLine);
        }

        private void Draw_Polygon_List(List<Polygon3> polygons)
        {
            foreach (var poly in polygons)
            {
                var pointCol = new PointCollection();
                
                for (int  i= 0; i< poly.Points.Count(); i++)
                {
                    var tmp = poly.Points[i].Multiply(VP);
                    if (tmp.W != 0)
                    {
                        tmp /= tmp.W;
                    }
                    pointCol.Add(
                        new Point(400 * tmp.X + 400,
                        800 - (400 * tmp.Y + 400))
                        );
                    //pointCol.Add(
                    //    new Point(400 * poly.Points[i].X + 400,
                    //   800 - (400 * poly.Points[i].Y + 400)));
                }

                Polygon pol = new Polygon();
                pol.Points = pointCol;
                pol.Fill = PickBrush();
                myCanvas.Children.Add(pol);
            }            
        }
        Random rnd = new Random();
        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            while (result == Brushes.Transparent)
            {
                Type brushesType = typeof(Brushes);

                PropertyInfo[] properties = brushesType.GetProperties();

                int random = rnd.Next(properties.Length);
                result = (Brush)properties[random].GetValue(null, null);
            }

            return result;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                camera.MoveForward(1);
            }
            else if (e.Key == Key.D)
            {
                camera.MoveRight(1);
            }
            else if (e.Key == Key.A)
            {
                camera.MoveRight(-1);
            }
            else if (e.Key == Key.S)
            {
                camera.MoveForward(-1);
            }
            else if (e.Key == Key.Q)
            {
                camera.RotateY(0.01F);
            }
            else if (e.Key == Key.E)
            {
                camera.RotateY(-0.01F);
            }
            else if (e.Key == Key.X)
            {
                camera.RotateX(0.01F);
            }
            else if (e.Key == Key.Z)
            {
                camera.RotateX(-0.01F);
            }
            else if (e.Key == Key.U)
            {
                camera.MoveUp(-1);
            }
            else if (e.Key == Key.J)
            {
                camera.MoveUp(1);
            }
            else if (e.Key == Key.Add)
            {
                camera.FieldOfView -= 5d;
            }
            else if (e.Key == Key.Subtract)
            {
                camera.FieldOfView += 5d;
            }
            setLines();
        }
    }
}
