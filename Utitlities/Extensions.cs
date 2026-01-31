using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrugStockWeb.Utitlities
{
    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }
        public static string Uncur(this string str)
        {
            string tempStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ',')
                {
                    tempStr += str[i];
                }
            }
            return tempStr;
        }
        public static string Cur(this string str)
        {
            bool flag = false;
            if (str.Contains("-"))
            {
                str = str.Substring(1, str.Length - 1);
                flag = true;
            }
            string tempStr = "";
            int j = 0;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                j++;
                tempStr = (str[i] + tempStr);
                if (j % 3 == 0 && j != (str.Length))
                {
                    tempStr = (',' + tempStr);
                }
            }

            if (flag)
                tempStr = "-" + tempStr;
            return tempStr;
        }

    }
}
