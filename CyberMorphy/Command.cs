using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberMorphy
{
    class Command
    {
        [JsonProperty]
        private String head;
        [JsonProperty]
        private String body;

        public Command(String head, String body)
        {
            this.head = head;
            this.body = body;
        }

        public String getHead()
        {
            return head;
        }

        public void setHead(String head)
        {
            this.head = head;
        }

        public String getBody()
        {
            return body;
        }

        public void setBody(String body)
        {
            this.body = body;
        }
    }
}
