using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class File
    {
        public string TargetFolder { get; set; }
        public string Mirror { get; set; }
        public string SourceFolder { get; set; }
        public string FileName { get; set; }
        public int Type { get; set; }

        public File(string fileName, string sourceFolder, string targetFolder, int type, string mirror)
        {
            TargetFolder = targetFolder;
            Mirror = mirror;
            SourceFolder = sourceFolder;
            FileName = fileName;
            Type = type;
        }
    }

}