using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    //Class used for differential backup that is comparing the hashes of the files to know which file has been modified.
    class ComparingFiles : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public ComparingFiles() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2) // it compare the files so we know which one match with the differential backup
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }


        public int GetHashCode(System.IO.FileInfo fi) // Function to get the hash of the file
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }
}