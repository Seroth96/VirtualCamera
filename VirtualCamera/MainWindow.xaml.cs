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
        public Matrix4x4 projectionMatrix { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            camera = new Camera(new Vector3(50, 0, 150F), new Vector3(150, 200, 250F));

            projectionMatrix = camera.GetProjectionMatrix(1);
            setLines();
        }

        private void setLines()
        { 
            Vector4 p1 = new Vector4(100, 150, 200, 1);
            Vector4 p2 = new Vector4(100, 250, 200, 1);
            Vector4 p3 = new Vector4(200, 250, 200, 1);
            Vector4 p4 = new Vector4(200, 150, 200, 1);
            Vector4 p12 = new Vector4(100, 150, 300, 1);
            Vector4 p22 = new Vector4(100, 250, 300, 1);
            Vector4 p32 = new Vector4(200, 250, 300, 1);
            Vector4 p42 = new Vector4(200, 150, 300, 1);


            p1 = p1.Multiply(camera.View).Multiply(projectionMatrix);
            p2 = p2.Multiply(camera.View).Multiply(projectionMatrix);
            p3 = p3.Multiply(camera.View).Multiply(projectionMatrix);
            p4 = p4.Multiply(camera.View).Multiply(projectionMatrix);
            p12 = p12.Multiply(camera.View).Multiply(projectionMatrix);
            p22 = p22.Multiply(camera.View).Multiply(projectionMatrix);
            p32 = p32.Multiply(camera.View).Multiply(projectionMatrix);
            p42 = p42.Multiply(camera.View).Multiply(projectionMatrix);         

            myCanvas.Children.Clear();
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Red;
            myLine.X1 = camera.Position.X;
            myLine.X2 = camera.Target.X;
            myLine.Y1 = camera.Position.Y;
            myLine.Y2 = camera.Target.Y;
            myLine.StrokeThickness = 5;
            myCanvas.Children.Add(myLine); 
            DrawLine(p1, p2);
            DrawLine(p2, p3);
            DrawLine(p3, p4);
            DrawLine(p4, p1);
            DrawLine(p12, p22);
            DrawLine(p22, p32);
            DrawLine(p32, p42);
            DrawLine(p42, p12);
            DrawLine(p1, p12);
            DrawLine(p2, p22);
            DrawLine(p3, p32);
            DrawLine(p4, p42);
        }

        private void DrawLine(Vector4 Start, Vector4 End)
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.Black;
            myLine.X1 = Start.X;
            myLine.X2 = End.X;
            myLine.Y1 = Start.Y;
            myLine.Y2 = End.Y;
            myLine.StrokeThickness = 2;
            myCanvas.Children.Add(myLine);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                camera.MoveForward(15);
            }
            else if (e.Key == Key.D)
            {
                camera.MoveRight(15);
            }
            else if (e.Key == Key.A)
            {
                camera.MoveRight(-5);
            }
            else if (e.Key == Key.S)
            {
                camera.MoveForward(-5);
            }
            else if (e.Key == Key.Q)
            {
                camera.RotateY(0.05F);
            }
            else if (e.Key == Key.E)
            {
                camera.RotateY(-0.05F);
            }
            else if (e.Key == Key.X)
            {
                camera.RotateX(0.05F);
            }
            else if (e.Key == Key.Z)
            {
                camera.RotateX(-0.05F);
            }
            

            setLines();
        }
    }
}
