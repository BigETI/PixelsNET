using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Pixels engine class
    /// </summary>
    public static class PixelsEngine
    {
        /// <summary>
        /// Pixels
        /// </summary>
        private static List<Pixels> pixels = new List<Pixels>();

        /// <summary>
        /// Sleep time in milliseconds
        /// </summary>
        private static readonly int sleepTime = 10;

        /// <summary>
        /// Load Pixels
        /// </summary>
        /// <param name="path">Pixels path</param>
        private static void LoadPixels(string path)
        {
            if (path != null)
            {
                if (File.Exists(path))
                {
                    Pixels p = Pixels.LoadFromFile(path);
                    if (p != null)
                    {
                        pixels.Add(p);
                        p.InvokeAll("onInit");
                    }
                }
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public static void Start(string[] paths)
        {
            DesktopGraphics desktop_graphics = new DesktopGraphics();
            try
            {
                if (Singleton.StartAsSingleton(paths))
                {
                    if (paths != null)
                    {
                        foreach (string path in paths)
                        {
                            LoadPixels(path);
                        }
                    }
                    while (pixels.Count > 0)
                    {
                        string[] command_line_arguments = Singleton.FetchCommandLineArguments;
                        Graphics graphics = desktop_graphics.Graphics;
                        if (command_line_arguments != null)
                        {
                            foreach (string command_line_argument in command_line_arguments)
                            {
                                LoadPixels(command_line_argument);
                            }
                        }
                        foreach (Pixels p in pixels)
                        {
                            p.InvokeAll("onUpdate");
                            Image sprite = p.Sprite;
                            if ((graphics != null) && (sprite != null))
                            {
                                graphics.DrawImage(sprite, p.Position);
                            }
                        }
                        Thread.Sleep(sleepTime);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                foreach (Pixels p in pixels)
                {
                    p.Dispose();
                }
                desktop_graphics.Dispose();
                Singleton.CleanUp();
            }
        }
    }
}
