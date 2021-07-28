using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ghostlight.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ghostlight.Server.Utility
{
    public class IsFolderAdministratorAttribute : ServiceFilterAttribute
    {
        public IsFolderAdministratorAttribute() : base(typeof(IsFolderAdministratorFilter)) { }
    }

    public class IsFolderAdministratorFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _db;
        public IsFolderAdministratorFilter(ApplicationDbContext db)
        {
            _db = db;
        }

        async public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string folderIdKey = "folderId";

            bool folderIdExists = context.RouteData.Values.ContainsKey(folderIdKey);
            if (folderIdExists)
            {
                var folderId = context.RouteData.Values[folderIdKey] as string;

                var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email);
                var email = claim.Value;

                var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Email == email);

                if (authorizedUser == null)
                    context.Result = new UnauthorizedResult();
                else if (authorizedUser.Administrator == false)
                    context.Result = new UnauthorizedResult();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }



    //Write
    public class CanFolderWriteAttribute : ServiceFilterAttribute
    {
        public CanFolderWriteAttribute() : base(typeof(CanFolderWriteFilter)) { }
    }

    public class CanFolderWriteFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _db;
        public CanFolderWriteFilter(ApplicationDbContext db)
        {
            _db = db;
        }

        async public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string folderIdKey = "folderId";

            bool folderIdExists = context.RouteData.Values.ContainsKey(folderIdKey);
            if (folderIdExists)
            {
                var folderId = context.RouteData.Values[folderIdKey] as string;

                var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email);
                var email = claim.Value;

                var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Email == email);

                if (authorizedUser == null)
                    context.Result = new UnauthorizedResult();
                else if (authorizedUser.Write == false)
                    context.Result = new UnauthorizedResult();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }



    //Read
    public class CanFolderReadAttribute : ServiceFilterAttribute
    {
        public CanFolderReadAttribute() : base(typeof(CanFolderReadFilter)) { }
    }

    public class CanFolderReadFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _db;
        public CanFolderReadFilter(ApplicationDbContext db)
        {
            _db = db;
        }

        async public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string folderIdKey = "folderId";

            bool folderIdExists = context.RouteData.Values.ContainsKey(folderIdKey);
            if (folderIdExists)
            {
                var folderId = context.RouteData.Values[folderIdKey] as string;

                var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email);
                var email = claim.Value;

                var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Email == email);

                if (authorizedUser == null)
                    context.Result = new UnauthorizedResult();
                else if (authorizedUser.Read == false)
                    context.Result = new UnauthorizedResult();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }


    //Delete
    public class CanFolderDeleteAttribute : ServiceFilterAttribute
    {
        public CanFolderDeleteAttribute() : base(typeof(CanFolderDeleteFilter)) { }
    }

    public class CanFolderDeleteFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _db;
        public CanFolderDeleteFilter(ApplicationDbContext db)
        {
            _db = db;
        }

        async public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string folderIdKey = "folderId";

            bool folderIdExists = context.RouteData.Values.ContainsKey(folderIdKey);
            if (folderIdExists)
            {
                var folderId = context.RouteData.Values[folderIdKey] as string;

                var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email);
                var email = claim.Value;

                var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Email == email);

                if (authorizedUser == null)
                    context.Result = new UnauthorizedResult();
                else if (authorizedUser.Delete == false)
                    context.Result = new UnauthorizedResult();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
