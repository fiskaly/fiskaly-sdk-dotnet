using System.Collections.Generic;

namespace Fiskaly.Models
{
    public class HealthStatus
    {
        public ComponentHealth Proxy { get; set; }
        public ComponentHealth Smaers { get; set; }
        public Dictionary<string, string> Backend { get; set; }

        public override string ToString()
        {
            return "{\n" +
                "\tProxy: " + this.Proxy + "\n"
                + "\tSmaers: " + this.Smaers + "\n"
                + "\tBackend: " + this.Backend + "\n}";
        }
    }

    public class ComponentHealth
    {
        public ComponentHealth(string status)
        {
            this.Status = status;
        }

        public string Status { get; }

        public override string ToString()
        {
            return "{\n" +
                "\tStatus: " + this.Status + "\n" +
                "}";
        }
    }
}
