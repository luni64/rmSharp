﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace rmSharp
{
    public partial class WitnessTable
    {
        public long WitnessId { get; set; }

        public long EventId { get; set; }

        public long PersonId { get; set; }

        public long WitnessOrder { get; set; }

        public long Role { get; set; }

        public string Sentence { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public string Given { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Prefix { get; set; } = string.Empty;

        public string Suffix { get; set; } = string.Empty;

        public DateTime ChangeDate { get; set; }
    }
}