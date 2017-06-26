using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Abp.Authorization;
using Abp.IO;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Dto;
using Magicodes.Admin.IO;
using Magicodes.Admin.Logging.Dto;
using Magicodes.Admin.Net.MimeTypes;

namespace Magicodes.Admin.Logging
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
    public class WebLogAppService : AdminAppServiceBase, IWebLogAppService
    {
        private readonly IAppFolders _appFolders;

        public WebLogAppService(IAppFolders appFolders)
        {
            _appFolders = appFolders;
        }

        public GetLatestWebLogsOutput GetLatestWebLogs()
        {
            var directory = new DirectoryInfo(_appFolders.WebLogsFolder);
            if (!directory.Exists)
            {
                return new GetLatestWebLogsOutput { LatestWebLogLines = new List<string>() };
            }

            var lastLogFile = directory.GetFiles("*.txt", SearchOption.AllDirectories)
                                        .OrderByDescending(f => f.LastWriteTime)
                                        .FirstOrDefault();

            if (lastLogFile == null)
            {
                return new GetLatestWebLogsOutput();
            }

            var lines = AppFileHelper.ReadLines(lastLogFile.FullName).Reverse().Take(1000).ToList();
            var logLineCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("DEBUG") ||
                    line.StartsWith("INFO") ||
                    line.StartsWith("WARN") ||
                    line.StartsWith("ERROR") ||
                    line.StartsWith("FATAL"))
                {
                    logLineCount++;
                }

                lineCount++;

                if (logLineCount == 100)
                {
                    break;
                }
            }

            return new GetLatestWebLogsOutput
            {
                LatestWebLogLines = lines.Take(lineCount).Reverse().ToList()
            };
        }

        public FileDto DownloadWebLogs()
        {
            //Create temporary copy of logs
            var tempLogDirectory = CopyAllLogFilesToTempDirectory();
            var logFiles = new DirectoryInfo(tempLogDirectory).GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();

            //Create the zip file
            var zipFileDto = new FileDto("WebSiteLogs.zip", MimeTypeNames.ApplicationZip);
            var outputZipFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, zipFileDto.FileToken);

            using (var outputZipFileStream = File.Create(outputZipFilePath))
            {
                using (var zipStream = new ZipArchive(outputZipFileStream, ZipArchiveMode.Create))
                {
                    foreach (var logFile in logFiles)
                    {
                        var entry = zipStream.CreateEntry(logFile.Name);
                        using (var entryStream = entry.Open())
                        {
                            using (var fs = new FileStream(logFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
                            {
                                fs.CopyTo(entryStream);
                                entryStream.Flush();
                            }
                        }
                    }
                }
            }

            //Delete temporary copied logs
            Directory.Delete(tempLogDirectory, true);

            return zipFileDto;
        }

        private string CopyAllLogFilesToTempDirectory()
        {
            var tempDirectoryPath = Path.Combine(_appFolders.TempFileDownloadFolder, Guid.NewGuid().ToString("N").Substring(16));
            DirectoryHelper.CreateIfNotExists(tempDirectoryPath);

            foreach (var file in GetAllLogFiles())
            {
                var destinationFilePath = Path.Combine(tempDirectoryPath, file.Name);
                File.Copy(file.FullName, destinationFilePath, true);
            }

            return tempDirectoryPath;
        }

        private List<FileInfo> GetAllLogFiles()
        {
            var directory = new DirectoryInfo(_appFolders.WebLogsFolder);
            return directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).ToList();
        }
    }
}