using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product_Config_Customer_v0.DomainManagement.Entity
{
    [Table("Anonymous_Request_Control")]
    public class AnonymousRequestControl
    {
        public int Id { get; set; }
        public string DomainName { get; set; }
        public bool AllowAnonymousRequest { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
