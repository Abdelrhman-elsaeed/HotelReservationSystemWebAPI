using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Facility
{
    public class DeleteFacilityDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
