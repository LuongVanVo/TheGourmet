using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGourmet.Domain.Entities.Identity
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty; // ID của Access Token đi kèm 
        public bool IsUsed { get; set; } // Đã dùng chưa ?
        public bool IsRevoked { get; set; } // Đã bị thu hồi chưa ?
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; } // Thời gian hết hạn
        
        // Foreign Key
        public Guid UserId { get; set; }

        // Thiết lập quan hệ với ApplicationUser (quan hệ 1 - n)
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}