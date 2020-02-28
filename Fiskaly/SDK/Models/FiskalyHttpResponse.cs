using System.Collections.Generic;

namespace Fiskaly.Client.Models
{
    public class FiskalyHttpResponse
    {
        public int Status { get; set; }
        public string Reason { get; set; }
        public byte[] Body { get; set; }
        public Dictionary<string, string[]> Headers { get; set; }
    }
}
