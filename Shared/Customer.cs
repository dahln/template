using System;
using System.Collections.Generic;
using System.Text;
using ghostlight.Shared.Enumerations;

namespace ghostlight.Shared
{
    public class Folder
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();
    }

    public class AuthorizedUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool Administrator { get; set; }

        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
    }

    public class FolderAuthorization
    {
        /// <summary>
        /// Folder.Id
        /// </summary>
        public string Id { get; set; }

        public string Name { get; set; }
        public bool Administrator { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
    }
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
        public string ImageBase64 { get; set; }
        public List<CustomerFile> Files { get; set; } = new List<CustomerFile>();


        public override int GetHashCode()
        {
            //Tuple can't hold 13. Break properties into batchs
            var firstBatch = Tuple.Create(Id, Name, Email, Phone, Address, City, State).GetHashCode();
            var secondBatch = Tuple.Create(Postal, BirthDate.ToString("yyyy/mm/dd"), Notes, Gender, Active, ImageBase64).GetHashCode();

            return firstBatch + secondBatch;
        }
    }

    public class CustomerSlim
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
    }

    public class CustomerFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Uploaded { get; set; }
    }

    public class UploadFile
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
    }
}
