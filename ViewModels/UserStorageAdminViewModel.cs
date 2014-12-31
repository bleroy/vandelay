using System.Collections.Generic;

namespace Vandelay.Industries.ViewModels {
    public class UserStorageAdminViewModel {
        public string UserName { get; set; }
        public IEnumerable<string> UserNames { get; set; }
        public string Folder { get; set; }
        public IEnumerable<string> Folders { get; set; }
        public string FileName { get; set; }
        public IEnumerable<string> FileNames { get; set; }
        public string Contents { get; set; }
    }
}