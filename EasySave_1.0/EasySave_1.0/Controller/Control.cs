using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using EasySave.Model;
using EasySave.view;
using File = EasySave.Model.File;

namespace EasySave.controller
{
    class Controller
    {
        private Modelisation model;
        private View view;
        private int inputMenu;
        public string lang = "en";

        public Controller()
        {
            model = new Modelisation();
            view = new View();
            model.Model();
            view.ShowIntro(); //Function call
            model.userMenuInput = Menu(); //Function call

        }

        private string GetSourceDir() //Function to retrieve the input from the source
        {
            string sourceDir = "";
            bool isValid = false;

            while (!isValid) //Loop to allow verification of the path
            {
                sourceDir = Console.ReadLine(); //Retrieving user input
                if (Directory.Exists(sourceDir.Replace("\"", ""))) //Remplace \ for ""
                {
                    isValid = true;
                }
                else
                {
                    view.ErrorMenu("Incorect Path"); //Show error message
                }

            }
            return sourceDir;
        }
        private string GetTargetDir() //Function to retrieve the input from the destination repesetory
        {
            string targetDir = "";
            bool isValid = false;

            while (!isValid)//Loop to allow verification of the path
            {
                targetDir = Console.ReadLine();// Retrieving user input
                if (Directory.Exists(targetDir.Replace("\"", "")))  //Remplace \ for ""
                {
                    isValid = true;
                }
                else
                {
                    view.ErrorMenu("Incorect Path"); //Show error message
                }

            }
            return targetDir;
        }

        private string Menu() //function for menu management
        {
            Stopwatch stopwatch = new Stopwatch();
            bool menu = true;
            while (menu) //Loop for menu
            {
                model.CheckDataFile(); // Calling the function to check the number of backups
                                       //try
                                       //{
                view.ShowMenu(lang); //Calling the function to display the menu
                inputMenu = int.Parse(Console.ReadLine()); //Retrieving user input for menu

                switch (inputMenu) // Switch of menu
                {
                    case 1:
                        Console.Clear();
                        view.ShowLanguageMenu();
                        int input_lan = int.Parse(Console.ReadLine());
                        switch (input_lan)
                        {
                            case 1:
                                lang = "en";
                                Console.Clear();
                                break;
                            case 2:
                                lang = "fr";
                                Console.Clear();
                                break;
                        }
                        break;

                    case 2:

                        if (model.checkdatabackup < 5) // Check not to exceed the save limit
                        {
                            Console.Clear(); //Console cleaning
                            view.ShowTypeChooseMenu(lang); // Calling the function to display the second menu
                            MenuSub(); // Calling the function for the second menu
                        }
                        else
                        {
                            Console.Clear(); //Console cleaning
                            view.ErrorMenu("You already have 5 backups to create."); // Show Error Message
                        }
                        break;

                    case 3:

                        view.ShowNameFile(lang); //Display message introduction on the backup names

                        string jsonString = System.IO.File.ReadAllText(model.backupListFile); //Function to read json file
                        File[] list = JsonConvert.DeserializeObject<File[]>(jsonString); // Function to dezerialize the json file

                        foreach (var obj in list) //Loop to display the names of the backups
                        {
                            Console.WriteLine(" - " + obj.FileName); //Display of backup names
                        }
                        view.AskName(lang);//Calling the function to display the names of the backups
                        string inputnamebackup = Console.ReadLine(); // Recovering backup names
                        model.LoadSave(inputnamebackup); // Calling the function to start the backup
                        break;

                    case 4:
                        Environment.Exit(0); //Stop the programs
                        break;
                }

                /*}
                catch
                {
                    Console.Clear();//Console cleaning
                }*/
            }

            return "";

        }

        private void MenuSub() //Function for the menu when creating backup jobs.
        {
            bool menusub = true;
            while (menusub) //Loop for menu
            {
                try
                {
                    int inputMenuSub = int.Parse(Console.ReadLine()); //Retrieving user input for second menu
                    switch (inputMenuSub) // Switch of menu
                    {

                        case 1: //Case 1, creating a full backup job
                            model.Type = 1; //Type declaration for backup
                            view.AskName(lang); //Display for backup name
                            model.FileName = Console.ReadLine(); // Retrieving the name of the backup
                            view.AskSourceDir(lang); // Display for folder source
                            model.Source = GetSourceDir(); // Function for checking the folder path
                            view.AskTargetDir(lang); // Display for the folder destination
                            model.Target = GetTargetDir();  // Function for checking the folder path
                            File backup = new File(model.FileName, model.Source, model.Target, model.Type, "");
                            model.AddSave(backup); // Calling the function to add a backup job
                            break;

                        case 2: //Case 2, creating a differential backup job
                            model.Type = 2; //Type declaration for backup
                            view.AskName(lang);
                            model.FileName = Console.ReadLine();
                            view.AskSourceDir(lang);
                            model.Source = GetSourceDir();
                            model.MirrorDir = "Xelkyo/EasySave/Version - 1.0/Application/Mirror";
                            view.AskTargetDir(lang);
                            model.Target = GetTargetDir();
                            File backup2 = new File(model.FileName, model.Source, model.Target, model.Type, model.MirrorDir);
                            model.AddSave(backup2); // Calling the function to add a backup job
                            break;

                        case 3:
                            Console.Clear();//Console cleaning
                            Menu(); //Calling up the menu function
                            break;
                    }

                }
                catch
                {
                    Console.Clear();
                    Menu(); //Calling up the menu function
                }

            }

        }

    }
}