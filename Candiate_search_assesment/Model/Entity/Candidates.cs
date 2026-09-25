using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Candiate_search_assesment.Model.Entity
{
    [Table("candidates")]
    public class Candidates
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("age")]
        public int Age { get; set; }

        [Column("gender")]
        public string Gender { get; set; } = string.Empty;

        [Column("location")]
        public string Location { get; set; } = string.Empty;

        [Column("education")]
        public string Education { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        [Column("refresh_token_hash")]
        public string? RefreshTokenHash { get; set; }

        [Column("refresh_token_expires_at")]
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}