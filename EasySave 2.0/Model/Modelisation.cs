using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using EasySave.Model;
using System.Threading;
using System.Xml;
using EasySave.ViewModel;
using CryptoSoftLib;

namespace EasySave.Model
{
    class Modelisation
    {
        //Declaration of all variables and properties
        public int checkdatabackup;
        private string serializeObj;
        public string backupListFile = System.Environment.CurrentDirectory + @"\Works\";
        public string stateFile = System.Environment.CurrentDirectory + @"\State\";
        public string statexml = System.Environment.CurrentDirectory + @"\State\";
        public string dailyxml = System.Environment.CurrentDirectory + @"\Daily\";
        public string dailyjson = System.Environment.CurrentDirectory + @"\Daily\";

        public logState logState { get; set; }
        public Log Log { get; set; }
        
        public string NameSaved { get; set; }
        public string BackupNameState { get; set; }
        public string Source { get; set; }
        public int nbfilesmax { get; set; }
        public int nbfiles { get; set; }
        public long size { get; set; }
        public float progress { get; set; }
        public string Target { get; set; }
        public string FileName { get; set; }
        public int Type { get; set; }
        public string File { get; set; }

        public string TypeString { get; set; }
        public long TotalSize { get; set; }
        public TimeSpan TimeTransfert { get; set; }
        public string userMenuInput { get; set; }
        public string MirrorDir { get; set; }
        public object JsonConvert { get; private set; }


        private static readonly Extensionfiletocrypt extension;
        private string prio = extension.FileType1;
        private string crypt = extension.FileType2;
        CryptoLib crypto = new CryptoLib();
        public long Time { get; set; }


        public void Model()
        {
            userMenuInput =  " ";
            //Check if Json and folder are correctly running

            if (!Directory.Exists(backupListFile)) 
            {
                DirectoryInfo di = Directory.CreateDirectory(backupListFile);
            }
            backupListFile += @"backupList.json";


            if (!Directory.Exists(stateFile))
            {
                DirectoryInfo di = Directory.CreateDirectory(stateFile);
            }
            stateFile += @"state.json";

            if (!Directory.Exists(statexml))
            {
                DirectoryInfo di = Directory.CreateDirectory(statexml);
            }
            statexml += @"state.xml";

            if (!Directory.Exists(dailyxml))
            {
                DirectoryInfo di = Directory.CreateDirectory(dailyxml);
            }
            dailyxml += @"DailyLogs_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xml";

            if (!Directory.Exists(dailyjson))
            {
                DirectoryInfo di = Directory.CreateDirectory(dailyjson);
            }
            dailyjson += @"DailyLogs_" + DateTime.Now.ToString("dd-MM-yyyy") + ".json";


        }

        public void CompleteSave(string inputpathsave, string inputDestToSave, bool copyDir, bool verif) //Function for full folder backup
        {
            logState = new logState(NameSaved);
            this.logState.SState = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //Starting the timed for the log file

            DirectoryInfo dir = new DirectoryInfo(inputpathsave);  // Get the subdirectories for the specified directory.

            if (!dir.Exists) //Check if the file is present
            {
                throw new DirectoryNotFoundException("ERROR 404 : Directory Not Found ! " + inputpathsave);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(inputDestToSave); // If the destination directory doesn't exist, create it.  

            FileInfo[] files = dir.GetFiles(); // Get the files in the directory and copy them to the new location.
            FileInfo[] TabPrio = new FileInfo[files.Length];
            FileInfo[] Tab = new FileInfo[files.Length];
            int i = 0;
            int j = 0;


            if (!verif) //  Check for the status file if it needs to reset the variables
            {
                TotalSize = 0;
                nbfilesmax = 0;
                size = 0;
                nbfiles = 0;
                progress = 0;

                foreach (FileInfo file in files) // Loop to allow calculation of files and folder size
                {
                    TotalSize += file.Length;
                    nbfilesmax++;
                }
                foreach(DirectoryInfo subdir in dirs) // Loop to allow calculation of subfiles and subfolder size
                {
                    FileInfo[] Maxfiles = subdir.GetFiles();
                    foreach(FileInfo file in Maxfiles)
                    {
                        TotalSize += file.Length;
                        nbfilesmax++;
                    }
                }

            }

            //Loop that allows to copy the files to make the backup
            foreach (FileInfo file in files) 
            {
                string tempPath = Path.Combine(inputDestToSave, file.Name);

                if(size > 0)
                {
                    progress = ((float)size / TotalSize) * 100;
                }

                //Systems which allows to insert the values ​​of each file in the report file.
                logState.SourceFolder = Path.Combine(inputpathsave, file.Name);
                logState.TargetFolder = tempPath;
                logState.TotalFile = nbfilesmax;
                logState.TotalSize = TotalSize;
                logState.TotalSizeRest = TotalSize - size;
                logState.FileRest = nbfilesmax - nbfiles;
                logState.Progress = progress;


                UpdateStatefile(); //Call of the function to start the state file system

                Thread.Sleep(1500);


                if (prio.IndexOf(Path.GetExtension(Path.Combine(inputpathsave, file.Name))) > -1)
                {
                    TabPrio[i] = file;
                    i++;
                }
                else
                {
                    Tab[j] = file;
                    j++;
                }
                nbfiles++;
                size += file.Length;

            }

            foreach(FileInfo file in TabPrio)
            {
                string tempPath = Path.Combine(inputDestToSave, file.Name);
                CryptOrSave(file, tempPath);
            }
            foreach(FileInfo file in Tab)
            {
                string tempPath = Path.Combine(inputDestToSave, file.Name);
                CryptOrSave(file, tempPath);
            }


            // If copying subdirectories, copy them and their contents to new location.
            if (copyDir)  
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(inputDestToSave, subdir.Name);
                    CompleteSave(subdir.FullName, tempPath, copyDir, true);
                }   
            }
            //System which allows the values ​​to be reset to 0 at the end of the backup
           /* logState.TotalSize = TotalSize;
            logState.SourceFolder = null;
            logState.TargetFolder = null;
            logState.TotalFile = 0;
            logState.TotalSize = 0;
            logState.TotalSizeRest = 0;
            logState.FileRest = 0;
            logState.Progress = 0;
            logState.SState = false;*/

