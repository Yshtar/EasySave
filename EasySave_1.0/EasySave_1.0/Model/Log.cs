using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{

    class Log
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string SourceFolder { get; set; }
        public string TargetFolder { get; set; }
        public string DateBackup { get; set; }
        public string SaveTime { get; set; }


    }
}