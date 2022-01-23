using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EasySave.Model;

namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for LoadWork.xaml
    /// </summary>
    public partial class LoadWork : Window

    {
        public string LoadName { get; private set; }
        public string langLoad { get; set; }

        private Modelisation model;


        public LoadWork(string lang)
        {
            InitializeComponent();
            LanguageSwitch(lang);
            langLoad = lang;
            model = new Modelisation();
            model.Model();
        }

        public void LanguageSwitch(string lang)
        {
            if (lang == "French")
            {
                MessageEnter.Text = "Entrez le nom de la sauvegarde que vous souhaitez charger :";
                Submit.Content = "Valider";
            }

            else if (lang == "English")
            {
                MessageEnter.Text = "Enter the name of the save you want to load :";
                Submit.Content = "Submit";
            }

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            LoadName = SaveName.Text;

             model.LoadSave(LoadName);

            SaveName.Text = "";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(langLoad);
            main.Show();
            this.Close();
        }

        private void SaveName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
