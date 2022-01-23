using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace EasyConsole
{
    class Program
    {

        public static NetworkStream networkStream;
        public static TcpClient socketForServer;
        public static string clientmessage = "test";

        // The port number for the remote device.  
        private const int port = 66;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        private static string ipserver { get; set; }
        private static string selectedWork { get; set; }


        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the ip of the server");
            ipserver = Console.ReadLine();
            StartClient(ipserver);
            while (true)
            {
                Console.WriteLine("Choose the save you want to control");
                selectedWork = Console.ReadLine();
                Console.WriteLine("Choose if you want to    PLAY->1   PAUSE->2   STOP->3  ");
                int input = int.Parse(Console.ReadLine());
                switch (input)
                {
                    case 1:
                        PlayBackup();
                        break;
                    case 2:
                        PauseBackup();
                        break;
                    case 3:
                        StopBackup();
                        break;
                }

            }


        }

        private static void StartClient()//Function to start the function to start the connection with the server.
        {
            try
            {
                StartClient(ipserver);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Server not found " + ex.ToString());
            }

        }

        private static void PlayBackup()//Method for the play button, to start the save process.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipserver);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "PLAY" + selectedWork);
                sendDone.WaitOne();

            }
            catch
            {

            }
        }

        private static void PauseBackup()//Method for the pause button, to pause the backup.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipserver);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "PAUSE" + selectedWork);
                sendDone.WaitOne();

            }
            catch
            {

            }

        }

        private static void StopBackup()//Method for the stop button, to stop backup
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipserver);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "STOP" + selectedWork);
                sendDone.WaitOne();

            }
            catch
            {

            }

        }

        public static void StartClient(string ip)//Function that allows to start the client with the server and view the backups.
        {
            // Connect to a remote device.  
            try
            {
                // Establish the local endpoint for the socket.
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress ipAddress = IPAddress.Parse("127.0.0.2");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "getdata");
                sendDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(client);
                receiveDone.WaitOne();

                string request = response;
                string[] array = request.Split(Environment.NewLine);
                foreach (var obj in array)
                {
                    Console.WriteLine(" - " + obj); //Displaying backups in the listbox
                }

            }
            catch (Exception e)
            {
                Console.WriteLine (e.ToString());
            }

        }

        private static void ConnectCallback(IAsyncResult ar)//Receive the message and call ReceiveCallBack
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client) //Reads and assigns message to response
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)//Function to receive the message
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)//Function to send a message
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)//Function that allows you to send a message asynchronously. Method linked with the send function
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void LoadProgress()//Function to retrieve progress information.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ipserver);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "getprogressing" + selectedWork);
                sendDone.WaitOne();

                Receive(client);
                receiveDone.WaitOne();
               Console.WriteLine("GET PRGRESSING....");
                Console.WriteLine("Progressing : " + response);
            }
            catch
            {

            }
        }
    }
}
