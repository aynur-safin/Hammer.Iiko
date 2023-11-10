namespace Hammer.Iiko.ReservePlugin.Dto
{
    public class PhoneDto
    {
        public PhoneDto(string phoneValue, bool isMain)
        {
            PhoneValue = phoneValue;
            IsMain = isMain;
        }

        public string PhoneValue { get; set; }
        public bool IsMain { get; set; }
    }
}
