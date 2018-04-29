using System;
using System.Drawing;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Desktop graphics class
    /// </summary>
    public class DesktopGraphics : IDisposable
    {
        /// <summary>
        /// Device context
        /// </summary>
        private IntPtr deviceContext = IntPtr.Zero;

        /// <summary>
        /// Graphics
        /// </summary>
        private Graphics graphics;

        /// <summary>
        /// Graphics
        /// </summary>
        public Graphics Graphics
        {
            get => graphics;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DesktopGraphics()
        {
            deviceContext = User32.GetDC(IntPtr.Zero);
            if (deviceContext != IntPtr.Zero)
            {
                graphics = Graphics.FromHdc(deviceContext);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (graphics != null)
            {
                graphics.Dispose();
            }
            if (deviceContext != IntPtr.Zero)
            {
                User32.ReleaseDC(IntPtr.Zero, deviceContext);
            }
        }
    }
}
