namespace Backend.Tools
{
    public static class TimeStampConvertor
    {
        public readonly static DateTime _dt1970;
        static TimeStampConvertor()
        {
            _dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime IntToDatetime(int unixSeconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(unixSeconds);
            // Unix timestamp is seconds past epoch
            return _dt1970.AddSeconds(unixSeconds).ToUniversalTime();
        }

        public static DateTime LongToDatetime(long unixMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(unixMilliseconds);
            return _dt1970.AddMilliseconds(unixMilliseconds).ToUniversalTime();
        }

        public static DateTime Diff(DateTime arg1, DateTime arg2)
        {
            return _dt1970 + (arg1 - arg2);
        }

        public static long DatetimeToLong(DateTime time)
        {
            var unix = time.Subtract(_dt1970);
            return Convert.ToInt64(unix.TotalMilliseconds);
        }


        public static string ToTextRange(TimeSpan timeSpan)
        {
            int weeks = timeSpan.Days / 7;
            if (timeSpan.TotalNanoseconds < 0) return "";
            if (weeks > 0)
            {
                var residue = weeks % 10;
                if (weeks == 1 || weeks > 20 && residue == 1)
                    return $" на {weeks} неделю";
                else if ((weeks < 10 || weeks > 20) && residue > 1 && residue < 5)
                    return $" на {weeks} недели";
                else
                    return $" на {weeks} недель";
            }
            else
            {
                if (timeSpan.Days > 0)
                {
                    var residue = timeSpan.Days % 10;
                    if (timeSpan.Days == 1 || timeSpan.Days > 20 && residue == 1)
                        return $" на {timeSpan.Days} день";
                    else if ((timeSpan.Days < 10 || timeSpan.Days > 20) && residue > 1 && residue < 5)
                        return $" на {timeSpan.Days} дня";
                    else
                        return $" на {timeSpan.Days} дней";
                }
                else
                {
                    if (timeSpan.Hours > 0)
                    {
                        var residue = timeSpan.Hours % 10;
                        if (timeSpan.Hours == 1 || timeSpan.Hours > 20 && residue == 1)
                            return $" на {timeSpan.Hours} час";
                        else if ((timeSpan.Hours < 10 || timeSpan.Hours > 20) && residue > 1 && residue < 5)
                            return $" на {timeSpan.Hours} часа";
                        else
                            return $" на {timeSpan.Hours} часов";
                    }
                    else
                    {
                        if (timeSpan.Minutes > 0)
                        {
                            var residue = timeSpan.Minutes % 10;
                            if (timeSpan.Minutes == 1 || timeSpan.Minutes > 20 && residue == 1)
                                return $" на {timeSpan.Minutes} минуту";
                            else if (residue > 1 && residue < 5)
                                return $" на {timeSpan.Minutes} минуты";
                            else
                                return $" на {timeSpan.Minutes} минут";
                        }
                        else
                            return "";
                    }
                }
            }
        }
    }
}