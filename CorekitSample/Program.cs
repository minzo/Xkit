using System;

namespace CorekitSample
{
    public interface IMappingInfo
    {
        string SourceName { get; set; }
        string TargetName { get; set; }
    }

    public class MappingInfo : IMappingInfo
    {
        public string SourceName { get; set; }

        public string TargetName { get; set; }

        public string PhysicsMaterialName { get; set; }

        public string LayerEntityHitMask { get; set; }

        public string SubLayerEntityHitMask { get; set; }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
