using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySave.Model;

namespace EasySave.Model
{
    class logDaily : Log
    {
        //Inherits from Log
        public string DName { get; set; }
        public long DSize { get; set; }
        public string DSource { get; set; }
        public string DTarget { get; set; }
        public string DTime { get; set; }

        //Specific variables
        public string DTransferTime { get; set; }

    }
}