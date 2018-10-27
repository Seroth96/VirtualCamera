using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VirtualCamera
{
    public class Camera
    {
        #region Properties
        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Camera Target - point to look at
        /// </summary>
        public Vector3 Target
        {
            get
            {
                Vector3 v = Vector3.UnitX;
                v = Vector3.Transform(v, Matrix4x4.CreateRotationY(phi /*- (float)Math.PI*/));
                //v = Vector3.Transform(v, Matrix4x4.CreateRotationX(theta));
                //v += Position;
                return v;
            }

            set
            {
                // calculate angles
                Vector3 dir = Vector3.Normalize(value/* - Position*/);

                float p = (float)Math.Sqrt(dir.X * dir.X + dir.Z * dir.Z);
                theta = (float)Math.Atan2(dir.Y, p);

                phi = CalculatePhi(dir);
            }
        }

        private double _fieldOfView;
        public double FieldOfView {
            get { return _fieldOfView; }
            set {
                if (value < 179 && value > 1)
                {
                    _fieldOfView = value;
                }
            }
        }

        public Matrix4x4 View
        {
            get; set;
        }

        public Matrix4x4 ProjectionMatrix
        {
            get { return GetProjectionMatrix(1); }
        }

        private Matrix4x4 CreateViewMatrix(Vector3 position, Vector3 target, Vector3 Up)
        {
            Vector3 zaxis = Vector3.Normalize(/*position - */target);    // The "forward" vector.
            Vector3 xaxis = Vector3.Normalize(Vector3.Cross(Up, zaxis));// The "right" vector.
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);     // The "up" vector.

            // Create a 4x4 orientation matrix from the right, up, and forward vectors
            // This is transposed which is equivalent to performing an inverse 
            // if the matrix is orthonormalized
            Matrix4x4 orientation = new Matrix4x4()
            {
                M11 = xaxis.X,
                M12 = yaxis.X,
                M13 = zaxis.X,
                M14 = 0,
                M21 = xaxis.Y,
                M22 = yaxis.Y,
                M23 = zaxis.Y,
                M24 = 0,
                M31 = xaxis.Z,
                M32 = yaxis.Z,
                M33 = zaxis.Z,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1
            };
            orientation = Matrix4x4.Transpose(orientation);

            Matrix4x4 translation = new Matrix4x4()
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = -position.X,
                M42 = -position.Y,
                M43 = -position.Z,
                M44 = 1
            };
            translation = Matrix4x4.Transpose(translation);

            return (orientation * translation);
        }

        private Quaternion cameraQuat;
        //private Quaternion keyQuat;
        public void UpdateView()
        {
            //keyQuat = Quaternion.CreateFromYawPitchRoll(AngleY, AngleX, 0);
            var x = Quaternion.CreateFromAxisAngle(Vector3.UnitX, AngleX);
            var y = Quaternion.CreateFromAxisAngle(Vector3.UnitY, AngleY);
            AngleX = AngleY = 0;

            cameraQuat = y * (cameraQuat * x);
           // var quat = Quaternion.Normalize(cameraQuat);
            var orient = Matrix4x4.CreateFromQuaternion(cameraQuat);

            // Create a 4x4 translation matrix.
            // The eye position is negated which is equivalent
            // to the inverse of the translation matrix. 
            // T(v)^-1 == T(-v)
            Matrix4x4 translation = new Matrix4x4 (){
                M11 =1, M12 =0, M13 = 0, M14 = 0 ,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0 ,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0 ,       
                M41 = -Position.X, M42 = -Position.Y, M43 = -Position.Z, M44 = 1 
            };
            translation = Matrix4x4.Transpose(translation);            

            // Combine the orientation and translation to compute 
            // the final view matrix. Note that the order of 
            // multiplication is reversed because the matrices
            // are already inverted.
            View = (orient * translation);
        }

        private Vector3 Up
        {
            get
            {
                Vector3 v = Vector3.UnitY;
                v = Vector3.Transform(v, Matrix4x4.CreateRotationX(theta));
                v = Vector3.Transform(v, Matrix4x4.CreateRotationY(phi - (float)(Math.PI/2)));
                return Vector3.Normalize(v);
            }
        }

        private Vector3 Right
        {
            get { return Vector3.Normalize(Vector3.Cross(Vector3.Normalize(Target/* - Position*/), Up)); }
        }
        #endregion

        #region Constructors
        public Camera()
        {
            Position = Vector3.Zero;
            phi = 0.0f;
            theta = 0.0f;
            FieldOfView = 45d;
            cameraQuat = Quaternion.CreateFromYawPitchRoll(AngleY, AngleX, 0);
        }

        public Camera(Vector3 position)
            : this()
        {
            Position = position;
        }

        public Camera(Vector3 position, Vector3 target)
        {
            Position = position;
            Target = target;
            FieldOfView = 45d;
            cameraQuat = Quaternion.CreateFromYawPitchRoll(AngleY, AngleX, 0);
        }
        #endregion

        #region Camera Movement
        /// <summary>
        /// Move camera forward its view direction
        /// </summary>
        /// <param name="m">Distance of movement, positive means moving forward,
        /// negative - moving backward</param>
        public void MoveForward(float m)
        {
            //Position += (m) * new Vector3(View.M13,View.M23, View.M33);
            Position += m * Vector3.Normalize(Target);
            UpdateView();
        }

        /// <summary>
        /// Strafe camera - move sideways
        /// </summary>
        /// <param name="m">Distance of movement, positive means moving right,
        /// negative - moving left</param>
        public void MoveRight(float m)
        {
            Position += m * Right;
            UpdateView();
        }

        /// <summary>
        /// Move camera up along its up vector, eg. if camera is looking down,
        /// it will move up and slightly forward
        /// </summary>
        /// <param name="m">Distance of movement, positive means moving up,
        /// negative - moving down</param>
        public void MoveUp(float m)
        {
            //Position += m * new Vector3(View.M21, View.M22, View.M32);
            Position += Up * m;
            UpdateView();
        }

        /// <summary>
        /// Translate camera regadless of phi and theta
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector3 translation)
        {
            Position += translation;
        }
        #endregion
        public float AngleX { get; set; }
        public float AngleY { get; set; }

        #region Camera Rotation
        /// <summary>
        /// Rotate around X axis. It is analogous to looking up and down.
        /// Maximum rotation angles are PI/2 and -PI/2 (camera can look
        /// straight up and straight down, but not any further)
        /// </summary>
        /// <param name="angle">Angle of rotation (in radians). Positive
        /// angle means looking up, negative - looking down</param>
        public void RotateX(float angle)
        {
            theta += angle;
            AngleX += angle;
            // Clamp to (-PI/2, PI/2)
            //if (theta > (float)(Math.PI / 2))
            //{
            //    //theta = (float)(Math.PI / 2) - 0.01f;
            //    theta -= angle;
            //    AngleX = 0;
            //};
            //if (theta < -(float)(Math.PI / 2))
            //{
            //    //theta = -(float)(Math.PI / 2) + 0.01f;
            //    theta -= angle;
            //    AngleX = 0;
            //}
            UpdateView();
        }

        /// <summary>
        /// Rotate camera around Y axis. It is analogous to looking
        /// left and right.
        /// </summary>
        /// <param name="angle">Angle of rotation (in radians). Positive
        /// angle means turning left, negative - turning right.</param>
        public void RotateY(float angle)
        {
            phi += angle;
            AngleY += angle;

            //// Clamp angle to [0, 2*PI]
            //if (phi >= (float)(Math.PI * 2))
            //{
            //    //phi -= (float)(Math.PI * 2);
            //    phi -= angle;
            //    AngleY = 0;
            //}
            //if (phi < 0.0f)
            //{
            //    //phi += (float)(Math.PI * 2);
            //    phi -= angle;
            //    AngleY = 0;
            //}
            UpdateView();
        }
        #endregion

        #region Fields
        private float phi;
        private float theta;
        #endregion

        #region Utility
        /// <summary>
        /// Calculates phi rotation angle
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static float CalculatePhi(Vector3 dir)
        {
            // special case #1: dir parallel to X-axis
            if (dir.Z == 0.0f)
                return dir.X >= 0.0f ? 0.0f : (float)Math.PI;

            // special case #2: dir parallel to Z-axis
            if (dir.X == 0.0f)
                return dir.Z >= 0.0f ? (float)(Math.PI / 2) : (float)(Math.PI / 2) + (float)Math.PI;

            if (dir.Z > 0.0f)
            {
                if (dir.X > 0.0f)
                    // first quarter of coordinate system
                    return (float)Math.Atan2(dir.Z, dir.X);

                // second quarter of coordinate system
                // using -dir.X instead of Math.Abs(dir.X)
                return (float)Math.Atan2(-dir.X, dir.Z) + (float)(Math.PI / 2);
            }

            // dir.Y < 0.0f
            if (dir.X < 0.0f)
                // third quarter of coordinate system
                // using negation instead of Math.Abs()
                return (float)Math.PI + (float)Math.Atan2(Math.Abs(dir.Z), Math.Abs(dir.X));

            // fourth quarter of coordinate system
            // using -dir.Z instead of Math.Abs(dir.Z)
            return (float)(Math.PI / 2) + (float)Math.PI + (float)Math.Atan2(dir.X, -dir.Z);
        }
        #endregion

        public Matrix4x4 GetProjectionMatrix(float aspectRatio)
        {
            // This math is identical to what you find documented for
            // D3DXMatrixOrthoRH with the exception that in WPF only
            // the camera's width is specified.  Height is calculated
            // from width and the aspect ratio.
            float fov = FieldOfView.ToRadians();
            float f = 1 / (float)Math.Tan(fov / 2.0F);
            float aspect = aspectRatio;
            float zn = 1.0f; 
            float zf = 800.0f;
            float m33 = (zf + zn) / (zn - zf);
            float m43 = (2.0F * zf * zn) / (zn - zf);

            return new Matrix4x4(
                f / aspect, 0, 0, 0,
                0, f, 0, 0,
                0, 0, m33, m43,
                0, 0, -1, 0
                );
        }
    }
}
