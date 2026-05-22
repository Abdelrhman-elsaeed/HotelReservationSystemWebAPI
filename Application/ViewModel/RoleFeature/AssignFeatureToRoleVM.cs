using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.RoleFeature
{
    public class AssignFeatureToRoleVM
    {
        [Required(ErrorMessage = "Role is required.")]
        public Role Role { get; set; }

        [Required(ErrorMessage = "Feature is required.")]
        public Feature Feature { get; set; }
    }
}
