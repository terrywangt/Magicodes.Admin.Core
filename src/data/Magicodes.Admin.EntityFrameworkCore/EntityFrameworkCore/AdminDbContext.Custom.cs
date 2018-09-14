using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Admin.Core.Custom.Attachments;
using Magicodes.Admin.Core.Custom.Authorization;
using Magicodes.Admin.Core.Custom.Contents;
using Magicodes.Admin.Core.Custom.LogInfos;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public partial class AdminDbContext
    {
        public virtual DbSet<AttachmentInfo> AttachmentInfos { get; set; }

        public virtual DbSet<ObjectAttachmentInfo> ObjectAttachmentInfos { get; set; }

        public virtual DbSet<ArticleInfo> ArticleInfos { get; set; }

        public virtual DbSet<ArticleSourceInfo> ArticleSourceInfos { get; set; }

        public virtual DbSet<ArticleTagInfo> ArticleTagInfos { get; set; }

        public virtual DbSet<ColumnInfo> ColumnInfos { get; set; }

        public virtual DbSet<AppUserOpenId> AppUserOpenIds { get; set; }

        public virtual DbSet<TransactionLog> TransactionLogs { get; set; }
    }
}
