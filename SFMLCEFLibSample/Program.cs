using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFMLCEFLib;

namespace SFMLCEFLibSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(900, 600), "SFMLCEFLibSample", Styles.Default);
            var cefRenderer = new CEFRenderer(window, @"http://msn.com");
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();
                window.Draw(cefRenderer);
                window.Display();
            }
        }
    }
}
