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
            String text = Clipboard.GetText();

            // Connecten
            MqttClient client = new MqttClient("broker.mqttdashboard.com");
            byte code = client.Connect(Guid.NewGuid().ToString());

            Console.WriteLine("Geben sie \"get\" oder \"post\" ein");
            string GetPostCmd = Console.ReadLine();
           
            if(GetPostCmd.ToLower() == "get")
            {
                // Zum Topic Subscriben
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                ushort msgIdSub = client.Subscribe(new string[] { "zwischenablage/windows" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                Console.WriteLine("Sie können nun Clipboards geteilt bekommen. Drücken sie eine Taste, um Abzubrechen.");
                Console.ReadKey();
                Console.Clear();
                client.Disconnect();
                
            }
            else if(GetPostCmd.ToLower() == "post")
            {
                // Clipboard publishen
                ushort msgIdPub = client.Publish("zwischenablage/windows",
                Encoding.UTF8.GetBytes(text),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false);
                Console.WriteLine("Ihr Clipboard wurde erfolgreich gepostet. Das Programm wird neugestartet, wenn sie eine Taste drücken.");
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
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
            thread.Abort();
            Console.WriteLine("Der Text " + msgReceived + " wurde in ihre Zwischenablage kopiert! Drücken sie eine Taste um das Programm neuzustarten!");
            Console.ReadKey();
            Console.Clear();
            StartProgram();
            
            
        }
        


    }
}