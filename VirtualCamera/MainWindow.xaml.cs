using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public MainWindow()
        {
            InitializeComponent();
            camera = new Camera(new Vector3(0, 0, -10F), new Vector3(0, 0, -1));

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

            setLines();
        }

        private void setLines()
        {            
            var VP = camera.ProjectionMatrix * camera.View;
            myCanvas.Children.Clear();
            CreateCube(DummyCube, VP, new Vector4(5, 0, 10, 1));
            CreateCube(DummyCube, VP, new Vector4(-5, 0, 10, 1));
        }

        private void CreateCube(List<Vector4> points, Matrix4x4 VP, Vector4 translate)
        {
            var newPoints = new List<Vector4>();
            for (int i = 0; i < points.Count(); i++)
            {
                var tmp = points[i] + translate;
                tmp = tmp.Multiply(VP);
                newPoints.Add(tmp / tmp.W);
            }

            DrawLine(newPoints[0], newPoints[1]);
            DrawLine(newPoints[1], newPoints[2]);
            DrawLine(newPoints[2], newPoints[3]);
            DrawLine(newPoints[3], newPoints[0]);
            DrawLine(newPoints[4], newPoints[5]);
            DrawLine(newPoints[5], newPoints[6]);
            DrawLine(newPoints[6], newPoints[7]);
            DrawLine(newPoints[7], newPoints[4]);
            DrawLine(newPoints[0], newPoints[4]);
            DrawLine(newPoints[1], newPoints[5]);
            DrawLine(newPoints[2], newPoints[6]);
            DrawLine(newPoints[3], newPoints[7]);

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                camera.MoveForward(1);
            }
            else if (e.Key == Key.D)
            {
                camera.MoveRight(-1);
            }
            else if (e.Key == Key.A)
            {
                camera.MoveRight(1);
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
                camera.MoveUp(1);
            }
            else if (e.Key == Key.J)
            {
                camera.MoveUp(-1);
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
