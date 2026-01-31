using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugStockWeb.Utitlities
{
    public static class PersianExtensions
    {
        public static string GetPersianNumber(this string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            for (var i = 48; i < 58; i++)
            {
                data = data.Replace(Convert.ToChar(i), Convert.ToChar(1728 + i));
            }
            return data;
        }

        public static string GetEnglishNumber(this string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            for (var i = 1776; i < 1786; i++)
            {
                data = data.Replace(Convert.ToChar(i), Convert.ToChar(i - 1728));
            }
            return data;
        }
        public static string GetPersianNumber(this long data)
        {
            return data.ToString(CultureInfo.InvariantCulture).GetPersianNumber();
        }
        public static string GetPersianNumber(this double data)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", data).GetPersianNumber();
        }
        public static string GetPersianNumber(this int data)
        {
            return data.ToString(CultureInfo.InvariantCulture).GetPersianNumber();
        }
        public static string GetNumberThousandSeperated(this int data)
        {
            string str = data.ToString();
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
        public static string GetNumberThousandSeperated(this long data)
        {
            string str = data.ToString();
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

        public static string GetPersianNumber(this decimal data)
        {
            return data.ToString(CultureInfo.InvariantCulture).GetPersianNumber();
        }
        public static string GetPersianNumber(this byte data)
        {
            return data.ToString(CultureInfo.InvariantCulture).GetPersianNumber();
        }
        public static string ReversePlaque(this string data)
        {
            string plaque = data;
            if (plaque.Length > 10)
            {
                var ar = plaque.Split('-');
                Array.Reverse(ar);
                if (ar.Length == 4)
                {
                    plaque = ar[1]+"-"+ar[0] + "-" + ar[2] + "-" + ar[3];
                }
            }

            return plaque;
        }

    }
}
