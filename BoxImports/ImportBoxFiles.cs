using System;
using System.Collections.Generic;
using System.Linq;
using Box.V2.Config;
using Box.V2.JWTAuth;
using Box.V2.Models;
using EBP.Business.Database;
using EBP.Business.Enums;
using EBP.Business.Repository;
using System.IO;
using System.Configuration;
using BoxApi.V2;

namespace EBP.BoxImports
{
    public class BoxSettings
    {
        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string EnterpriceId { get; set; }

        public string PrivateKey { get; set; }

        public string JwtPublicKeyId { get; set; }

        public string JwtPrivateKeyPassword { get; set; }
    }

    public class ImportBoxFiles
    {

        private BoxSettings Settings { get; set; }
        static BoxManager boxManager;
        public ImportBoxFiles(BoxSettings settings)
        {
            this.Settings = settings;
        }
      
        CareConnectCrmEntities DBEntity = new CareConnectCrmEntities(); 
        RepositoryEnrolledServices repositoryEnrolledService = new RepositoryEnrolledServices();
       
        public async void ImportFiles(string basePath)
        {
            try
            {
                // Get Box Auth Token
                if (!string.IsNullOrEmpty(Settings.PrivateKey))
                    Settings.PrivateKey = Settings.PrivateKey.Replace("\\n", "\n");

                var boxConfig = new BoxConfig(
                    Settings.ClientID, 
                    Settings.ClientSecret, 
                    Settings.EnterpriceId, 
                    Settings.PrivateKey, 
                    Settings.JwtPrivateKeyPassword, 
                    Settings.JwtPublicKeyId);

                var boxJWTAuth = new BoxJWTAuth(boxConfig);
                var adminToken = boxJWTAuth.AdminToken();
                var client = boxJWTAuth.AdminClient(adminToken);
                boxManager = new BoxManager(adminToken);

                var enrolledServices = repositoryEnrolledService.GetServicesByImportMode(ServiceReportImportModes.BoxAPI);

                if (enrolledServices.Model == null || enrolledServices.Model.List.Count() == 0)
                    return;

                foreach (var listItem in enrolledServices.Model.List)
                {
                    string importPath = Path.Combine(basePath, listItem.BusinessId.ToString(), "Sales", listItem.ServiceName);

                    BoxItem sharedItemInServiceFolder = await client.SharedItemsManager.SharedItemsAsync(listItem.BoxUrl);

                    var sharedServiceEnties = await client.FoldersManager.GetFolderItemsAsync(sharedItemInServiceFolder.Id, 100, 0,
                        new List<string>
                        {
                            BoxFolder.FieldName,
                            BoxFolder.FieldPathCollection,
                            BoxFolder.FieldModifiedAt,
                            BoxFolder.FieldItemCollection
                        }
                    );

                    foreach (var sharedEntry in sharedServiceEnties.Entries)
                    {
                        if (sharedEntry.Type.ToLower() == "file")
                        {
                            bool isFileExists = new RepositorySales().IsSalesFileExists(sharedEntry.Name, listItem.BusinessId, listItem.Id) || File.Exists(Path.Combine(importPath, sharedEntry.Name));
                            if (isFileExists)
                            {
                                
                                continue;
                            }

                            if (!Directory.Exists(importPath))
                                Directory.CreateDirectory(importPath);

                            using (FileStream fileStream = new FileStream(Path.Combine(importPath, sharedEntry.Name), FileMode.CreateNew, System.IO.FileAccess.Write))
                            {
                                using (Stream stream = await client.FilesManager.DownloadStreamAsync(sharedEntry.Id))
                                {
                                    int bytesRead;
                                    var buffer = new byte[8192];
                                    do
                                    {
                                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    } while (bytesRead > 0);
                                }
                            }

                            // Move download file to archive folder

                            var subfolder = new BoxApi.V2.Model.Folder();
                            var folderInfo = await client.FoldersManager.GetInformationAsync(sharedItemInServiceFolder.Id);
                            if (folderInfo.ItemCollection.Entries.OfType<BoxFolder>().Count() > 0)
                            {
                                var foundFolder = folderInfo.ItemCollection.Entries.OfType<BoxFolder>().Any((f) => f.Name == "Archive");
                                // var foundFolder = entryItems.Parent.ItemCollection.Entries.OfType<BoxFolder>().First((a) => a.Name == "my subfolder");
                                if (foundFolder == false)
                                {
                                    subfolder = boxManager.CreateFolder(sharedItemInServiceFolder.Id, "Archive");
                                    boxManager.MoveFile(sharedEntry.Id, subfolder.Id);
                                }
                                // Move the file to the subfolder
                                var foundFolderDetails = folderInfo.ItemCollection.Entries.OfType<BoxFolder>().First((f) => f.Name == "Archive");
                                boxManager.MoveFile(sharedEntry.Id, foundFolderDetails.Id);
                                // getfolderDetails(folderItems.Id, path);
                            }
                            else
                            {
                                subfolder = boxManager.CreateFolder(sharedItemInServiceFolder.Id, "Archive");
                                boxManager.MoveFile(sharedEntry.Id, subfolder.Id);
                            }
                        }
                        //else case entry type is 'folder'   
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                throw ex;
            }
        }
    }
}