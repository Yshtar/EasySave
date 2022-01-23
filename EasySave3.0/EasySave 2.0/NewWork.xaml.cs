using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasySave.Model;


namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NewWork : Window
    {
      
        public string langSave { get; set; }
        public int Type { get; set; }

        private Modelisation model;


        public NewWork(string lang)
        {
            InitializeComponent();
            LanguageSwitch(lang);
            langSave = lang;
            model = new Modelisation();
            model.Model();

        }
        public void Back()
        {
            MainWindow menuback1 = new MainWindow(langSave);
            menuback1.Show();
            this.Close();
        }
        public void LanguageSwitch(string lang)
        {
            if (lang == "French")
            {
                NameTk.Text = "Nom de votre sauvegarde";
                SourceTk.Text = "Répertoire Source";
                TargetTk.Text = "Répertoire Cible";
                TypeTk.Text = "Type de la sauvegarde";
                Type1.Content = "Complète";
                Type2.Content = "Différentielle";
                Submit.Content = "Valider";
            }

            else if (lang == "English")
            {
                NameTk.Text = "Name of the Save";
                SourceTk.Text = "Source Directory";
                TargetTk.Text = "Target Directory";
                TypeTk.Text = "Type of the Save";
                Type1.Content = "Complete";
                Type2.Content = "Differential";
                Submit.Content = "Submit";
            }

        }


        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            model.FileName = TName.Text;
            model.Source = TSourceDir.Text;
            model.Target = TTargetDir.Text;

            File backup = new File(model.FileName, model.Source, model.Target, model.Type, "");
            model.AddSave(backup);
            Back();

        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back();

        }

        private void Type1_Click(object sender, RoutedEventArgs e)
        {
            Type = 1;
            Type_Info.Text = "Complete";
        }

        private void Type2_Click(object sender, RoutedEventArgs e)
        {
            Type = 2;
            Type_Info.Text = "Differential";
        }

        private void TargetFolder_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox.

            // Load content of file in a TextBlock
            if (result == true)
            {
                TTargetDir.Text = openFileDlg.FileName;
                //NormalSaveSourcePathTextBlock.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox.

            // Load content of file in a TextBlock
            if (result == true)
            {
                TSourceDir.Text = openFileDlg.FileName;
                //NormalSaveSourcePathTextBlock.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }
    }
}
