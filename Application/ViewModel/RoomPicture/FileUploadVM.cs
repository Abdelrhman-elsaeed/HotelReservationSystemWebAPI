using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Application.ViewModel.RoomPicture
{
    public class FileUploadVM
    {
        [Required(ErrorMessage = "File content is required.")]
        public Stream Content { get; set; }

        [Required(ErrorMessage = "File name is required.")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Content type is required.")]
        [RegularExpression(@"^image\/(jpeg|png|gif|bmp|webp)$", ErrorMessage = "Only image formats (jpeg, png, gif, bmp, webp) are allowed.")]
        public string ContentType { get; set; }
    }
}
