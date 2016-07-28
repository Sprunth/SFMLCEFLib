using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;

namespace SFMLCEFLib
{
    internal sealed class CefEngine : IDisposable
    {
        private readonly ChromiumWebBrowser browser;
        private Bitmap _latestBitmap;

        public bool RenderDirty { get; private set; }

        public Bitmap LatestRender
        {
            get
            {
                RenderDirty = false;
                return _latestBitmap;
            }
            set
            {
                RenderDirty = true;
                _latestBitmap = value;
            }
        }

        public CefEngine(Size browserSize, EventHandler<ConsoleMessageEventArgs> consoleMsgHandler = null,
            bool disableRightClick = true)
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");

            Cef.Initialize(settings, true, true);

            browser = new ChromiumWebBrowser() {Size = browserSize};
            if (consoleMsgHandler != null)
                browser.ConsoleMessage += consoleMsgHandler;

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            if (disableRightClick)
                browser.MenuHandler = new InactiveMenuHandler();
            browser.NewScreenshot += (sender, args) => LatestRender = browser.Bitmap;

            LatestRender = null;

            while (RenderDirty == false || LatestRender == null)
            {
                Thread.Yield();
            }
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // this function is called twice, once on load start, one when finished
            if (e.IsLoading)
                return;

            // don't want more snapshots
            browser.LoadingStateChanged -= Browser_LoadingStateChanged;

            LatestRender = browser.Bitmap;
        }

        public void Run(string pageAddress)
        {
            while (!browser.IsBrowserInitialized)
                Thread.Yield();

            browser.Load(pageAddress);
        }

        #region emulate user actions

        public void MouseClick(int x, int y, int type, bool mouseUp)
        {
            browser.GetBrowser()
                .GetHost()
                .SendMouseClickEvent(x, y, IntToMouseButtonType(type), mouseUp, 1, CefEventFlags.None);
        }

        public void MouseMove(int x, int y, bool mouseLeave)
        {
            browser.GetBrowser().GetHost().SendMouseMoveEvent(x, y, mouseLeave, CefEventFlags.None);
        }

        public void MouseWheel(int x, int y, int deltaX, int deltaY)
        {
            browser.GetBrowser().GetHost().SendMouseWheelEvent(x, y, deltaX, deltaY, CefEventFlags.None);
        }

        public void KeyPress(bool down, int winKeyCode)
        {
            var evnt = new KeyEvent()
            {
                Type = down ? KeyEventType.KeyDown : KeyEventType.KeyUp,
                WindowsKeyCode = winKeyCode
            };
            browser.GetBrowser().GetHost().SendKeyEvent(evnt);
        }

        public void ChangeWindowSize(Size newSize)
        {
            browser.Size = newSize;
            Debug.WriteLine($"CEF Size changed: {browser.Size}");
        }

        private MouseButtonType IntToMouseButtonType(int type)
        {
            MouseButtonType mouseType;
            switch (type)
            {
                case 0:
                    mouseType = MouseButtonType.Left;
                    break;
                case 2:
                    mouseType = MouseButtonType.Middle;
                    break;
                case 1:
                    mouseType = MouseButtonType.Right;
                    break;
                default:
                    // todo: handle this better
                    mouseType = MouseButtonType.Left;
                    break;
            }
            return mouseType;
        }

        #endregion

        public void Dispose()
        {
            browser.Dispose();
            Cef.Shutdown();
        }
    }

    internal class InactiveMenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters,
            IMenuModel model)
        {
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters,
            CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return true;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IContextMenuParams parameters,
            IMenuModel model, IRunContextMenuCallback callback)
        {
            return true;
        }
    }
}