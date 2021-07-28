using System.Linq;

namespace ghostlight.Server.Utility
{
    static public class ToSharedModels
    {
        static public Shared.Folder ToSharedFolder(this Entities.Folder model)
        {
            var folder = new Shared.Folder()
            {
                Id = model.Id,
                Name = model.Name,
                AuthorizedUsers = model.AuthorizedUsers.Select(a => a.ToSharedAuthorizedUser()).ToList()
            };

            return folder;
        }

        static public Shared.AuthorizedUser ToSharedAuthorizedUser(this Entities.AuthorizedUser model)
        {
            var authorizedUser = new Shared.AuthorizedUser()
            {
                Id = model.Id,
                Email = model.Email,
                Administrator = model.Administrator,
                Write = model.Write,
                Read = model.Read,
                Delete = model.Delete
            };

            return authorizedUser;
        }

        static public Shared.CustomerSlim ToSharedCustomerSlim(this Entities.Customer model)
        {
            var customer = new Shared.CustomerSlim()
            {
                Id = model.Id,
                Name = model.Name,
                Gender = model.Gender,
                State = model.State,
                Active = model.Active
            };

            return customer;
        }

        static public Shared.Customer ToSharedCustomer(this Entities.Customer model)
        {
            var customer = new Shared.Customer()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Postal = model.Postal,
                Notes = model.Notes,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                Active = model.Active,
                ImageBase64 = model.ImageBase64
            };

            return customer;
        }

    }
}
