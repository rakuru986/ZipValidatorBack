using Ionic.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Linq;
using ZipValidator.Model;

namespace ZipValidator.Controllers
{




    public class FileController : ControllerBase
    {

        [Route("api/file")]
        [HttpPost]
        public ActionResult Post([FromForm] FileModel file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string fileName = file.FileName;
            int index = fileName.LastIndexOf(".");
            if (index >= 0)
                fileName = fileName.Substring(0, index); // or index + 1 to keep slash

            try
            {
                
                string path = Path.Combine(Directory.GetCurrentDirectory(), "temp", file.FileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FromFile.CopyTo(stream);
                    stream.Position = 0;

                    ZipFile zip = ZipFile.Read(stream);
                    string _path = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                    System.IO.DirectoryInfo di = new DirectoryInfo(_path);






                    if (zip.EntryFileNames.Contains(fileName + "/dlls/") && zip.EntryFileNames.Contains(fileName + "/images/") && zip.EntryFileNames.Contains(fileName + "/languages/"))
                    {
                        

                        bool containsRootFileDLL = false;
                        bool containsOneImagePngOrJpg = false;

                        bool containsRootFileLanguage = false;




                        foreach (ZipEntry e in zip)
                        {




                            if (e.FileName.StartsWith(fileName + "/dlls/"+fileName+".dll"))
                            {
                                containsRootFileDLL = true;

                            }
                            else if (e.FileName.StartsWith(fileName + "/images") && !e.FileName.EndsWith("/"))
                            {
                                if (e.FileName.EndsWith(".jpg") || e.FileName.EndsWith(".png"))
                                {
                                    containsOneImagePngOrJpg = true;

                                }
                                else if (!e.FileName.EndsWith(".jpg") || !e.FileName.EndsWith(".png"))
                                {
                                    return Ok("only .png or .jpg files in images");
                                }
                            }
                            else if (e.FileName.StartsWith(fileName + "/languages") && !e.FileName.EndsWith('/'))
                            {
                                if (e.FileName.StartsWith(fileName + "/languages/"+fileName+"_en.xml"))
                                {
                                    containsRootFileLanguage = true;
                                }
                                if (!e.FileName.EndsWith(".xml"))
                                {

                                    return Ok("only .xml files in images");
                                }

                                string languageCode;

                                languageCode = e.FileName.Replace(fileName + "/languages/" + fileName + '_', string.Empty);
                                languageCode = languageCode.Replace(".xml", string.Empty);

                                if (!e.FileName.EndsWith(".xml") || !e.FileName.StartsWith(fileName + "/languages/" + fileName + '_') || languageCode.Length > 2)
                                {

                                    return Ok("wrong language code");
                                }

                            }
                        }

                        string response= "";

                        if (!containsRootFileDLL)
                        {
                            
                            return Ok("no RootFolder.dll file in dlls folder");
                        }

                        if (!containsOneImagePngOrJpg)
                        {
                            return Ok("no .png or .jpg file in images");
                        }
                        if (!containsRootFileLanguage) 
                        {                            
                            return Ok("no RootFolder_en.xml in languages");
                        }





                        return Ok("zip validated");


                    }
                    else
                    {
                        return Ok("folder structure wrong");
                    }

                    stream.Close();
                    return StatusCode(StatusCodes.Status201Created);


                }

                
                

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Route("api/savefile")]
        [HttpPost]
        public ActionResult saveFile([FromForm] FileModel file)
        {

            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "zips", file.FileName);

                

                
                    var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    file.FromFile.CopyTo(fileStream);
                    fileStream.Dispose();



                return Ok("zip saved");
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

        }


        [Route("api/deletefiles")]
        [HttpPost]
        public ActionResult DeleteFiles()
        {

            try 
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "zips");

                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete(); 
                }

                return Ok("zips deleted");
            }

            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            
        }
    }



}

