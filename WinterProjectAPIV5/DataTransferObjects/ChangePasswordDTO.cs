﻿namespace WinterProjectAPIV5.DataTransferObjects;

public class ChangePasswordDTO
{
    public int UserID { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}