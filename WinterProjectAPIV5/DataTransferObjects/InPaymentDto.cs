namespace WinterProjectAPIV5.DataTransferObjects
{
    public class InPaymentDto
    {
        public int? TransactionID { get; set; }
        public int? UserGroupId { get; set; }

        public double? Amount { get; set; }
        public DateTime? DatePaid { get; set; }
    }
}
