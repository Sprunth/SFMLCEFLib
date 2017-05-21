﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp; 
using SFML.Graphics;
using SFML.Window;

namespace SFMLCEFLib
{
    public class CEFRenderer : Drawable, IDisposable
    {
        private readonly CefEngine cef;

        private bool mouseInWindowBounds = true;
        private Texture cefTexture;
        private readonly Sprite cefSprite;

        public CEFRenderer(RenderWindow window, string initialPageAddress,
            EventHandler<string> consoleMessageHandler = null, bool disableRightClick = true)
        {
            window.Closed += (sender, eventArgs) => { window.Close(); };

            window.MouseButtonPressed +=
                (sender, eventArgs) => { cef.MouseClick(eventArgs.X, eventArgs.Y, (int) eventArgs.Button, false); };
            window.MouseButtonReleased +=
                (sender, eventArgs) => { cef.MouseClick(eventArgs.X, eventArgs.Y, (int) eventArgs.Button, true); };
            window.MouseMoved +=
                (sender, eventArgs) => { cef.MouseMove(eventArgs.X, eventArgs.Y, !mouseInWindowBounds); };
            window.MouseWheelScrolled += (sender, eventArgs) =>
            {
                if (eventArgs.Wheel == Mouse.Wheel.HorizontalWheel)
                    cef.MouseWheel(eventArgs.X, eventArgs.Y, (int) Math.Round(eventArgs.Delta*10), 0);
                else
                    cef.MouseWheel(eventArgs.X, eventArgs.Y, 0, (int) Math.Round(eventArgs.Delta*10));
            };
            window.KeyPressed += (sender, args) =>
            {
                // todo: Map SFML key enums to https://github.com/adobe/webkit/blob/master/Source/WebCore/platform/chromium/KeyboardCodes.h
            };
            window.KeyReleased += (sender, args) =>
            {
                // todo: Map SFML key enums to https://github.com/adobe/webkit/blob/master/Source/WebCore/platform/chromium/KeyboardCodes.h
            };
            window.MouseLeft += (sender, eventArgs) => mouseInWindowBounds = false;
            window.MouseEntered += (sender, eventArgs) => mouseInWindowBounds = true;
            window.Resized += (sender, args) =>
            {
                window.SetView(new View(new FloatRect(0, 0, args.Width, args.Height)));
                cef.ChangeWindowSize(new Size((int) args.Width, (int) args.Height));
            };

            var consoleMsgHandler =
                new EventHandler<ConsoleMessageEventArgs>(
                    (sender, args) =>
                        consoleMessageHandler?.Invoke(null, $"[{args.Source}:{args.Line}] {args.Message}"));

            cef = new CefEngine(new Size((int) window.Size.X, (int) window.Size.Y), initialPageAddress,
                consoleMessageHandler != null ? consoleMsgHandler : null, disableRightClick);
            cefTexture = new Texture(BmpToByteArray(cef.LatestRender));
            cefSprite = new Sprite(cefTexture);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (cef.RenderDirty)
            {
                cefTexture.Dispose();
                cefTexture = new Texture(BmpToByteArray(cef.LatestRender));
                cefSprite.Texture = cefTexture;
            }
            target.Draw(cefSprite);
        }

        public void ToggleDevTools(bool open)
        {
            cef.ToggleDevTools(open);
        }

        private static byte[] BmpToByteArray(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            cef.Dispose();
            cefTexture.Dispose();
            cefSprite.Dispose();
        }
    }
}