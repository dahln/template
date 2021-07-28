using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ghostlight.Server.Data;
using ghostlight.Server.Entities;
using ghostlight.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ghostlight.Server.Models;
using ghostlight.Server.Utility;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ghostlight.Shared.Enumerations;

namespace ghostlight.Server.Controllers
{
    [ApiController]
    public class DataController : ControllerBase
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("api/v1/folder/{folderId}/customer")]
        [Authorize]
        [CanFolderWrite]
        async public Task<IActionResult> CustomerCreate([FromBody] Shared.Customer model, string folderId)
        {
            string userId = User.GetUserId();

            if (string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Customer name is required");
            }

            ghostlight.Server.Entities.Customer customer = new ghostlight.Server.Entities.Customer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Postal = model.Postal,
                Notes = model.Notes,
                ImageBase64 = model.ImageBase64,
                FolderId = folderId
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            Shared.Customer response = customer.ToSharedCustomer();
            //Not necessary at this time. Not files are automatically uploaded as part of customer creation.
            //response.Files = await _db.CustomerFiles.Where(c => c.Customer.FolderId == folderId && c.CustomerId == customerId).Select(f => new ghostlight.Shared.CustomerFile() { Id = f.Id, Name = f.PrettyFileName, Uploaded = f.Uploaded }).ToListAsync();

            return Ok(response);
        }

        [HttpGet]
        [Route("api/v1/folder/{folderId}/customer/{customerId}")]
        [Authorize]
        [CanFolderRead]
        async public Task<IActionResult> CustomerGetById(string folderId, string customerId)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.FolderId == folderId && c.Id == customerId).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            Shared.Customer response = customer.ToSharedCustomer();
            response.Files = await _db.CustomerFiles.Where(c => c.Customer.FolderId == folderId && c.CustomerId == customerId).Select(f => new ghostlight.Shared.CustomerFile() { Id = f.Id, Name = f.PrettyFileName, Uploaded = f.Uploaded }).ToListAsync();

            return Ok(response);
        }

        [HttpPut]
        [Route("api/v1/folder/{folderId}/customer/{customerId}")]
        [Authorize]
        [CanFolderWrite]
        async public Task<IActionResult> CustomerUpdateById([FromBody] Shared.Customer model, string folderId, string customerId)
        {
            string userId = User.GetUserId();

            if (string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Customer name is required");
            }

            var customer = await _db.Customers.Where(c => c.FolderId == folderId && c.Id == customerId).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            customer.Name = model.Name;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.City = model.City;
            customer.State = model.State;
            customer.Postal = model.Postal;
            customer.Notes = model.Notes;
            customer.ImageBase64 = model.ImageBase64;
            customer.UpdateOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            Shared.Customer response = customer.ToSharedCustomer();
            response.Files = await _db.CustomerFiles.Where(c => c.Customer.FolderId == folderId && c.CustomerId == customerId).Select(f => new ghostlight.Shared.CustomerFile() { Id = f.Id, Name = f.PrettyFileName, Uploaded = f.Uploaded }).ToListAsync();

            return Ok(response);
        }

        [HttpDelete]
        [Route("api/v1/folder/{folderId}/customer/{customerId}")]
        [Authorize]
        [CanFolderDelete]
        async public Task<IActionResult> CustomerDeleteById(string folderId, string customerId)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.FolderId == folderId && c.Id == customerId).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            return Ok();
        }


        [Authorize]
        [HttpPost]
        [Route("api/v1/folder/{folderId}/customers")]
        [CanFolderRead]
        async public Task<IActionResult> CustomerSearch([FromBody] Search model, string folderId)
        {
            string userId = User.GetUserId();

            var query = _db.Customers.Where(c => c.FolderId == folderId);

            if (!string.IsNullOrEmpty(model.FilterText))
            {
                query = query.Where(i => i.Name.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Email.ToLower().ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Phone.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Address.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.State.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Postal.ToLower().Contains(model.FilterText.ToLower()) ||
                                        i.Notes.ToLower().Contains(model.FilterText.ToLower()));
            }

            if (model.SortBy == nameof(Entities.Customer.Name))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Name)
                            : query.OrderByDescending(c => c.Name);
            }
            else if (model.SortBy == nameof(Entities.Customer.State))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.State)
                            : query.OrderByDescending(c => c.State);
            }
            else if (model.SortBy == nameof(Entities.Customer.City))
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.State)
                            : query.OrderByDescending(c => c.State);
            }
            else
            {
                query = model.SortDirection == SortDirection.Ascending
                            ? query.OrderBy(c => c.Name)
                            : query.OrderByDescending(c => c.Name);
            }


            SearchResponse<Shared.CustomerSlim> response = new SearchResponse<Shared.CustomerSlim>();
            response.Total = await query.CountAsync();

            var dataResponse = await query.Skip(model.Page * model.PageSize)
                                        .Take(model.PageSize)
                                        .ToListAsync();

            response.Data = dataResponse.Select(i => i.ToSharedCustomerSlim()).ToList();

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/folder")]
        async public Task<IActionResult> CreateFolder([FromBody] ghostlight.Shared.Folder model)
        {
            string userEmail = User.GetUserEmail();

            Entities.Folder folder = new Entities.Folder()
            {
                Name = model.Name,
                AuthorizedUsers = new List<Entities.AuthorizedUser>()
                {
                    new Entities.AuthorizedUser()
                    {
                        Email = userEmail,
                        Administrator = true,
                        Write = true,
                        Read = true,
                        Delete = true
                    }
                }
            };
            _db.Folders.Add(folder);
            await _db.SaveChangesAsync();

            return Ok(folder.ToSharedFolder());
        }

        [HttpPut]
        [Authorize]
        [Route("api/v1/folder/{folderId}")]
        [IsFolderAdministrator]
        async public Task<IActionResult> UpdateFolder([FromBody] ghostlight.Shared.Folder model, string folderId)
        {
            var folder = await _db.Folders.FirstOrDefaultAsync(o => o.Id == folderId);
            folder.Name = model.Name;

            await _db.SaveChangesAsync();

            return Ok(folder.ToSharedFolder());
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/folder/{folderId}")]
        [IsFolderAdministrator]
        async public Task<IActionResult> DeleteFolder(string folderId)
        {
            var deleteThis = _db.Folders.Where(o => o.Id == folderId);
            _db.Folders.RemoveRange(deleteThis);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/folder/{folderId}")]
        [IsFolderAdministrator]
        async public Task<IActionResult> GetFolder(string folderId)
        {
            var folder = await _db.Folders.Include(o => o.AuthorizedUsers).FirstOrDefaultAsync(o => o.Id == folderId);

            return Ok(folder.ToSharedFolder());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/authorized/folder")]
        async public Task<IActionResult> GetAuthorizedFolders(string folderId)
        {
            string userEmail = User.GetUserEmail();

            var folders = await _db.Folders
                                .Include(o => o.AuthorizedUsers)
                                .Where(o => o.AuthorizedUsers.Any(a => a.Email == userEmail &&
                                (
                                    a.Administrator || a.Write || a.Read || a.Delete
                                ))).ToListAsync();

            List<FolderAuthorization> response = new List<FolderAuthorization>();
            foreach (var folder in folders)
            {
                response.Add(new FolderAuthorization()
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    Administrator = folder.AuthorizedUsers.FirstOrDefault(a => a.Email == userEmail).Administrator,
                    Write = folder.AuthorizedUsers.FirstOrDefault(a => a.Email == userEmail).Write,
                    Read = folder.AuthorizedUsers.FirstOrDefault(a => a.Email == userEmail).Read,
                    Delete = folder.AuthorizedUsers.FirstOrDefault(a => a.Email == userEmail).Delete,
                });
            }

            return Ok(response);
        }



        [HttpPut]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderAddAuthorizedUser([FromBody] ghostlight.Shared.AuthorizedUser user, string folderId)
        {
            var authorizedUser = new Entities.AuthorizedUser()
            {
                Email = user.Email,
                FolderId = folderId
            };
            _db.AuthorizedUsers.Add(authorizedUser);
            await _db.SaveChangesAsync();

            return Ok(authorizedUser.ToSharedAuthorizedUser());
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized/{authorizedUserId}")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderDeleteAuthorizedUser(string authorizedUserId)
        {
            var deleteThis = _db.AuthorizedUsers.Where(a => a.Id == authorizedUserId);
            _db.AuthorizedUsers.RemoveRange(deleteThis);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized/{authorizedUserId}/admin")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderToggleAdmin(string folderId, string authorizedUserId)
        {
            var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Id == authorizedUserId);
            if (authorizedUser != null)
            {
                authorizedUser.Administrator = !authorizedUser.Administrator;
                await _db.SaveChangesAsync();
            }

            return Ok(authorizedUser.ToSharedAuthorizedUser());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized/{authorizedUserId}/write")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderToggleWrite(string folderId, string authorizedUserId)
        {
            var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Id == authorizedUserId);
            if (authorizedUser != null)
            {
                authorizedUser.Write = !authorizedUser.Write;
                await _db.SaveChangesAsync();
            }

            return Ok(authorizedUser.ToSharedAuthorizedUser());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized/{authorizedUserId}/read")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderToggleRead(string folderId, string authorizedUserId)
        {
            var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Id == authorizedUserId);
            if (authorizedUser != null)
            {
                authorizedUser.Read = !authorizedUser.Read;
                await _db.SaveChangesAsync();
            }

            return Ok(authorizedUser.ToSharedAuthorizedUser());
        }

        [HttpGet]
        [Authorize]
        [Route("api/v1/folder/{folderId}/authorized/{authorizedUserId}/delete")]
        [IsFolderAdministrator]
        async public Task<IActionResult> FolderToggleDelete(string folderId, string authorizedUserId)
        {
            var authorizedUser = await _db.AuthorizedUsers.FirstOrDefaultAsync(a => a.FolderId == folderId && a.Id == authorizedUserId);
            if (authorizedUser != null)
            {
                authorizedUser.Delete = !authorizedUser.Delete;
                await _db.SaveChangesAsync();
            }

            return Ok(authorizedUser.ToSharedAuthorizedUser());
        }

        [HttpPost]
        [Route("api/v1/folder/{folderId}/customer/{customerId}/file")]
        [Authorize]
        [CanFolderWrite]
        async public Task<IActionResult> UploadFile(string folderId, string customerId, [FromBody] Shared.UploadFile model)
        {
            string userId = User.GetUserId();

            var customer = await _db.Customers.Where(c => c.FolderId == folderId && c.Id == customerId).FirstOrDefaultAsync();
            if (customer == null)
                return BadRequest("Customer not found");

            try
            {
                Entities.CustomerFile fileData = new Entities.CustomerFile()
                {
                    PrettyFileName = model.FileName,
                    FileType = model.MimeType,
                    CustomerId = customerId,
                    Data = model.Data
                };

                _db.CustomerFiles.Add(fileData);
                await _db.SaveChangesAsync();

                return Ok(fileData.Id);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("api/v1/folder/{folderId}/customer/{customerId}/file/{fileId}")]
        [Authorize]
        [CanFolderRead]
        async public Task<IActionResult> DownloadFileByCompanyIdFileId(string folderId, string customerId, string fileId)
        {
            var file = await _db.CustomerFiles.FirstOrDefaultAsync(f => f.Customer.FolderId == folderId && f.CustomerId == customerId && f.Id == fileId);

            return Ok(new Shared.UploadFile()
            {
                Data = file.Data,
                FileName = file.PrettyFileName,
                MimeType = file.FileType
            });
        }

        [HttpDelete]
        [Route("api/v1/folder/{folderId}/customer/{customerId}/file/{fileId}")]
        [Authorize]
        [CanFolderWrite]
        async public Task<IActionResult> DeleteFileByCompanyIdFileId(string folderId, string customerId, string fileId)
        {
            var file = _db.CustomerFiles.Where(f => f.Customer.FolderId == folderId && f.CustomerId == customerId && f.Id == fileId);

            _db.CustomerFiles.RemoveRange(file);
            await _db.SaveChangesAsync();

            return Ok();
        }

    }//End Controller
}//End Namespace