            UpdateStatefile(); //Call of the function to start the state file system

            stopwatch.Stop(); //Stop the stopwatch
            this.TimeTransfert = stopwatch.Elapsed; // Transfer of the chrono time to the variable
        }


        private void CryptOrSave(FileInfo file, string endpath)
        {

            string originpath = file.FullName;
            string key = "5";
            if (crypt.IndexOf(Path.GetExtension(file.Name)) > -1)
            {
                string endfile = Path.Combine(endpath, file.Name);
                Time = crypto.CryptoSoft(originpath, endfile, key);
            }
            else
            {
                file.CopyTo(endpath, true); //Function that allows you to copy the file to its new folder.
            }
        }


        private void UpdateStatefile()//Function that updates the status file.
        {
            List<logState> stateList = new List<logState>();
            this.serializeObj = null;
            if (!System.IO.File.Exists(stateFile)) //Checking if the file exists
            {
                System.IO.File.Create(stateFile).Close();
            }

            string jsonString = System.IO.File.ReadAllText(stateFile);  //Reading the json file

            if (jsonString.Length != 0) //Checking the contents of the json file is empty or not
            {
                logState[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<logState[]>(jsonString); //Derialization of the json file

                foreach (var obj in list) // Loop to allow filling of the JSON file
                {
                    if(obj.FileName == this.NameSaved) //Verification so that the name in the json is the same as that of the backup
                    {
                        obj.SourceFolder = this.logState.SourceFolder;
                        obj.TargetFolder = this.logState.TargetFolder;
                        obj.TotalFile = this.logState.TotalFile;
                        obj.TotalSize = this.logState.TotalSize;
                        obj.FileRest = this.logState.FileRest;
                        obj.TotalSizeRest = this.logState.TotalSizeRest;
                        obj.Progress = this.logState.Progress;
                        obj.DateBackup = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        obj.SState = this.logState.SState;
                    }

                    stateList.Add(obj); //Allows you to prepare the objects for the json filling

                }

                this.serializeObj = Newtonsoft.Json.JsonConvert.SerializeObject(stateList.ToArray(), Newtonsoft.Json.Formatting.Indented) + Environment.NewLine; //Serialization for writing to json file

                System.IO.File.WriteAllText(stateFile, this.serializeObj); //Function to write to JSON file

                // To convert JSON text contained in string json into an XML node
                string Row = "Save";
                XmlDocument doc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode("{"+Row+":" + this.serializeObj + "}", "root");
                doc.Save(statexml);
            }


        }

        public void UpdateLogFile(string FileName, string Source, string target)//Function to allow modification of the log file
        {
            Stopwatch stopwatch = new Stopwatch();
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", TimeTransfert.Hours, TimeTransfert.Minutes, TimeTransfert.Seconds, TimeTransfert.Milliseconds / 10); //Formatting the stopwatch for better visibility in the file
            
            logDaily datalogs = new logDaily //Apply the retrieved values ​​to their classes
            {
                DName = FileName,
                DSource = Source,
                DTarget = target,
                DateBackup = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                DSize = TotalSize,
                DTime = elapsedTime
            };

            string path = System.Environment.CurrentDirectory; //Allows you to retrieve the path of the program environment
            var directory = System.IO.Path.GetDirectoryName(path); // This file saves in the project: \EasySaveApp\bin

            string serializeObj = Newtonsoft.Json.JsonConvert.SerializeObject(datalogs, Newtonsoft.Json.Formatting.Indented) + Environment.NewLine; //Serialization for writing to json file
            System.IO.File.AppendAllText(dailyjson, serializeObj); //Function to write to log file

            string Row = "Save";
            XmlDocument doc = Newtonsoft.Json.JsonConvert.DeserializeXmlNode("{" + Row + ":" + this.serializeObj + "}", "root");
            doc.Save(dailyxml);

            stopwatch.Reset(); // Reset of stopwatch
        }
       
        public void AddSave(EasySave.Model.File backup) //Function that creates a backup job
        {
            List<File> backupList = new List<File>();
            this.serializeObj = null; 

            if (!System.IO.File.Exists(backupListFile)) //Checking if the file exists
            {
                System.IO.File.WriteAllText(backupListFile, this.serializeObj);
            }

            string jsonString = System.IO.File.ReadAllText(backupListFile); //Reading the json file

            if (jsonString.Length != 0) //Checking the contents of the json file is empty or not
            {
                File[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<File[]>(jsonString); //Derialization of the json file
                foreach ( var obj in list) //Loop to add the information in the json
                {
                    backupList.Add(obj);
                }
            }
            backupList.Add(backup); //Allows you to prepare the objects for the json filling

            this.serializeObj = Newtonsoft.Json.JsonConvert.SerializeObject(backupList.ToArray(), Newtonsoft.Json.Formatting.Indented) + Environment.NewLine; //Serialization for writing to json file
            System.IO.File.WriteAllText(backupListFile, this.serializeObj); // Writing to the json file

            logState = new logState(this.FileName); //Class initiation

            logState.DateBackup = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); //Adding the time in the variable
            AddState(); //Call of the function to add the backup in the report file.
        }

        public void AddState() //Function that allows you to add a backup job to the report file.
        {
            List<logState> stateList = new List<logState>();
            this.serializeObj = null;

            if (!System.IO.File.Exists(stateFile)) //Checking if the file exists
            {
                System.IO.File.Create(stateFile).Close();
            }

            string jsonString = System.IO.File.ReadAllText(stateFile); //Reading the json file

            if (jsonString.Length != 0)
            {
                logState[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<logState[]>(jsonString); //Derialization of the json file
                foreach (var obj in list) //Loop to add the information in the json
                {
                    stateList.Add(obj);
                }
            }
            this.logState.SState = false;
            stateList.Add(this.logState); //Allows you to prepare the objects for the json filling

            this.serializeObj = Newtonsoft.Json.JsonConvert.SerializeObject(stateList.ToArray(), Newtonsoft.Json.Formatting.Indented) + Environment.NewLine; //Serialization for writing to json file
            System.IO.File.WriteAllText(stateFile, this.serializeObj);// Writing to the json file


        }

        public void LoadSave(string backupname) //Function that allows you to load backup jobs
        {
            File backup = null;
            this.TotalSize = 0;
            BackupNameState = backupname;

            string jsonString = System.IO.File.ReadAllText(backupListFile); //Reading the json file


            if (jsonString.Length != 0) //Checking the contents of the json file is empty or not
            {
                File[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<File[]>(jsonString);  //Derialization of the json file
                foreach (var obj in list)
                {
                    if(obj.FileName == backupname) //Check to have the correct name of the backup
                    {
                        backup = new File(obj.FileName, obj.SourceFolder, obj.TargetFolder, obj.Type, obj.Mirror); //Function that allows you to retrieve information about the backup
                    }
                }
            }


            NameSaved = backup.FileName;
            var t = new Thread(() => CompleteSave(backup.SourceFolder, backup.TargetFolder, true, false)); //Calling the function to run the full backup
            t.Start();
            UpdateLogFile(backup.FileName, backup.SourceFolder, backup.TargetFolder); //Call of the function to start the modifications of the log file
        }

        public void CheckDataFile()  // Function that allows to count the number of backups in the json file of backup jobs
        {
            checkdatabackup = 0;

            if (System.IO.File.Exists(backupListFile)) //Check on file exists
            {
                string jsonString = System.IO.File.ReadAllText(backupListFile);//Reading the json file
                if (jsonString.Length != 0)//Checking the contents of the json file is empty or not
                {
                    File[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<File[]>(jsonString); //Derialization of the json file
                    checkdatabackup = list.Length; //Allows to count the number of backups
                }
            }
        }

        public List<File> NameList()//Function that lets you know the names of the backups.
        {
            List<File> backupList = new List<File>();

            if (!System.IO.File.Exists(backupListFile)) //Checking if the file exists
            {
                System.IO.File.WriteAllText(backupListFile, this.serializeObj);
            }

            List<File> names = new List<File>();
            string jsonString = System.IO.File.ReadAllText(backupListFile); //Function to read json file
            File[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<File[]>(jsonString); // Function to dezerialize the json file

            if (jsonString.Length != 0)
            {
                foreach (var obj in list) //Loop to display the names of the backups
                {
                    names.Add(obj);
                }

            }

            return names;

        }

    }

}
