using EBP.Business.Enums;
using EBP.Business.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinSCP;

namespace EBP.FtpImports
{
    public class ImportFtpFiles
    {
        public void ImportFiles(string basePath)
        {
                RepositoryEnrolledServices repositoryEnrolledService = new RepositoryEnrolledServices();
            var enrolledServices = repositoryEnrolledService.GetServicesByImportMode(ServiceReportImportModes.Ftp);

            if (enrolledServices.Model == null || enrolledServices.Model.List.Count() == 0)
                return;


            List<DownloadSettings> DownloadSettings = new List<DownloadSettings>();
            bool isSuccess = false;

            foreach (var service in enrolledServices.Model.List)
            {
                string importPath = Path.Combine(basePath, service.BusinessId.ToString(), "Sales", service.ServiceName);

                #region Ftp Details

                DownloadSettings.Add(new DownloadSettings
                {
                    SessionOptions = new SessionOptions
                    {
                        Protocol = (Protocol)service.FtpInfo.Protocol,
                        HostName = service.FtpInfo.Host,
                        UserName = service.FtpInfo.Username,
                        Password = service.FtpInfo.Passsword,
                        PortNumber = service.FtpInfo.PortNumber ?? 21,
                        //GiveUpSecurityAndAcceptAnySshHostKey = true
                    },
                    RemotePath = service.FtpInfo.RemotePath,
                    DestinationPath = importPath,
                    RemoteArchivesFolderPath = "/Archives/"
                });

            #endregion

            //Download Files from the FTP to the specified directory
            retryDownload:
                bool needSleep = false;
                isSuccess = new SyncMessageFiles().Download(DownloadSettings, out needSleep);
                if (needSleep)
                {
                    Thread.Sleep((int)(float.Parse(ConfigurationManager.AppSettings["SleepTime"]) * 60000));
                    goto retryDownload;
                }
                isSuccess = true;

                if (isSuccess)
                {
                    // Download success
                }
            }
        }
    }

    public class DownloadSettings
    {
        public SessionOptions SessionOptions { get; set; }

        public string RemotePath { get; set; }

        public string DestinationPath { get; set; }

        public bool RemoveDownloadedFile { get; set; }

        public string RemoteArchivesFolderPath { get; set; }
    }

