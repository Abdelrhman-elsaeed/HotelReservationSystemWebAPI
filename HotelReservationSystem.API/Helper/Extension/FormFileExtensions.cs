using Application.DTOS.RoomPicture;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.API.Helper.Extension
{
    public static class FileMappingExtensions
    {
        public static List<FileUploadDto> ToFileUploadDtos(this List<IFormFile> files)
        {
            var dtoList = new List<FileUploadDto>();

            if (files == null || !files.Any())
                return dtoList;

            var allowedExtensions = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    if (!allowedExtensions.Contains(file.ContentType.ToLower()))
                    {
                        throw new Exception($"File {file.FileName} is not a valid image format.");
                    }

                    dtoList.Add(new FileUploadDto
                    {
                        //Convert to stream
                        Content = file.OpenReadStream(), 
                        FileName = file.FileName,
                        ContentType = file.ContentType
                    });
                }
            }

            return dtoList;
        }
    }

    public class UploadRoomPicturesVM
    {
        [Required(ErrorMessage = "Room ID is required.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "You must upload at least one picture.")]
        public List<IFormFile> Pictures { get; set; }
    }
}
