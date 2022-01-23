using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using EasySave.Model;




namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for Extensionfiletocrypt.xaml
    /// </summary>
    public partial class Extensionfiletocrypt : Window
    {
        public string langExt { get; set; }
        public string FileType1 { get; private set; }
        public string FileType2 { get; private set; }

        public Extensionfiletocrypt(string lang)
        {
            InitializeComponent();
            LanguageSwitch(lang);
            langExt = lang;

        }

        public void LanguageSwitch(string lang)
        {
            if (lang == "French")
            {
                Label1.Content = "Entrez toutes les extensions de fichiers que vous \nvoulez avec les points :";
                Enter.Content = "Valider";
                button2.Content = "Valider";
                File1.Text = "Extentsions de fichiers à prioriser :";
                File2.Text = "Extentsions de fichiers à crypter :";

            }

            else if (lang == "English")
            {
                Label1.Content = "Enter all the extension file you want with point :  ";
                Enter.Content = "Submit";
                button2.Content = "Submit";
                File1.Text = "File extension to prioritise :";
                File2.Text = "File extension to encrypt :";
            }

        }

        private void filecrypt(object sender, RoutedEventArgs e)
        {
            Extension.crypt = text2.Text;


            Thread.Sleep(10);

            if (langExt == "French")
            {
                Message2.Text = "La liste ci-contre vous confirme ce que vous avez entré, vous pouvez revenir au menu si cela vous convient ou ajouter des enxtensions.";
            }

            else if (langExt == "English")
            {
                Message2.Text = "List aside show you the extensions you enter, you can go back to the menu or change.";
            }

            File2_Copy.Text = Extension.crypt;

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void backmenu(object sender, RoutedEventArgs e)
        {
            MainWindow menuback1 = new MainWindow(langExt);
            menuback1.Show();
            this.Close();
        }

        private void Prio_Click(object sender, RoutedEventArgs e)
        {
            Extension.prio = text1.Text;


            Thread.Sleep(10);

            if (langExt == "French")
            {
                Message.Text = "La liste ci-dessus vous confirme ce que vous avez entré, vous pouvez revenir au menu si cela vous convient ou ajouter des enxtensions.";
            }

            else if (langExt == "English")
            {
                Message.Text = "List above show you the extensions you enter, you can go back to the menu or change.";
            }

            File1_Copy.Text = Extension.prio;

        }
    }
}
