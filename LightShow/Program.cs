using System;
using System.Collections.Generic;
using System.Drawing;

namespace LightShow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting\n");
            using Lights lights = new Lights(150);

            Console.WriteLine("Select a pattern:");
            Console.WriteLine("1: Fade");
            Console.WriteLine("2: Rainbow");
            Console.WriteLine("3: Static colors");
            Console.WriteLine("4: Color marquee");

            var input = Console.ReadLine();

            if (input == "1")
            {
                var colors = GetColorsFromUser();
                Console.WriteLine("Enter time");
                int time = int.Parse(Console.ReadLine());
                lights.Fade(time, colors.ToArray());
            }
            else if (input == "2")
            {
                lights.RainbowTransition();
            }
            else if (input == "3")
            {
                var colors = GetColorsFromUser();
                lights.StaticColors(colors.ToArray());
            }
            else if (input == "4")
            {
                var colors = GetColorsFromUser();
                Console.WriteLine("Enter time");
                int time = int.Parse(Console.ReadLine());
                lights.ColorMarquee(time, colors.ToArray());
            }

        }

        private static List<Color> GetColorsFromUser()
        {
            Console.WriteLine("Enter colors (R G B or name)");
            var colors = new List<Color>();
            string colorInput;
            while (!string.IsNullOrWhiteSpace(colorInput = Console.ReadLine()))
            {
                string[] colorValues = colorInput.Split();
                if (colorValues.Length == 3)
                {
                    colors.Add(Color.FromArgb(int.Parse(colorValues[0]), int.Parse(colorValues[1]), int.Parse(colorValues[2])));
                }
                else
                {
                    colors.Add(Color.FromName(string.Join(' ', colorValues)));
                }
            }
            return colors;
        }
    }
}
