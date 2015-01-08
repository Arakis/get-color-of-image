/*******************************************************************************************************

  Copyright (C) Sebastian Loncar, Web: http://loncar.de
  Project: https://github.com/Arakis/get-color-of-image

  MIT License:

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
  associated documentation files (the "Software"), to deal in the Software without restriction, 
  including without limitation the rights to use, copy, modify, merge, publish, distribute,
  sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial
  portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
  NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
  OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*******************************************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace GetColorOfImage
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var fileName = "";
            var showColorDialog = false;

            if (args.Length == 0 || args[0] == "/?" || args[0] == "--help")
            {
                Console.WriteLine("This application calculates the averagaging color of an image.");
                Console.WriteLine("GetColorOfImage.exe [--showcolordialog] filename");
                Console.WriteLine("GetColorOfImage.exe --help");
                return;
            }

            if (args.Length == 1)
            {
                fileName = args[0];
            }
            else if (args.Length == 2)
            {
                if (args[0] == "--showcolordialog")
                    showColorDialog = true;
                fileName = args[1];
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                return;
            }

            var image = new Bitmap(fileName);

            long sumColorR = 0;
            long sumColorG = 0;
            long sumColorB = 0;

            //counting the sums
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    sumColorR += pixel.R;
                    sumColorG += pixel.G;
                    sumColorB += pixel.B;
                }
            }

            var pixelLength = image.Width * image.Height;

            //calculate the average
            double avgColorR = sumColorR / pixelLength;
            double avgColorG = sumColorG / pixelLength;
            double avgColorB = sumColorB / pixelLength;

            var avgColor = Color.FromArgb((int)Math.Round(avgColorR), (int)Math.Round(avgColorG), (int)Math.Round(avgColorB));

            //convert to 1.0-range
            avgColorR /= 255;
            avgColorG /= 255;
            avgColorB /= 255;

            var hsl = RGBtoHSL(avgColorR, avgColorG, avgColorR);

            var H = hsl.Item1;
            var S = hsl.Item2;
            var L = hsl.Item3;

            Console.WriteLine("RGB => R: {0}, G: {1}, B: {2}", avgColor.R, avgColor.G, avgColor.B);
            Console.WriteLine("HSL => H: {0}, S: {1}, L: {2}", H, S, L);

            if (showColorDialog)
            {
                var form = new Form();
                form.BackColor = avgColor;
                form.KeyDown += (sender, e) => Environment.Exit(0);
                form.FormClosed += (sender, e) => Environment.Exit(0);
                Console.WriteLine("Press any key to continue...");
                form.ShowDialog();
            }
        }

        public static Tuple<double, double, double> RGBtoHSL(double r, double g, double b)
        {
            double max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Max(g, b));
            double h = 0, s = 0, l = (max + min) / 2;

            if (max == min)
            {
                h = s = 0; // achromatic
            }
            else
            {
                var d = max - min;
                s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
                if (r == max)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (g == max)
                {
                    h = (b - r) / d + 2;
                }
                else if (b == max)
                {
                    h = (r - g) / d + 4;
                }

                h = h / 6;
            }

            return new Tuple<double, double, double>(h, s, l);
        }

    }

}
