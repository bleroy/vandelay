using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface IUserStorageService : IDependency {
        string Load(string folder, string fileName, string userName = null);
        IDictionary<string, string> Load(string folder, IEnumerable<string> fileNames, string userName = null);
        void Save(string folder, string fileName, string value, string userName = null);
        IEnumerable<string> GetUsers();
        IEnumerable<string> GetFolders(string userName = null);
        IEnumerable<string> GetFiles(string folder, string userName = null);
        void Delete(string folder, string fileName, string userName = null);
    }

    [OrchardFeature("Vandelay.UserStorage")]
    public class UserStorageService : IUserStorageService {
        // Storage quota per user, in kB
        public const int Quota = 1000;

        private readonly IRepository<UserStorageRecord> _repository;
        private readonly IWorkContextAccessor _wca;

        public UserStorageService(IRepository<UserStorageRecord> repository, IWorkContextAccessor wca) {
            _repository = repository;
            _wca = wca;
        }

        public string Load(string folder, string fileName, string userName = null) {
            userName = EnsureUserName(userName);
            var record = _repository.Get(r => 
                r.UserName == userName &&
                r.Folder == folder && 
                r.FileName == fileName);
            if (record == null) return null;
            return record.Contents;
        }

        public IDictionary<string, string> Load(string folder, IEnumerable<string> fileNames, string userName = null) {
            userName = EnsureUserName(userName);
            return _repository
                .Table
                .Where(
                    f =>
                    f.Folder == folder &&
                    f.UserName == userName &&
                    fileNames.Contains(f.FileName))
                .ToDictionary(f => f.FileName, f => f.Contents);
        }

        private string EnsureUserName(string userName) {
            return userName ?? _wca.GetContext().CurrentUser.UserName;
        }

        public void Save(string folder, string fileName, string contents, string userName = null) {
            userName = EnsureUserName(userName);
            var currentTotalSize = _repository
                .Table
                .Where(r => r.UserName == userName)
                .Sum(r => r.Size);
            var record = _repository.Get(r =>
                r.UserName == userName &&
                r.Folder == folder &&
                r.FileName == fileName);
            if (record != null) {
                if (currentTotalSize - record.Size + contents.Length > Quota * 1000) {
                    throw new InvalidOperationException("Quota exceeded");
                }
                record.Contents = contents;
                record.Size = contents.Length;
                _repository.Update(record);
            }
            else {
                if (currentTotalSize + contents.Length > Quota * 1000) {
                    throw new InvalidOperationException("Quota exceeded");
                }
                _repository.Create(
                    new UserStorageRecord {
                        UserName = userName,
                        Folder = folder,
                        FileName = fileName,
                        Contents = contents,
                        Size = contents.Length
                    });
            }
        }

        public IEnumerable<string> GetUsers() {
            return _repository
                .Table
                .Select(r => r.UserName)
                .Distinct()
                .ToList();
        }

        public IEnumerable<string> GetFolders(string userName = null) {
            userName = EnsureUserName(userName);
            return _repository.Table
                .Where(r => r.UserName == userName)
                .Select(r => r.Folder)
                .Distinct()
                .ToList();
        }

        public IEnumerable<string> GetFiles(string folder, string userName = null) {
            userName = EnsureUserName(userName);
            return _repository.Table
                .Where(r => r.UserName == userName && r.Folder == folder)
                .Select(r => r.FileName)
                .Distinct()
                .ToList();
        }

        public void Delete(string folder, string fileName, string userName = null) {
            userName = EnsureUserName(userName);
            var record = _repository.Get(r =>
                r.UserName == userName &&
                r.Folder == folder &&
                r.FileName == fileName);
            if (record != null) {
                _repository.Delete(record);
            }
        }
    }
}