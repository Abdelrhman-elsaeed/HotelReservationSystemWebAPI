using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Guest
{
    public class UpdateGuestDto
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string MobileNumber { get; set; }
    }
}
