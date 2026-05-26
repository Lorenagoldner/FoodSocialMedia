namespace BlazorAppFood.Models
{
    public static class TimeFormatter
    {
        public static string FormatMinutes(int minutes)
        {
            if (minutes <= 0) return "—";
            if (minutes < 60) return $"{minutes} min";

            int h = minutes / 60;
            int m = minutes % 60;

            if (m == 0) return $"{h}h";
            return $"{h}h {m}min";
        }
    }
}
