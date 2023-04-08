

namespace Entities
{
    public class StringFormats
    {
        public const string MobileRegex = @"^([6-9]{1})([0-9]{9})$";
        public const string DD_MMM_YYYY_Regex = @"([0-3][0-9]) ([a-zA-Z]{3}) \d{4}$";
        public const string NumberOnly = @"\d*$";
    }
}
