using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Drawing;
using System.Threading;
using Iot.Device.Graphics;
using Iot.Device.Ws28xx;

namespace LightShow
{
    public class Lights : IDisposable
    {
        private int LedCount { get; }
        private Ws2812b Device { get; }
        private SpiDevice Spi { get; }

        public Lights(int ledCount)
        {
            LedCount = ledCount;
            var settings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };
            Spi = SpiDevice.Create(settings);
            Device = new Ws2812b(Spi, LedCount);
        }

        public void ColorMarquee(int interval = 100, params Color[] colors)
        {
            BitmapImage image = Device.Image;
            image.Clear();
            try
            {
                while (!Console.KeyAvailable)
                {
                    for (int offset = 0; offset < colors.Length; offset++)
                    {
                        for (int i = 0; i < LedCount; i++)
                        {
                            image.SetPixel(i, 0, colors[(i + offset) % colors.Length]);
                        }

                        Device.Update();
                        Thread.Sleep(interval);
                    }
                }
            }
            finally
            {
                image.Clear();
                Device.Update();
            }
        }

        public void StaticColors(params Color[] colors)
        {
            BitmapImage image = Device.Image;
            image.Clear();
            try
            {
                for (int i = 0; i < LedCount; i++)
                {
                    image.SetPixel(i, 0, colors[i % colors.Length]);
                }

                Device.Update();
                Console.ReadKey();
            }
            finally
            {
                image.Clear();
                Device.Update();
            }
        }

        public void RainbowTransition(int time = 10)
        {
            BitmapImage image = Device.Image;
            image.Clear();
            int r = 255;
            int g = 0;
            int b = 0;
            try
            {
                while (!Console.KeyAvailable)
                {
                    if (r > 0 && b == 0)
                    {
                        r--;
                        g++;
                    }
                    if (g > 0 && r == 0)
                    {
                        g--;
                        b++;
                    }
                    if (b > 0 && g == 0)
                    {
                        r++;
                        b--;
                    }
                    image.Clear(Color.FromArgb(r, g, b));
                    Device.Update();
                    Thread.Sleep(time);
                }

            }
            finally
            {
                image.Clear();
                Device.Update();
            }
        }

        public void Fade(int time = 10, params Color[] colors)
        {
            BitmapImage image = Device.Image;
            image.Clear();
            try
            {
                while (!Console.KeyAvailable)
                {
                    foreach (var color in colors)
                    {
                        var gradient = new List<Color> { color };

                        while (gradient[gradient.Count - 1].R > 0
                            || gradient[gradient.Count - 1].G > 0
                            || gradient[gradient.Count - 1].B > 0)
                        {
                            gradient.Add(DimByFactor(gradient[gradient.Count - 1], .9));
                        }

                        for (int i = gradient.Count - 1; i >= 0; i--)
                        {
                            image.Clear(gradient[i]);
                            Device.Update();
                            Thread.Sleep(time);
                        }
                        for (int i = 0; i < gradient.Count; i++)
                        {
                            image.Clear(gradient[i]);
                            Device.Update();
                            Thread.Sleep(time);
                        }

                    }
                }
            }
            finally
            {
                image.Clear();
                Device.Update();
            }
        }

        private static Color DimByFactor(Color modifiedColor, double factor)
        {
            return Color.FromArgb((int)(modifiedColor.R * factor), (int)(modifiedColor.G * factor), (int)(modifiedColor.B * factor));
        }

        public void Dispose()
        {
            Spi.Dispose();
        }
    }
}
