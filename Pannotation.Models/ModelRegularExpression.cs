namespace Pannotation.Models
{
    public static class ModelRegularExpression
    {
        public const string REG_NOT_CONTAIN_SPACES = "(^(?!\\s+$).+)";
        public const string REG_MUST_NOT_CONTAIN_SPACES = "[\\p{L}\\p{Nd}-_]+";
        public const string REG_CANT_START_FROM_SPACES = "^\\S.*$";
        public const string REG_NOT_CONTAIN_SPACES_WITH_LINE_BREAKS = "^(\\s*\\S+\\s*)+$";

        public const string REG_ONE_LATER_DIGIT = "^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9_-]+)";
        public const string REG_ONE_LATER_DIGIT_WITH_SPEC = "^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9\\s\\S_-]+)";
        public const string REG_ONE_LATER_DIGIT_CAPITAL_WITH_SPEC = "^(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[A-Z])([a-zA-Z0-9\\s\\S_-]+)";

        public const string REG_EMAIL = "([a-zA-Z0-9]+([+=#-._][a-zA-Z0-9]+)*@([a-zA-Z0-9]+(-[a-zA-Z0-9]+)*)+(([.][a-zA-Z0-9]{2,4})*)?)";
        public const string REG_EMAIL_DOMAINS = "(^.{1,64}@.{1,64}$)";
        public const string REG_ALPHANUMERIC = "^[a-zA-Z0-9 ]*$";
        public const string REG_ALPHANUMERIC_PUNCTUATION = "^[a-zA-Z0-9 .-]*$";
        public const string REG_NUMERIC = "^[0-9]*$";
        public const string REG_PHONE = @"^\+[0-9]+$";
        public const string REG_URL = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
        public const string REG_YOUTUBE_LINK = @"^((?:https?:)\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$";
        public const string REG_EXPIRATION_DATE = @"^((0[1-9])|(1[0-2]))/((0[1-9])|([1-9]\d))$";
        public const string REG_ALPHANUMERIC_WITHOUT_SPACES = "^[a-zA-Z0-9]*$";
    }
}
