using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models
{
    public class RefreshTokenModel
    {
        [Column("RefreshTokenId")]
        public Guid Id { get; set; }

        [ForeignKey(nameof(UserModel))]
        public Guid UserId { get; set; }
        public UserModel? User { get; set; }

        public string? Token { get; set; }
        public string? JwtId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }


    }
}
