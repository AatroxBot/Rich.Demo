using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ace
{
    public static class AceUtils
    {
        /// <summary>
        /// 判断一个字符串是否全有数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static bool IsDigit(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsDigit(input[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 判断一个用户名是否包含非法字符。此方法仅判断是否包含非法字符，不包括长度判断。
        /// </summary>
        /// <param name="input"></param>
        /// <param name="illegalChar">包含的非法字符</param>
        /// <returns></returns>
        static bool IsLegalUserName(string input, out string illegalChar)
        {
            /*
             * 
             */

            //[^a-z]

            illegalChar = null;

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            Match match = Regex.Match(input, @"[^A-Za-z0-9_\.-]");
            if (match == null || match.Success == false)
                return true;

            illegalChar = match.Value;
            return false;
        }
        /// <summary>
        /// 判断一个字符串是否是邮箱地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static bool IsEmail(string input)
        {
            // /^[0-9A-Za-z][\.-_0-9A-Za-z]*@[0-9A-Za-z]+(\.[0-9A-Za-z]+)+$/

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            Match match = Regex.Match(input, @"^[0-9A-Za-z][\.-_0-9A-Za-z]*@[0-9A-Za-z]+(\.[0-9A-Za-z]+)+$");
            if (match != null && match.Success == true)
                return true;

            return false;
        }
    }
}