    public class SyncMessageFiles
    {
        /// <summary>
        /// Download Message Files from the FTP
        /// </summary>
        /// <param name="objSyncSettings">FTP Settings like login credentials,Ftp source folder path, archives directory etc</param>
        /// <returns> returns True if download success otherwise returns false</returns>
        public bool Download(List<DownloadSettings> objSyncSettings, out bool needSleep)
        {
            try
            {
                foreach (DownloadSettings objSessionOptions in objSyncSettings)
                {
                    try
                    {
                        //www.codeproject.com/Tips/769049/sFTP-and-NET
                        using (Session session = new Session())
                        {
                            //session.SessionLogPath = "your log path";
                            session.Timeout = TimeSpan.MaxValue;
                            session.Open(objSessionOptions.SessionOptions); //Attempts to connect to your sFtp site
                            //Get Ftp File
                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Automatic; //The Transfer Mode - Automatic, Binary, or Ascii 
                            transferOptions.FilePermissions = null;  //Permissions applied to remote files; 
                            transferOptions.PreserveTimestamp = true;  //Set last write time of destination file 
                            //to that of source file - basically change the timestamp to match destination and source files.    
                            transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                            FilePermissions filePerm = new FilePermissions();
                            filePerm.UserWrite = true;
                            transferOptions.FilePermissions = filePerm;

                            TransferOperationResult transferResult;
                            //the parameter list is: remote Path, Local Path with filename 
                            //(optional - if different from remote path), Delete source file?, transfer Options  

                            List<string> remoteInnerDirectoryInfo = new List<string>();
                            var remotePath = objSessionOptions.RemotePath;
                            var localDestinationDir = objSessionOptions.DestinationPath;
                            var remoteArchivesFolderPath = objSessionOptions.RemoteArchivesFolderPath;

                        innerDir:

                            if (remoteInnerDirectoryInfo.Count > 0)
                            {
                                var innerDir = remoteInnerDirectoryInfo.Last();
                                remotePath = Path.Combine(objSessionOptions.RemotePath, innerDir);
                                localDestinationDir = Path.Combine(objSessionOptions.DestinationPath, innerDir);
                                remoteArchivesFolderPath = Path.Combine(objSessionOptions.RemoteArchivesFolderPath, innerDir);
                                remoteInnerDirectoryInfo.Remove(innerDir);
                            }

                            RemoteDirectoryInfo remoteDirectoryInfo = session.ListDirectory(remotePath);
                            foreach (RemoteFileInfo fileInfo in remoteDirectoryInfo.Files)
                            {
                                try
                                {
                                    if (fileInfo.IsDirectory && (fileInfo.Name.ToLower() == "finance"))
                                    {
                                        remoteInnerDirectoryInfo.Add(fileInfo.Name);
                                    }

                                    if (fileInfo.Length > 0)
                                    {
                                        string fileName = fileInfo.Name,
                                            sourceFile = Path.Combine(remotePath, fileName).Replace("\\", "/"),
                                            destinationFile = Path.Combine(remoteArchivesFolderPath, fileName).Replace("\\", "/"),
                                            localDestinationPath = Path.Combine(localDestinationDir, fileName);

                                        // Create the local directory for the incoming folder, if it is not exists.
                                        if (!Directory.Exists(localDestinationDir))
                                        {
                                            string[] s = localDestinationDir.Split('\\');
                                            string _path = s[0] + "\\";
                                            for (int i = 1; i < s.Length; i++)
                                            {
                                                if (!Directory.Exists(_path = Path.Combine(_path, s[i])))
                                                    Directory.CreateDirectory(_path);
                                            }
                                        }
                                        if (File.Exists(localDestinationPath))
                                        {
                                            localDestinationPath = Path.Combine(localDestinationDir, GenerateFileName(fileName));
                                        }
                                        if (!Directory.Exists(localDestinationDir))
                                            Directory.CreateDirectory(localDestinationDir);
                                        //Download file to the local directory

                                        // Get list of files in the directory
                                        RemoteDirectoryInfo directoryInfo = session.ListDirectory(objSessionOptions.RemotePath);
                                        
                                        #region Select the most recent file

                                        //// Select the most recent file
                                        //RemoteFileInfo latest =
                                        //    directoryInfo.Files
                                        //        //.Where(file => !file.IsDirectory)
                                        //        .Where(file => !file.Name.Contains('.'))
                                        //        .OrderByDescending(file => file.LastWriteTime)
                                        //        .FirstOrDefault();
                                        //if (latest != null)
                                        //{
                                        //    string tempLocalPath = GetTempFilePathWithExtension(".timestamp"),
                                        //        trmpServerPath = Path.Combine(objSessionOptions.RemotePath, Path.GetFileName(tempLocalPath));
                                        //    using (new FileStream(tempLocalPath, FileMode.CreateNew))
                                        //    {
                                        //    }
                                        //    transferResult = session.PutFiles(tempLocalPath, trmpServerPath, true);
                                        //    RemoteFileInfo s = session.GetFileInfo(trmpServerPath);
                                        //    session.RemoveFiles(trmpServerPath);
                                        //    TimeSpan ts = latest.LastWriteTime.Subtract(s.LastWriteTime);
                                        //    if (Math.Abs(ts.Minutes) < 15)
                                        //    {
                                        //        needSleep = true;
                                        //        return false;
                                        //    }
                                        //}

                                        #endregion

                                        transferResult = session.GetFiles(sourceFile, localDestinationPath, objSessionOptions.RemoveDownloadedFile, transferOptions);
                                    tryMoveFile:
                                        //Move downloaded file to the archive's folder
                                        if (transferResult.IsSuccess)
                                        {
                                            if (!string.IsNullOrEmpty(remoteArchivesFolderPath) && !session.FileExists(destinationFile))
                                            {
                                                //Create archives directory, if it is not exist in FTP
                                                if (!session.FileExists(remoteArchivesFolderPath))
                                                {
                                                    string[] s = remoteArchivesFolderPath.Split('/');
                                                    string _path = string.IsNullOrEmpty(s[0]) ? "/" : s[0];
                                                    for (int i = 1; i < s.Length; i++)
                                                    {
                                                        if (!session.FileExists(_path = Path.Combine(_path, s[i]).Replace("\\", "/")))
                                                            session.CreateDirectory(_path);
                                                    }
                                                }
                                                session.MoveFile(sourceFile, destinationFile);
                                            }
                                            else
                                            {
                                                fileName = GenerateFileName(fileName);
                                                destinationFile = Path.Combine(remoteArchivesFolderPath, fileName);
                                                goto tryMoveFile;
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    continue;
                                }
                            }

                            if (remoteInnerDirectoryInfo.Count() > 0)
                            {
                                goto innerDir;
                            }

                            //Throw on any error 
                            //transferResult.Check();
                            //Log information and break out if necessary  
                        }
                    }
                    catch(Exception ex)
                    {
                        continue;
                    }
                }
                needSleep = false;
                return true;
            }
            catch
            {
                needSleep = true;
                return false;
            }
        }

        /// <summary>
        /// Generate a unique name for the filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GenerateFileName(string fileName)
        {
            return string.Format("{0}-{1:yyyyMMddHHmmss}{2}", Path.GetFileNameWithoutExtension(fileName), DateTime.Now, Path.GetExtension(fileName));
        }

        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }
    }
}
