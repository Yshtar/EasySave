using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    class logState : Log
    {


        //Specific variables
        public int SNumberFiles { get; set; }
        public int SNumberLeft { get; set; }
        public long TotalSize { get; set; }
        public long TotalSizeRest { get; set; }
        public int TotalFile { get; set; }
        public int FileRest { get; set; }
        public float Progress { get; set; }
        public bool SState { get; set; }

        
        public logState(string sName)
        {
            FileName = sName;
        }
    }
}

