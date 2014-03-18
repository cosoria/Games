using System;
using SharpDX.Win32;

namespace SharpDX
{
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 vector, float rotationAngle)
        {
            double oldPlaneX = vector.X;
            double oldPlaneY = vector.Y;
            
            var x = (float)(oldPlaneX * Math.Cos(rotationAngle) - oldPlaneY * Math.Sin(rotationAngle));
            var y = (float)(oldPlaneX * Math.Sin(rotationAngle) + oldPlaneY * Math.Cos(rotationAngle));

            return new Vector2(x, y);
        }

        public static Vector2 Add(this Vector2 vector, Vector2 otherVector)
        {
            return Vector2.Add(vector, otherVector);
        }

        public static Vector2 Substract(this Vector2 vector, Vector2 otherVector)
        {
            return Vector2.Subtract(vector, otherVector);
        }

        public static Vector2 Multiply(this Vector2 vector, float factor)
        {
            return Vector2.Multiply(vector, factor);
        }

        public static double ToDegrees(this double radians)
        {
            return (360 * radians) / (2.0 * Math.PI);
        }

        public static double ToRadians(this double degrees)
        {
            return (degrees * (2.0 * Math.PI)) / 360.0;
        }
    }
}