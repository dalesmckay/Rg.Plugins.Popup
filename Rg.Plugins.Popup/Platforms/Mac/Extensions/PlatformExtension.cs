﻿using System.Linq;

using AppKit;

using Rg.Plugins.Popup.MacOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

using XFPlatform = Xamarin.Forms.Platform.MacOS.Platform;

namespace Rg.Plugins.Popup.MacOS.Extensions
{
    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = XFPlatform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = XFPlatform.CreateRenderer(bindable);
                XFPlatform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }

        public static void DisposeModelAndChildrenRenderers(this VisualElement view)
        {
            IVisualElementRenderer renderer;
            foreach (var child in view.Descendants().OfType<VisualElement>())
            {
                renderer = XFPlatform.GetRenderer(child);
                XFPlatform.SetRenderer(child, null);

                if (renderer == null)
                    continue;

                renderer.NativeView.RemoveFromSuperview();
                renderer.Dispose();
            }

            renderer = XFPlatform.GetRenderer(view);
            if (renderer != null)
            {
                renderer.NativeView.RemoveFromSuperview();
                renderer.Dispose();
            }
            XFPlatform.SetRenderer(view, null);
        }

        public static void UpdateSize(this PopupPageRenderer renderer)
        {
            var currentElement = renderer.CurrentElement;

            if (renderer.View?.Superview?.Frame == null || currentElement == null)
                return;

            var superviewFrame = renderer.View.Superview.Frame;

            renderer.SetElementSize(new Size(superviewFrame.Width, superviewFrame.Height));
        }
    }
}
