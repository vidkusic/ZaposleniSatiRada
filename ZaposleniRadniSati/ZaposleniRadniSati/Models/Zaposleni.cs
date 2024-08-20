﻿namespace ZaposleniRadniSati.Models
{
    public class Zaposleni
    {
        public string Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StarTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string EntryNotes { get; set; }
        public DateTime? DeletedOn { get; set; }

        public double TotalHoursWorked => (EndTimeUtc - StarTimeUtc).TotalHours;
    }
}
