using System;
using System.Collections.Generic;

namespace Corekit.Extensions
{
    public static class MathExtensions
    {
        public static double Clamp(double value, double min, double max) => Math.Min(Math.Max(value, min), max);
        public static float Clamp(float value, float min, float max) => Math.Min(Math.Max(value, min), max);
        public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);
        public static uint Clamp(uint value, uint min, uint max) => Math.Min(Math.Max(value, min), max);
    }
}
