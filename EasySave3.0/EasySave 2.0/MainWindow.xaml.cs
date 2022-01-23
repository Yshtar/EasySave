using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EasySave.Model;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using EasySave.ViewModel;

namespace EasySave.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string langmain { get; set; } = "English";
        private static Modelisation model;
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        public static ManualResetEvent allDone = new ManualResetEvent(false);


        public MainWindow()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                InitializeComponent();
                model = new Modelisation();
                Thread ServerThread = new Thread(StartServer);
                ServerThread.Start();
                mutex.ReleaseMutex();

            }
            else
            {
                MessageBox.Show("Instance already running");
                Environment.Exit(0);
                return;
            }
        }

        public MainWindow(string lang)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                InitializeComponent();
                LanguageSwitch(lang);
                langmain = lang;
            }
            else
            {
                MessageBox.Show("Instance already running");
                Environment.Exit(0);
                return;
            }
        }

        public void LanguageSwitch(string lang)
        {
            if (lang == "French")
            {
                Button2.Content = "2. Extension de fichiers";
                Button3.Content = "3. Créer nouvelle sauvegarde";
                Button4.Content = "4. Charger une sauvegarde";
                Button5.Content = "5. Gestion des Sauvegardes";
                Button6.Content = "6. Quit";
            }

            else if (lang == "English")
            {
                Button2.Content = "2. Extension file";
                Button3.Content = "3. Create new Work";
                Button4.Content = "4. Load Work";
                Button5.Content = "5. Save Managing";
                Button6.Content = "6. Quit";
            }

        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Language langue = new Language(langmain);
            langue.Show();
            this.Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

            Extensionfiletocrypt crypt = new Extensionfiletocrypt(langmain);
            crypt.Show();
            this.Close();
        }

        private void button3(object sender, RoutedEventArgs e)
        {
            NewWork work1 = new NewWork(langmain);
            work1.Show();
            this.Close();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            LoadWork load = new LoadWork(langmain);
            load.Show();
            this.Close();
        }
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            SaveManagement saveManagement = new SaveManagement(langmain);
            saveManagement.Show();
            this.Close();
        }

        private void close(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public static List<string> ListBackup()//Function that lets you know the lists of the names of the backups.
        {
            List<string> nameslist = new List<string>();
            foreach (var obj in model.NameList())
            {
                nameslist.Add(obj.FileName);
            }
            return nameslist;
        }

        public void StartServer()//Function to start the server
        {
            // Establish the local endpoint for the socket.    
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 66);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();// Set the event to nonsignaled state.  

                    // Start an asynchronous socket to listen for connections. 
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();// Wait until a connection is made before continuing.  
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();// Signal the main thread to continue.  

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            try
            {
                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead >= 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read
                    // more data.  
                    content = state.sb.ToString();

                    List<string> names = ListBackup();
                    foreach (var name in names)//Loop that allows you to manage the names in the list.
                    {
                        if (content.IndexOf("getdata") > -1)
                        {
                            Send(handler, name + Environment.NewLine); //Function that allows you to insert the names of the backups in the list.
                        }
                        else if (content.IndexOf("PLAY" + name) > -1)
                        {
                            // MessageBox.Show("PLAY" + name);
                            model.LoadSave(name);

                        }
                        else if (content.IndexOf("PAUSE" + name) > -1)
                        {
                            MessageBox.Show("PAUSE" + name);

                        }
                        else if (content.IndexOf("STOP" + name) > -1)
                        {
                            MessageBox.Show("STOP" + name);
                        }
                        else if (content.IndexOf("getprogressing" + name) > -1)
                        {
                            string prog = "Progressions de la Save";
                            Send(handler, prog);
                        }
                        else
                        {
                            // Not all data received. Get more.  
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                        }

                    }
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch
            {

            }
        }

        private void Send(Socket handler, String data)//Function to send a message
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);// Convert the string data to byte data using ASCII encoding.  

                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler); // Begin sending the data to the remote device. 

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        private void SendCallback(IAsyncResult ar)//Function to send a message a asynchronous
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;// Retrieve the socket from the state object.  

                int bytesSent = handler.EndSend(ar); // Complete sending the data to the remote device.  
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

    }
}