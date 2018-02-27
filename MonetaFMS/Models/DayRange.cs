using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public enum DayRange
    {
        DueIn14,
        DueIn29,
        DueIn44,
        DueIn89,
        DueIn90Plus,
        Overdue
    }

    public static class DayRangeExtensions
    {
        public static string ToFriendlyString(this DayRange range)
        {
            switch (range)
            {
                case DayRange.DueIn14:
                    return "Next 14 Days";
                case DayRange.DueIn29:
                    return "15-29 Days";
                case DayRange.DueIn44:
                    return "30-44 Days";
                case DayRange.DueIn89:
                    return "45-89 Days";
                case DayRange.DueIn90Plus:
                    return "90+ Days";
                case DayRange.Overdue:
                    return "Overdue";
                default:
                    return range.ToString();
            }
        }
    }
}
