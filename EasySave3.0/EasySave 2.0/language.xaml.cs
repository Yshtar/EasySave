using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EasySave.Model;


namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for language.xaml
    /// </summary>
    public partial class Language : Window
    {
        public string langLang { get; set; }

        public Language(string lang)
        {
            InitializeComponent();
            langLang = lang;
        }

        private void Back()
        {
            MainWindow menuback1 = new MainWindow(langLang);
            menuback1.Show();
            this.Close();
        }

        private void French_Click(object sender, RoutedEventArgs e)
        {
            langLang = "French";
            Back();
        }
        private void English_Click(object sender, RoutedEventArgs e)
        {
            langLang = "English";
            Back();
        }

        private void backmenu(object sender, RoutedEventArgs e)
        {
            Back();
        }
    }
}
