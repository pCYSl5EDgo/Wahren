using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wahren.PicEncoder
{
    public static class ImageChg
    {
        public static bool Encode(this ReadOnlySpan<byte> src, out byte[] dest)
        {
            var length = src.Length;
            if (length < 132 || (src[0] == (byte)'m' && src[1] == (byte)'y' && src[2] == (byte)'c' && src[3] == (byte)'g'))
            {
                dest = null;
                return false;
            }
            dest = new byte[length + 4];
            dest[0] = (byte)'m';
            dest[1] = (byte)'y';
            dest[2] = (byte)'c';
            dest[3] = (byte)'g';
            var destSpan = dest.AsSpan().Slice(4);
            src.Slice(100).CopyTo(destSpan);
            destSpan = destSpan.Slice(length - 100);
            src.Slice(0, 100).CopyTo(destSpan);
            return true;
        }
        public static bool Decode(this ReadOnlySpan<byte> src, out byte[] dest)
        {
            if (src.Length < 132 || src[0] != (byte)'m' || src[1] != (byte)'y' || src[2] != (byte)'c' || src[3] != (byte)'g')
            {
                dest = null;
                return false;
            }
            src = src.Slice(4);
            dest = new byte[src.Length];
            var destSpan = dest.AsSpan();
            var length = src.Length - 100;
            src.Slice(0, length).CopyTo(destSpan.Slice(100));
            src.Slice(length).CopyTo(destSpan);
            return true;
        }
    }
}