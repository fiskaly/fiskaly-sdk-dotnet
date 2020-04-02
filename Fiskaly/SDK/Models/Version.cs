
namespace Fiskaly.Models
{
    public class Version
    {
        public ComponentVersion SmaersVersion { get; set; }
        public ComponentVersion ClientVersion { get; set; }
        public string SdkVersion { get; set; }

        public override string ToString()
        {
            return 
                "SDK: {\n\tVersion: " + SdkVersion + "\n}\n" +
                "Client: " + ClientVersion.ToString() + "\n" +
                "Smaers: " + SmaersVersion.ToString() + "\n";
        }
    }

    public class ComponentVersion
    {
        public string Version { get; set; }
        public string CommitHash { get; set; }
        public string SourceHash { get; set; }

        public override string ToString()
        {
            return 
                "{\n" +
                    "\tVersion: " + Version + ",\n" +
                    "\tCommitHash: " + CommitHash + ",\n" +
                    "\tSourceHash: " + SourceHash + "\n" +
                "}";
        }
    }
}
