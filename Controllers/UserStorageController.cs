using System.Reflection;
using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Admin;
using Vandelay.Industries.Permissions;
using Vandelay.Industries.Services;
using Vandelay.Industries.ViewModels;

namespace Vandelay.Industries.Controllers {
    [OrchardFeature("Vandelay.UserStorage")]
    [Admin]
    public class UserStorageController : Controller {
        private readonly IUserStorageService _userStorageService;

        public UserStorageController(IOrchardServices services, IUserStorageService userStorageService) {
            Services = services;
            _userStorageService = userStorageService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public ActionResult Index(string userName = null, string folder = null, string fileName = null) {
            if (!Services.Authorizer.Authorize(UserStoragePermissions.ManageUserStorage, T("Not allowed to manage user storage")))
                return new HttpUnauthorizedResult();

            var users = _userStorageService.GetUsers();
            var model = new UserStorageAdminViewModel {
                UserNames = users,
                UserName = userName,
                Folder = folder,
                FileName = fileName
            };
            if (!string.IsNullOrWhiteSpace(userName)) {
                model.Folders = _userStorageService.GetFolders(userName);
            }
            if (!string.IsNullOrWhiteSpace(folder)) {
                model.FileNames = _userStorageService.GetFiles(folder, userName);
            }
            if (!string.IsNullOrWhiteSpace(fileName)) {
                model.Contents = _userStorageService.Load(folder, fileName, userName);
            }
            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("submit.Save")]
        public ActionResult IndexSave() {
            if (!Services.Authorizer.Authorize(UserStoragePermissions.ManageUserStorage, T("Not allowed to manage user storage")))
                return new HttpUnauthorizedResult();

            var model = new UserStorageAdminViewModel();
            if (!TryUpdateModel(model)) {
                return Index(model.UserName, model.Folder, model.FileName);
            }
            _userStorageService.Save(model.Folder, model.FileName, model.Contents, model.UserName);
            return RedirectToAction("Index", new {model.UserName, model.Folder, model.FileName});
        }

        [HttpPost, ActionName("Index")]
        [FormValueRequired("submit.Delete")]
        public ActionResult IndexDelete() {
            if (!Services.Authorizer.Authorize(UserStoragePermissions.ManageUserStorage, T("Not allowed to manage user storage")))
                return new HttpUnauthorizedResult();

            var model = new UserStorageAdminViewModel();
            if (!TryUpdateModel(model)) {
                return Index(model.UserName, model.Folder, model.FileName);
            }
            _userStorageService.Delete(model.Folder, model.FileName, model.UserName);
            return RedirectToAction("Index");
        }

        public class FormValueRequiredAttribute : ActionMethodSelectorAttribute {
            private readonly string _submitButtonName;

            public FormValueRequiredAttribute(string submitButtonName) {
                _submitButtonName = submitButtonName;
            }

            public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
                var value = controllerContext.HttpContext.Request.Form[_submitButtonName];
                return !string.IsNullOrEmpty(value);
            }
        }
    }
}