#pragma warning disable 164

namespace MyBot
{
    public class Math
    {
        /// <summary>
        /// Fast calculation of the absolute value using bitwise hacking
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The absolute value of value</returns>
        public static int Abs(int value)
        {
            return (value + (value >> 31)) ^ (value >> 31);
        }

        /// <summary>
        /// Returns x raised to the power of y
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>x raised to the power of y</returns>
        public static double Pow(double x, double y)
        {
            return System.Math.Pow(x, y);
        }

        /// <summary>
        /// Returns wether or not value is a power of 2
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// A super-fast integer square-root
        /// Based on http://www.hackersdelight.org/hdcodetxt/isqrt.c.txt
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static uint IntegerSquareRoot(uint x)
        {
            uint y, z;

            if (x < 1u << 16)
            {
                if (x < 1u << 08)
                {
                    if (x < 1u << 04)
                    {
                        return x < 1u << 02 ? x + 3u >> 2 : x + 15u >> 3;
                    }
                    if (x < 1u << 06)
                    {
                        y = 1u << 03;
                        x -= 1u << 04;
                        if (x >= 5u << 02) { x -= 5u << 02; y |= 1u << 02; }
                        goto L0;
                    }
                    y = 1u << 05;
                    x -= 1u << 06;
                    if (x >= 5u << 04) { x -= 5u << 04; y |= 1u << 04; }
                    goto L1;
                }
                if (x < 1u << 12)
                {
                    if (x < 1u << 10)
                    {
                        y = 1u << 07;
                        x -= 1u << 08;
                        if (x >= 5u << 06) { x -= 5u << 06; y |= 1u << 06; }
                        goto L2;
                    }
                    y = 1u << 09;
                    x -= 1u << 10;
                    if (x >= 5u << 08) { x -= 5u << 08; y |= 1u << 08; }
                    goto L3;
                }
                if (x < 1u << 14)
                {
                    y = 1u << 11;
                    x -= 1u << 12;
                    if (x >= 5u << 10) { x -= 5u << 10; y |= 1u << 10; }
                    goto L4;
                }
                y = 1u << 13;
                x -= 1u << 14;
                if (x >= 5u << 12) { x -= 5u << 12; y |= 1u << 12; }
                goto L5;
            }
            if (x < 1u << 24)
            {
                if (x < 1u << 20)
                {
                    if (x < 1u << 18)
                    {
                        y = 1u << 15;
                        x -= 1u << 16;
                        if (x >= 5u << 14) { x -= 5u << 14; y |= 1u << 14; }
                        goto L6;
                    }
                    y = 1u << 17;
                    x -= 1u << 18;
                    if (x >= 5u << 16) { x -= 5u << 16; y |= 1u << 16; }
                    goto L7;
                }
                if (x < 1u << 22)
                {
                    y = 1u << 19;
                    x -= 1u << 20;
                    if (x >= 5u << 18) { x -= 5u << 18; y |= 1u << 18; }
                    goto L8;
                }
                y = 1u << 21;
                x -= 1u << 22;
                if (x >= 5u << 20) { x -= 5u << 20; y |= 1u << 20; }
                goto L9;
            }
            if (x < 1u << 28)
            {
                if (x < 1u << 26)
                {
                    y = 1u << 23;
                    x -= 1u << 24;
                    if (x >= 5u << 22) { x -= 5u << 22; y |= 1u << 22; }
                    goto L10;
                }
                y = 1u << 25;
                x -= 1u << 26;
                if (x >= 5u << 24) { x -= 5u << 24; y |= 1u << 24; }
                goto L11;
            }
            if (x < 1u << 30)
            {
                y = 1u << 27;
                x -= 1u << 28;
                if (x >= 5u << 26) { x -= 5u << 26; y |= 1u << 26; }
                goto L12;
            }
            y = 1u << 29;
            x -= 1u << 30;
            if (x >= 5u << 28) { x -= 5u << 28; y |= 1u << 28; }

            L13: z = y | 1u << 26; y >>= 1; if (x >= z) { x -= z; y |= 1u << 26; }
            L12: z = y | 1u << 24; y >>= 1; if (x >= z) { x -= z; y |= 1u << 24; }
            L11: z = y | 1u << 22; y >>= 1; if (x >= z) { x -= z; y |= 1u << 22; }
            L10: z = y | 1u << 20; y >>= 1; if (x >= z) { x -= z; y |= 1u << 20; }
            L9: z = y | 1u << 18; y >>= 1; if (x >= z) { x -= z; y |= 1u << 18; }
            L8: z = y | 1u << 16; y >>= 1; if (x >= z) { x -= z; y |= 1u << 16; }
            L7: z = y | 1u << 14; y >>= 1; if (x >= z) { x -= z; y |= 1u << 14; }
            L6: z = y | 1u << 12; y >>= 1; if (x >= z) { x -= z; y |= 1u << 12; }
            L5: z = y | 1u << 10; y >>= 1; if (x >= z) { x -= z; y |= 1u << 10; }
            L4: z = y | 1u << 08; y >>= 1; if (x >= z) { x -= z; y |= 1u << 08; }
            L3: z = y | 1u << 06; y >>= 1; if (x >= z) { x -= z; y |= 1u << 06; }
            L2: z = y | 1u << 04; y >>= 1; if (x >= z) { x -= z; y |= 1u << 04; }
            L1: z = y | 1u << 02; y >>= 1; if (x >= z) { x -= z; y |= 1u << 02; }
            L0: return x > y ? (y >> 1) | 1u : (y >> 1);
        }
    }

}
