using System;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LoadLocally
{
    public class IconLoader 
    {
        private const uint ShgfiIcon = 0x000000100;    // get icon
        private const uint ShgfiLargeicon = 0x000000000;    // get large icon
        private const uint ShgfiUsefileattributes = 0x000000010; // use passed dwFileAttributes
        private const int FileAttributeNormal = 0x00000080;  // file attribute

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi, uint cbFileInfo, uint uFlags);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Shfileinfo
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public Sprite LoadIcon(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            Shfileinfo shinfo = new Shfileinfo();
            IntPtr hImgSmall = SHGetFileInfo(path, FileAttributeNormal, ref shinfo, (uint)Marshal.SizeOf(shinfo), ShgfiIcon | ShgfiLargeicon | ShgfiUsefileattributes);

            if (shinfo.hIcon != IntPtr.Zero)
            {
                try
                {
                    using var icon = Icon.FromHandle(shinfo.hIcon);
                    var bitmap = icon.ToBitmap();
                    bitmap.MakeTransparent();

                    // 转换Bitmap为Texture2D
                    var texture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            System.Drawing.Color sourceColor = bitmap.GetPixel(x, bitmap.Height - 1 - y); // 在此处修改y坐标
                            texture.SetPixel(x, y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                        }
                    }
                    texture.Apply();
                
                    // 创建一个Sprite并返回
                    Sprite sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(0, 0, 0, 0));
                    return sprite;

                }
                catch (Exception ex)
                {
                    Debug.LogError("无法加载图标: " + ex.Message);
                }
                finally
                {
                    DestroyIcon(shinfo.hIcon);
                }
            }
            else
            {
                Debug.LogError("无法从文件中提取图标: " + path);
            }

            return null;
        }
    }
}
