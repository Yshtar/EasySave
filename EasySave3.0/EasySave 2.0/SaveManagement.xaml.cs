using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EasySave.Model;

namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for SaveManagement.xaml
    /// </summary>
    /// 



    public partial class SaveManagement : Window
    {

        Thread a, b, c, d, g, f;
        Thread t, u, v, w, x, y;

        bool pause1 = false;
        bool pause2 = false;
        bool pause3 = false;
        bool pause4 = false;
        bool pause5 = false;
        bool pause6 = false;

        public string langManag { get; set; }
        public string LoadName { get; private set; }
        private Modelisation model;

        public SaveManagement(string lang)
        {
            InitializeComponent();
            LanguageSwitch(lang);
            langManag = lang;
            model = new Modelisation();
            model.Model();

            List <string> names = MainWindow.ListBackup();
            Save1.ItemsSource = names;
            Save2.ItemsSource = names;
            Save3.ItemsSource = names;
            Save4.ItemsSource = names;
            Save5.ItemsSource = names;
            Save6.ItemsSource = names;

        }

        public void LanguageSwitch(string lang)
        {
            if (lang == "French")
            {
                Label1.Content = "Veuillez sélectionner le travail de sauvegarde afin de le contrôler";
            }

            else if (lang == "English")
            {
                Label1.Content = "Choose the Save you want to control";
            }

        }

        private void backmenu(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(langManag);
            main.Show();
            this.Close();
        }

        private void Play_Click1(object sender, RoutedEventArgs e)
        {
            if (pause2)
            {
                b.Interrupt();
            }
        }

        private void Play_Click2(object sender, RoutedEventArgs e)
        {
            if (pause3)
            {
                c.Interrupt();
            }
        }

        private void Play_Click3(object sender, RoutedEventArgs e)
        {
            if (pause4)
            {
                d.Interrupt();
            }
        }

        private void Play_Click4(object sender, RoutedEventArgs e)
        {
            if (pause5)
            {
                g.Interrupt();
            }
        }

        private void Play_Click5(object sender, RoutedEventArgs e)
        {
            if (pause6)
            {
                f.Interrupt();
            }
        }

        private void Pause_Click2(object sender, RoutedEventArgs e)
        {
            if (pause2)
            {
                b.Interrupt();
            }
        }

        private void Pause_Click3(object sender, RoutedEventArgs e)
        {
            if (pause3)
            {
                c.Interrupt();
            }
        }

        private void Pause_Click4(object sender, RoutedEventArgs e)
        {
            if (pause4)
            {
                d.Interrupt();
            }
        }

        private void Pause_Click5(object sender, RoutedEventArgs e)
        {
            if (pause5)
            {
                g.Interrupt();
            }
        }

        private void Pause_Click6(object sender, RoutedEventArgs e)
        {
            if (pause6)
            {
                f.Interrupt();
            }
        }

        private void Stop_Click2(object sender, RoutedEventArgs e)
        {
            b.Abort();
        }

        private void Stop_Click3(object sender, RoutedEventArgs e)
        {
            c.Abort();
        }

        private void Stop_Click4(object sender, RoutedEventArgs e)
        {
            d.Abort();
        }

        private void Stop_Click5(object sender, RoutedEventArgs e)
        {
            g.Abort();
        }

        private void Stop_Click6(object sender, RoutedEventArgs e)
        {
            f.Abort();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (pause1)
            {
                a.Interrupt();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (!pause1)
            {
                a = new Thread(Wait);
                a.Start();
                pause1 = true;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            a.Abort();
        }

        private void Lancer1_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save1.Text;
            t = new Thread(() => model.LoadSave(LoadName));
            t.Start();
        }
        private void Lancer2_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save2.Text;

            u = new Thread(() => model.LoadSave(LoadName));
            u.Start();
        }
        private void Lancer3_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save3.Text;

            v = new Thread(() => model.LoadSave(LoadName));
            v.Start();
        }
        private void Lancer4_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save4.Text;

            w = new Thread(() => model.LoadSave(LoadName));
            w.Start();
        }
        private void Lancer5_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save5.Text;

            x = new Thread(() => model.LoadSave(LoadName));
            x.Start();
        }
        private void Lancer6_Click(object sender, RoutedEventArgs e)
        {
            LoadName = Save6.Text;

            y = new Thread(() => model.LoadSave(LoadName));
            y.Start();
        }

        private void Wait()
        {
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
