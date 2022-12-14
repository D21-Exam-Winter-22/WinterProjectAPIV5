using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WinterProjectAPIV5.Models;

public partial class InPayment
{
    public int TransactionId { get; set; }

    public int? UserGroupId { get; set; }

    public double? Amount { get; set; }

    public DateTime? DatePaid { get; set; }

    
    public virtual UserGroup? UserGroup { get; set; }
}
