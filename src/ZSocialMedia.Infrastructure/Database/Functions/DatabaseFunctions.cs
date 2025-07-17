using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSocialMedia.Infrastructure.Database.Functions
{
    /// <summary>
    /// Database functions that can be used in LINQ queries
    /// These map to PostgreSQL functions we created in init-db.sql
    /// </summary>
    public static class DatabaseFunctions
    {
        /// <summary>
        /// Normalize text for case-insensitive, accent-insensitive search
        /// Maps to normalize_text() PostgreSQL function
        /// </summary>
        public static string NormalizeText(string input) => throw new NotSupportedException();
    }
}
