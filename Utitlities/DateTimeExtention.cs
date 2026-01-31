using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrugStockWeb.Utitlities
{
    public static class DateTimeExtention
    {
        public static string ToPersianString(this DateTime datetime, PersianDateTimeFormat format = PersianDateTimeFormat.ShortDateShortTime)
        {
            return new PersianDateTime(datetime).ToString(format).GetPersianNumber();
        }
        public static string ToShamsi(this DateTime datetime, PersianDateTimeFormat format = PersianDateTimeFormat.ShortDateShortTime)
        {
            return new PersianDateTime(datetime).ToString(format);
        }
        public static DateTime ToMiladi(this string datetime)
        {
            PersianCalendar pc = new PersianCalendar();
            string[] persianDateTime = datetime.Split(new []{"/"}, StringSplitOptions.None);
            DateTime dt = new DateTime(int.Parse(persianDateTime[0]), int.Parse(persianDateTime[1]), int.Parse(persianDateTime[2]), pc);
            return dt;
        }
        public static string ToPersianTimeString(this DateTime datetime)
        {
            return new PersianDateTime(datetime).ToString("hh:mm:ss tt").GetPersianNumber();
        }
        public static string ToPersianString(this DateTime? datetime, PersianDateTimeFormat format)
        {
            return datetime != null ? new PersianDateTime(datetime.Value).ToString(format).GetPersianNumber() : string.Empty;
        }
        public static string ToPersianTimeString(this DateTime? datetime)
        {
            return datetime != null ? new PersianDateTime(datetime.Value).ToString("hh:mm:ss tt").GetPersianNumber() : string.Empty;
        }
     
    }
}
