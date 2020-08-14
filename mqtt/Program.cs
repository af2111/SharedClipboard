using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;


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
            if(StartCmd == "start")
            {
                
                PublishClipboard();
                StartProgram();
            }

            else
            {
                Console.WriteLine("Falsches Kommando!");
                StartProgram();
            }
        }
        static void PublishClipboard()
        {
            String text = Clipboard.GetText();
            MqttClient client = new MqttClient("localhost");
            byte code = client.Connect(Guid.NewGuid().ToString());
            


            ushort msgId = client.Publish("zwischenablage/zwischenablage", // topic
                           Encoding.UTF8.GetBytes(text), // message body
                           MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                           false); // retained
        }
       
    }
}
