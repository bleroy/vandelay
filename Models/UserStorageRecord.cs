using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Models {
    [OrchardFeature("Vandelay.UserStorage")]
    public class UserStorageRecord {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Folder { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Contents { get; set; }
        public virtual int Size { get; set; }
    }
}