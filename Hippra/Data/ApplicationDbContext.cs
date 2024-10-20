﻿using System;
using System.Collections.Generic;
using System.Text;
using Hippra.Models.SQL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hippra.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //means db has table named Cases
        public DbSet<Case> Cases { get; set; }

        public DbSet<CaseComment> CaseComments { get; set; }

        public DbSet<HistoryLog> HistoryLogs { get; set; }

        public DbSet<CaseCommentVote> CaseCommentVotes { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Follow> Follows { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<MedicalSubCategory> MedicalSubCategories { get; set; }
        public DbSet<CaseCommentFile> CaseCommentFiles { get; set; }
        
        public DbSet<CaseLike> CaseLikes { get; set; }
        public DbSet<CaseCommentReport> CaseCommentReports { get; set; }
        public DbSet<CaseFile> CaseFiles { get; set; }

    }
}
