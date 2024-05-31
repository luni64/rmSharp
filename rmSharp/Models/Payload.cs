﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace rmSharp
{
    public partial class Payload
    {
        public long RecId { get; set; }
        public long RecType { get; set; }
        public long OwnerType { get; set; }
        public long OwnerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }

        public override string ToString() => Title;
    }

    public class PayloadGroup2 : Payload
    {

        virtual public ICollection<GroupEntry> Entries { get; set; } = [];



    } // ownertype = 20
    public class PayloadSearch : Payload { } // ownertype = 8
}
