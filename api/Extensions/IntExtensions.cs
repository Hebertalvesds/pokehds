namespace api.Extensions
{
    static class IntExtensions
    {
        public static bool ToBool(this int number)
        {
            return number == 0;
        }
    }
}
