using System.Runtime.InteropServices;

namespace Wahren.Analysis.Map
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ChipData
    {
        public static readonly ChipData Empty = new ChipData("________________");
        public ChipData(string a_name, uint a_x = 0, uint a_y = 0, uint a_w = 0, uint a_h = 0, int a_z = 0, int a_zIndex = 0)
        {
            name = string.Intern(a_name);
            x = a_x;
            y = a_y;
            width = a_w;
            height = a_h;
            zOrder = a_z;
            zIndex = a_zIndex;
        }
        public ChipData(in ChipData data)
        {
            name = data.name;
            x = data.x;
            y = data.y;
            width = data.width;
            height = data.height;
            zOrder = data.zOrder;
            zIndex = data.zIndex;
        }
        public string name;
        public uint x;
        public uint y;
        public uint width;
        public uint height;
        public int zOrder;
        public int zIndex;
        public (uint left, uint top, uint width, uint height) GetRect()
            => zOrder == 1 ? (x - (width >> 1), y + 1 - height, width, height) : (x, y, width, height);
        public (uint left, uint top, uint width, uint height) GetRect(uint borderX, uint borderY)
        {
            uint sx, sy, dx, dy;
            if (zOrder == 1)
            {
                long tsx = x - (long)(width >> 1);
                if (x < width >> 1)
                    sx = 0;
                else sx = x - (width >> 1);
                if (y + 1 < height)
                    sy = 0;
                else sy = y + 1 - height;
                dx = sx + width;
                dy = sy + height;
            }
            else
            {
                sx = x;
                sy = y;
                dx = x + width;
                dy = y + height;
            }

            sx = (sx > borderX) ? borderX : sx;
            sy = (sy > borderY) ? borderY : sy;
            dx = (dx > borderX) ? borderX : dx;
            dy = (dy > borderY) ? borderY : dy;
            return (sx, sy, dx - sx, dy - sy);
        }
    }
}