using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.RoomPicture
{
    public class FileUploadDto
    {
        public Stream Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
