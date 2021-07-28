using ghostlight.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ghostlight.Shared.Enumerations;

namespace ghostlight.Server.Entities
{
    public class Folder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        public List<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
    }

    public class AuthorizedUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public bool Administrator { get; set; }

        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }

        public string FolderId { get; set; }
        public Folder Folder { get; set; }
    }
    public class Customer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public DateTime BirthDate { get; set; }
        public string Notes { get; set; }

        public Gender Gender { get; set; }
        public bool Active { get; set; }

        public string ImageBase64 { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateOn { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<CustomerFile> Files { get; set; } = new List<CustomerFile>();

        public string FolderId { get; set; }
        public Folder Folder { get; set; }
    }

    public class CustomerFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Uploaded { get; set; } = DateTime.UtcNow;

        public string PrettyFileName { get; set; }
        public string FileType { get; set; }

        public byte[] Data { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
