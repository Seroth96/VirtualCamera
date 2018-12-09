using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace VirtualCamera
{
    public static class Extensions
    {
        // <summary>
         /// Convert to Radians.
         /// </summary>
         /// <param name="val">The value to convert to radians</param>
         /// <returns>The value in radians</returns>
         public static float ToRadians(this double val)
         {
             return (float)((Math.PI / 180) * val);
         }
        
        public static Vector4 Multiply(this Vector4 vector, Matrix4x4 matrix)
        {
            Vector4 newVector4 = new Vector4();
            newVector4.X = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W;
            newVector4.Y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W;
            newVector4.Z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W;
            newVector4.W = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W;

            return newVector4;
        }

       
    }
}
