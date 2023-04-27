using System;
using System.Text;

namespace ArcDock.Tools
{
    /// <summary>
    /// 文字拼音转换工具类
    /// </summary>
    public static class PinyinHelpers
    {

        /// <summary>
        /// 获取完整拼音
        /// </summary>
        /// <param name="chr">文字</param>
        /// <returns>拼音字符串</returns>
        private static string GetSpell(char chr)
        {
            var coverchr = NPinyin.Pinyin.GetPinyin(chr);
            return coverchr;
        }

        /// <summary>
        /// 汉字转全拼
        /// </summary>
        /// <param name="strChinese">汉字字符串</param>
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
        /// <param name="strChinese">汉字字符串</param>
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
