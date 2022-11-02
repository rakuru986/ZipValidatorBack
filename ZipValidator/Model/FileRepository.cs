using System.IO.Compression;

namespace ZipValidator.Model
{
    public class FileRepository : IFileRepository
    {
        public FileModel ValidateZip(FileModel fileModel)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "zip", fileModel.FileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Read))
                        foreach (ZipArchiveEntry entry in zip.Entries)
                            if (entry.Name == "CatGame")
                                entry.ExtractToFile("myfile");

                    fileModel.FromFile.CopyTo(stream);
                }

                return fileModel;
            }
            catch (Exception)
            {
                return fileModel;
            }
        }
    }

    public interface IFileRepository
    {
        FileModel ValidateZip(FileModel fileModel);
       
    }

}
