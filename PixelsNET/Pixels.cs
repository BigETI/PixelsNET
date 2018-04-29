using NAudio.Wave;
using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Text;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Pixels class
    /// </summary>
    public class Pixels : IDisposable
    {
        /// <summary>
        /// Scripts
        /// </summary>
        private Dictionary<string, JavascriptContext> scripts;

        /// <summary>
        /// Sprites
        /// </summary>
        private Dictionary<string, Image> sprites;

        /// <summary>
        /// Sounds
        /// </summary>
        private Dictionary<string, WaveOut> sounds;

        /// <summary>
        /// Serializer
        /// </summary>
        private static readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PixelsDataContract));

        /// <summary>
        /// Sprite
        /// </summary>
        public Image Sprite
        {
            get
            {
                // TODO
                return null;
            }
        }

        /// <summary>
        /// Sprite
        /// </summary>
        public Point Position
        {
            get
            {
                // TODO
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scripts">Scripts</param>
        /// <param name="sprites">Sprites</param>
        /// <param name="sounds">Sounds</param>
        private Pixels(Dictionary<string, JavascriptContext> scripts, Dictionary<string, Image> sprites, Dictionary<string, WaveOut> sounds)
        {
            this.scripts = scripts;
            this.sprites = sprites;
            this.sounds = sounds;
        }

        /// <summary>
        /// Is number
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>"true" if "value" is a number, otherwise "false"</returns>
        private static bool IsNumber(object value)
        {
            return ((value is sbyte) || (value is byte) || (value is short) || (value is ushort) || (value is int) || (value is uint) || (value is long) || (value is ulong) || (value is float) || (value is double) || (value is decimal));
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="callback">Callback</param>
        /// <param name="args">Arguments</param>
        private void Invoke(JavascriptContext script, string callback, params object[] args)
        {
            StringBuilder str = new StringBuilder(callback);
            str.Append("(");
            foreach (object arg in args)
            {
                str.Append((arg == null) ? "null" : (IsNumber(arg) ? arg.ToString() : ("\"" + arg.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"")));
            }
            str.Append(")");
            script.Run(str.ToString());
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="callback">Callback</param>
        /// <param name="args">Arguments</param>
        public void Invoke(string script, string callback, params object[] args)
        {
            if (scripts.ContainsKey(script))
            {
                Invoke(scripts[script], callback, args);
            }
        }

        /// <summary>
        /// Invoke all
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="args">Arguments</param>
        public void InvokeAll(string callback, params object[] args)
        {
            foreach (JavascriptContext script in scripts.Values)
            {
                Invoke(script, callback, args);
            }
        }

        /// <summary>
        /// Load Pixels from file
        /// </summary>
        /// <param name="path">Path to pixels file</param>
        /// <returns>Pixels</returns>
        public static Pixels LoadFromFile(string path)
        {
            Pixels ret = null;
            try
            {
                if (path != null)
                {
                    if (File.Exists(path))
                    {
                        PixelsDataContract pixels_data = null;
                        using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
                        {
                            try
                            {
                                ZipArchiveEntry entry = archive.GetEntry("meta.json");
                                if (entry != null)
                                {
                                    using (Stream stream = entry.Open())
                                    {
                                        pixels_data = serializer.ReadObject(stream) as PixelsDataContract;
                                    }
                                }
                                if (pixels_data != null)
                                {
                                    Dictionary<string, JavascriptContext> scripts = new Dictionary<string, JavascriptContext>();
                                    Dictionary<string, Image> sprites = new Dictionary<string, Image>();
                                    Dictionary<string, WaveOut> sounds = new Dictionary<string, WaveOut>();
                                    foreach (PixelsAssetDataContract asset in pixels_data.Assets)
                                    {
                                        try
                                        {
                                            if (asset != null)
                                            {
                                                EPixelsAssetType pixels_asset_type;
                                                if (Enum.TryParse(asset.Type, out pixels_asset_type))
                                                {
                                                    entry = archive.GetEntry(asset.Entry);
                                                    if (entry != null)
                                                    {
                                                        switch (pixels_asset_type)
                                                        {
                                                            case EPixelsAssetType.Script:
                                                                using (StreamReader reader = new StreamReader(entry.Open()))
                                                                {
                                                                    JavascriptContext context = new JavascriptContext();
                                                                    context.SetParameter("pixels", new { path, entry = Path.GetFileNameWithoutExtension(asset.Entry) });
                                                                    context.Run(reader.ReadToEnd());
                                                                    scripts.Add(Path.GetFileNameWithoutExtension(asset.Entry), context);
                                                                }
                                                                break;
                                                            case EPixelsAssetType.Sprite:
                                                                using (Stream stream = entry.Open())
                                                                {
                                                                    Image image = Image.FromStream(stream);
                                                                    if (image != null)
                                                                    {
                                                                        sprites.Add(Path.GetFileNameWithoutExtension(asset.Entry), image);
                                                                    }
                                                                }
                                                                break;
                                                            case EPixelsAssetType.Sound:
                                                                using (StreamMediaFoundationReader reader = new StreamMediaFoundationReader(entry.Open()))
                                                                {
                                                                    WaveOut wave_out = new WaveOut(WaveCallbackInfo.FunctionCallback());
                                                                    wave_out.Init(reader);
                                                                    sounds.Add(Path.GetFileNameWithoutExtension(asset.Entry), wave_out);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Console.Error.WriteLine("Pixels asset type \"" + asset.Type + "\" is invalid.");
                                                }

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.Error.WriteLine(e.Message);
                                        }
                                    }
                                    ret = new Pixels(scripts, sprites, sounds);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return ret;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            foreach (JavascriptContext script in scripts.Values)
            {
                script.Dispose();
            }
            foreach (Image sprite in sprites.Values)
            {
                sprite.Dispose();
            }
            foreach (WaveOut sound in sounds.Values)
            {
                sound.Dispose();
            }
            scripts.Clear();
            sprites.Clear();
            sounds.Clear();
        }
    }
}
