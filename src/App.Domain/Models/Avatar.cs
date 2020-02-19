using App.Domain.Core.Models;

namespace App.Domain.Models
{
    public class Avatar : EntityBase
    {
        public virtual byte[] File { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FilePathSettings { get; set; }
    }
}
