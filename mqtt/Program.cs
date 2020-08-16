using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;
using System.IO;

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
            MqttClient client = new MqttClient("localhost");
            byte code = client.Connect(Guid.NewGuid().ToString());

            Console.WriteLine("Geben sie \"get\" oder \"post\" ein");
            string GetPostCmd = Console.ReadLine();
            if(GetPostCmd.ToLower() == "get")
            {
                // Zum Topic Subscriben
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                ushort msgIdSub = client.Subscribe(new string[] { "zwischenablage/zwischenablage" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            else if(GetPostCmd.ToLower() == "post")
            {
                // Clipboard publishen
                ushort msgIdPub = client.Publish("zwischenablage/zwischenablage",
                Encoding.UTF8.GetBytes(text),
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                false);
            }
            else
            {
                Console.WriteLine("Falsches Kommando! Versuche es erneut");
                client.Disconnect();
                initClipboard();
            }
           
            
            
            

        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            String msgReceived = Encoding.UTF8.GetString(e.Message);
            Clipboard.SetText(msgReceived);
            Console.WriteLine(msgReceived);
        }



    }
}