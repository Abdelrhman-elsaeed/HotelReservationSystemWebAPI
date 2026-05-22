using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.RoleFeature
{
    public class AssignFeatureToRoleDto
    {
        public int? ID { get; set; }
        public Role Role { get; set; }
        public Feature Feature { get; set; }
    }
}
