using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.User
{
    public class RoleFeature : BaseEntity
    {
        public Role Role { get; set; }
        public Feature Feature { get; set; }
    }
}
