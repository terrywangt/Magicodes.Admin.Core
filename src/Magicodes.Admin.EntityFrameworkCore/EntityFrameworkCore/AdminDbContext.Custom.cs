using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.App.Core.Attachments;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public partial class AdminDbContext
    {
        public virtual DbSet<AttachmentInfo> AttachmentInfos { get; set; }
    }
}
