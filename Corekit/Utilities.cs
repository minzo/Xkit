using System;
using System.Collections.Generic;
using System.Text;

namespace CoreKit
{
    public static class Utilities
    {
        /// <summary>
        /// RGBからHSVの色を取得します
        /// </summary>
        /// <param name="r">Red [0,255]</param>
        /// <param name="g">Green [0,255]</param>
        /// <param name="b">Blue [0,255]</param>
        /// <returns>h=[0,360) s=[0,255] v=[0,255] </returns>
        public static (double h, double s, double v) GetHSVFromRGB(byte r, byte g, byte b)
        {
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double range = max - min;

            double h = 0.0, s = 0.0, v = 0.0;

            if (max != 0 && range > 0)
            {
                if (max == r)
                    h = 60 * (g - b) / range;
                if (max == g)
                    h = 60 * (b - r) / range + 120;
                if (max == b)
                    h = 60 * (r - g) / range + 240;

                if (h < 0)
                    h += 360;

                s = (1 - min / max) * 255;
                v = max;
            }

            return (h, s, v);
        }

        /// <summary>
        /// HSVからRGBの色を取得します
        /// </summary>
        /// <param name="h">Hue [0,360)</param>
        /// <param name="s">Saturation [0,255]</param>
        /// <param name="v">Value [0,255]</param>
        /// <returns></returns>
        public static (byte r, byte g, byte b) GetRGBFromHSV(double h, double s, double v)
        {
            double max = v;
            double min = max - s / 255 * max;
            double r = 0.0, g = 0.0, b = 0.0;

            if (h <= 60.0)
            {
                r = max;
                g = (h - 0.0) / 60.0 * (max - min) + min;
                b = min;
            }
            else if (h <= 120)
            {
                r = (120.0 - h) / 60.0 * (max - min) + min;
                g = max;
                b = min;
            }
            else if (h <= 180)
            {
                r = min;
                g = max;
                b = (h - 120) / 60 * (max - min) + min;
            }
            else if (h <= 240)
            {
                r = min;
                g = (240 - h) / 60 * (max - min) + min;
                b = max;
            }
            else if (h <= 300)
            {
                r = (h - 240) / 60 * (max - min) + min;
                g = min;
                b = max;
            }
            else if (h <= 360)
            {
                r = max;
                g = min;
                b = (360 - h) / 60 + (max - min) + min;
            }


            return ((byte)r, (byte)g, (byte)b);
        }
    }
}
