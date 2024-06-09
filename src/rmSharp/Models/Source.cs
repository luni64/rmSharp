﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace rmSharp
{
    public partial class Source
    {
        public long SourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RefNumber { get; set; } = string.Empty;
        public string ActualText { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public long IsPrivate { get; set; }
        public long TemplateId { get; set; }
        public byte[] Fields { get; set; } = [];
        public DateTime ChangeDate { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; } = [];
        public virtual ICollection<Citation> Citations { get; set; } = [];
        public virtual ICollection<Medium> Media { get; set; } = [];
        public virtual ICollection<SourceWebTag> WebTags { get; set; } = [];

        public override string ToString() => Name;
    }
}