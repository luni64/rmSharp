﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace rmSharp
{
    public partial class Medium
    {
        public long MediaId { get; set; }
        public long MediaType { get; set; }
        public string MediaPath { get; set; } = string.Empty;
        public string MediaFile { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public byte[] Thumbnail { get; set; } = [];
        public string Caption { get; set; } = string.Empty;
        public string RefNumber { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public long SortDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
    }
}