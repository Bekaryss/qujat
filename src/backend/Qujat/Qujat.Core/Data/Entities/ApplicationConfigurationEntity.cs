using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qujat.Core.Data.Entities
{
    public enum EmailSenderProvider
    {
        GmailSmtp,
        SendGrid
    }

    public class ApplicationConfigurationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public EmailSenderProvider EmailSenderProviderType { get; set; }
    }
}
