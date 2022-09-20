using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Tools
{
    public static class PinyinHelpers
    {

        private static Encoding gb2312 = Encoding.GetEncoding("GB2312");


        private static string GetSpell(char chr)
        {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);
            return coverchr;
        }

        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string ConvertToAllSpell(string strChinese)
        {
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr));
                    }
                    return fullSpell.ToString().ToUpper();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("出错！" + e.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// 汉字转首字母
        /// </summary>
        /// <param name="strChinese"></param>
        /// <returns></returns>
        public static string GetFirstSpell(string strChinese)
        {
            try
            {
                if (strChinese.Length != 0)
                {
                    StringBuilder fullSpell = new StringBuilder();
                    for (int i = 0; i < strChinese.Length; i++)
                    {
                        var chr = strChinese[i];
                        fullSpell.Append(GetSpell(chr)[0]);
                    }
                    return fullSpell.ToString().ToUpper();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("出错！" + e.Message);
            }

            return string.Empty;
        }

        public static String GetPYstring(string str)
        {
            string temStr = "";
            foreach (char c in str)
            {
                if ((int)c > 33 && (int)c <= 126)
                {
                    temStr += c.ToString();
                }
                else
                {
                    temStr += GetPYChar(c.ToString());
                }
            }
            return temStr;
        }

        public static String GetPYChar(string c)
        {
            try
            {
                if (c.Equals(" "))
                {
                    return c;
                }
                if (c.Equals("行"))
                {
                    return "x";
                }
                byte[] array = new byte[2];
                array = Encoding.Default.GetBytes(c);
                int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
                if (i < 0xB0A1) return "*";
                if (i < 0xB0C5) return "a";
                if (i < 0xB2C1) return "b";
                if (i < 0xB4EE) return "c";
                if (i < 0xB6EA) return "d";
                if (i < 0xB7A2) return "e";
                if (i < 0xB8C1) return "f";
                if (i < 0xB9FE) return "g";
                if (i < 0xBBF7) return "h";

                if (i < 0xBFA6) return "j";
                if (i < 0xC0AC) return "k";
                if (i < 0xC2E8) return "l";
                if (i < 0xC4C3) return "m";
                if (i < 0xC5B6) return "n";
                if (i < 0xC5BE) return "o";
                if (i < 0xC6DA) return "p";
                if (i < 0xC8BB) return "q";
                if (i < 0xC8F6) return "r";
                if (i < 0xCBFA) return "s";
                if (i < 0xCDDA) return "t";

                if (i < 0xCEF4) return "w";
                if (i < 0xD1B9) return "x";
                if (i < 0xD4D1) return "y";
                if (i < 0xD7FA) return "z";
                return c;
            }
            catch (Exception)
            {
                return c;
            }
        }

        /// <summary>
        /// 判断文本是否全是字母组合
        /// </summary>
        /// <param name="text">需判断的文本或是字符串</param>
        /// <returns>返回true代表有字母存在</returns>
        public static bool IsAllChar(string text)

        {
            foreach (char tempchar in text.ToCharArray())
            {
                if (!(tempchar >= 'A' && tempchar <= 'z'))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
