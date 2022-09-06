namespace Addon.Models
{
    internal class MainCircuitBreakerModel
    {
        public int Voltage { get; set; }
        public string PoleType { get; set; }
        public int Current { get; set; }
        public string AicRating { get; set; }
        public string NemaType { get; set; }
        public string Material { get; set; }
        public string ProtectionType { get; set; }
        public int TagNumber { get; set; }
    }
}
