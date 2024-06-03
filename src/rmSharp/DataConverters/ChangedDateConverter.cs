using DelegateDecompiler;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace rmSharp
{
    public static partial class Extensions
    {
        static readonly DateTime epoch = new DateTime(1899, 12, 30);

        [Decompile]
        public static double toUTCModDate(this DateTime date)
        {
            return (date.ToUniversalTime() - epoch).TotalDays;
        }

        [Decompile]
        public static DateTime toDateTime(this double utcModDate)
        {
            return epoch.AddDays(utcModDate).ToLocalTime();
        }
    }

    public class ChangedDateConverter : ValueConverter<DateTime, double>
    {
        public ChangedDateConverter()
            : base(
                v => v.toUTCModDate(),
                v => v.toDateTime())
        { }
    }
}
