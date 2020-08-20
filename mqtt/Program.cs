using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace mqtt
{
    class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            StartProgram();
            

        }
        [STAThread]

        static void StartProgram()
        {
            Console.WriteLine("Geben sie start ein, um das Program zu Starten.");
            String StartCmd = Console.ReadLine();
            if (StartCmd.ToLower() == "start")
            {

                initClipboard();
                StartProgram();
            }

            else
            {
                Console.WriteLine("Falsches Kommando!");
                StartProgram();
                
            }
        }
        [STAThread]
        static void initClipboard()
        {
            
            // Clipboard bekommen
            String myClipboard = Clipboard.GetText();

            // Connecten
            MqttClient client = new MqttClient("broker.mqttdashboard.com");
            byte code = client.Connect(Guid.NewGuid().ToString());

            Console.WriteLine("Gib \"get\" oder \"post\" ein!");
            string GetPostCmd = Console.ReadLine();
           
            if(GetPostCmd.ToLower() == "get")
            {
                Console.WriteLine("Gib das Passwort ein.");
                string PasswordEingabe = Console.ReadLine();
                // Zum Topic Subscriben
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                ushort msgIdSub = client.Subscribe(new string[] { "zwischenablage" + PasswordEingabe + "/windows" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                Console.WriteLine("Du kannst nun Clipboards geteilt bekommen. Drücke eine Taste, um Abzubrechen.");
                Console.ReadKey();
                Console.Clear();
                client.Disconnect();
            }
            else if(GetPostCmd.ToLower() == "post")
            {
                Console.WriteLine("Wähle ein Passwort.");
                string Password = Console.ReadLine();
                Console.WriteLine("Das Passwort wurde gewählt. Drücke eine Taste um deine Zwischenablage zu posten.");
                Console.ReadKey();
                // Clipboard publishen
                ushort msgIdPub = client.Publish("zwischenablage" + Password + "/windows",
                Encoding.UTF8.GetBytes(myClipboard),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false);
                Console.WriteLine("Dein Clipboard wurde erfolgreich gepostet. Das Programm wird neugestartet, wenn du eine Taste drückst.");
                Console.ReadKey();
                Console.Clear();
                client.Disconnect();
            }
            else
            {
                Console.WriteLine("Falsches Kommando! Versuche es erneut");
                client.Disconnect();
                initClipboard();
                

            }
           
            
            
            

        }
        [STAThread]
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
           
                string msgReceived = Encoding.UTF8.GetString(e.Message);
                Thread thread = new Thread(() => Clipboard.SetText(msgReceived));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                thread.Abort();
                Console.WriteLine("Der Text " + msgReceived + " wurde in deine Zwischenablage kopiert! Drück eine Taste um das Programm neuzustarten!");
                Console.ReadKey();
                Console.Clear();
                StartProgram();
          
           
            
            
        }
        


    }
}