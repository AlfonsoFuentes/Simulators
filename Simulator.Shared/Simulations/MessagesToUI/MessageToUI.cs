using Simulator.Shared.Simulations.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.Simulations.MessagesToUI
{
    public class MessageToUIList
    {
        public BaseLine Line { get; private set; }

        public MessageToUIList(BaseLine line)
        {
            Line = line;
        }

        public string CurrenTime => Line.CurrentDate.ToString();

        public Func<MessageToUI, Task> SendMessage { get; set; } = null!;
       
        public void OnSendMessageToUI(MessageToUI message)
        {
            Task.Run(async () => await SendMessage.Invoke(message));

        }
        
    }
    public class MessageToUI
    {
        public string Line { get; set; } = string.Empty;
        public string Equipment { get; set; } = string.Empty;
        public string EquipmentStatus { get; set; } = string.Empty;
        public string EquipmentValue { get; set; } = string.Empty;

    }
}
