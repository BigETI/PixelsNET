using System;
using System.Runtime.InteropServices;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// User32 class
    /// </summary>
    public static class User32
    {
        /// <summary>
        /// Get device context
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        /// <returns>Device context</returns>
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>
        /// Release device context
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        /// <param name="dc">Device context</param>
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
    }
}
