using System;
using System.IO;
using System.Linq;
using Abp.Reflection.Extensions;

namespace Magicodes.Admin.Web
{
    /// <summary>
    /// 此类用于查找源码根目录以及Web工程目录
    /// 用于EF Core命令行获取连接字符串.
    /// </summary>
    public static class WebContentDirectoryFinder
    {
        public static string CalculateContentRootFolder()
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(AdminCoreModule).GetAssembly().Location);
            if (coreAssemblyDirectoryPath == null)
            {
                throw new Exception("无法获取Magicodes.Admin.Core程序集所在的目录!");
            }
            //遍历获取根目录
            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!directoryInfo.GetFiles("*.sln").Any())
            {
                if (directoryInfo.Parent == null)
                {
                    throw new Exception("无法找到源码根目录!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            var webHostFolder = Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}admin{Path.DirectorySeparatorChar}api{Path.DirectorySeparatorChar}Admin.Host");
            if (Directory.Exists(webHostFolder))
            {
                return webHostFolder;
            }
            else
            {
                webHostFolder = Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}app{Path.DirectorySeparatorChar}api{Path.DirectorySeparatorChar}App.Host");
                if (Directory.Exists(webHostFolder))
                {
                    return webHostFolder;
                }
            }

            throw new Exception("无法找到Web工程目录!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
