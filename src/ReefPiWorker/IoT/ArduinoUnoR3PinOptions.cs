using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReefPiWorker.IoT
{
    public class ArduinoUnoR3PinOptions
    {
        public int PinLedStatus { get; set; }
        public int PinDht22 { get; set; }
        public int PinButton { get; set; }
        public string PortName { get; set; } = string.Empty;
        public int BoudRate { get; set; }
    }
}
