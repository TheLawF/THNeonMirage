using System;

namespace THNeonMirage.Util
{
    public class UnicodeTable
    {
        public static readonly Range Controlls = 0x0..0x20;
        public static readonly Range HalfPunctrations = 0x21..0x2F;
        public static readonly Range Numbers = 0x30..0039;
        
        public static readonly Range BasicLatinAlphabets = 0x0..0x7E;
        public static readonly Range ASCII = 0x21..0x7E;

        public static readonly Range Characters = 0x4E00..0x9FFF;
    }
}