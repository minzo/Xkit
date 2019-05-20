using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Corekit
{
    /// <summary>
    /// .vcxproj ファイルを出力します
    /// </summary>
    public class VcxprojBuilder
    {
        public static void Build(string path, IEnumerable<string> includePathes)
        {
            var project = Project(ToolVersion,
                Comment("自動生成"),
                ProjectConfigurations(
                    (target: "Debug", platform: "X64"),
                    (target: "Release", platform: "X64")
                ),
                Import(@"$(VCTargetsPath)\Microsoft.Cpp.default.props"),
                PropertyGroup("Configuration"),
                Import(@"$(VCTargetsPath)\Microsoft.Cpp.props"),
                ImportGroup("ExtensionSettings"),
                ImportGroup("PropertySheets"),
                PropertyGroup("UserMacros"),
                Import(@"$(VCTargetsPath)\Microsoft.Cpp.targets"),
                ImportGroup("ExtensionTargets"));

            new XDocument(new XDeclaration("1.0", "utf-8", null), project).Save(path);
        }

        static private readonly string ToolVersion = "4.0";

        static private readonly XNamespace ns = @"http://schemas.microsoft.com/developer/msbuild/2003";

        static private XElement Project(string ToolVersion, params XNode[] node)
            => new XElement( ns + "Project",
                new XAttribute("ToolVersion", ToolVersion),
                new XAttribute("DefaultTarget", "Build"),
                node);    

        static private XElement ProjectConfigurations(params (string target, string platform)[] configurations)
            => ItemGroup("ProjectConfigurations", configurations.Select(i => ProjectConfiguration(i.target, i.platform)));

        static private XElement ProjectConfiguration(string Target, string Platform)
            => new XElement("ProjectConfiguration", new XAttribute("Include", $"{Target}|{Platform}"),
                         new XElement("Configuration", Target),
                         new XElement("Configuration", Platform)
                         );

        static private XElement IncludeItems(IEnumerable<string> includePathes)
            => new XElement("ItemGroup", includePathes.Select(i => new XElement("ClInclude", new XAttribute("Include", i))));

        static private XElement IncludeCompiles(IEnumerable<string> includePathes)
            => new XElement("ItemGroup", includePathes.Select(i => new XElement("ClCompile", new XAttribute("Include", i))));

        static private XElement ItemGroup(string label, params object[] items) 
            => new XElement("ItemGroup", new XAttribute("Label", label), items);

        static private XElement PropertyGroup(string label, params object[] properties)
            => new XElement("PropertyGroup", new XAttribute("Label", label), properties);

        static private XElement Import(string path) 
            => new XElement("Import", new XAttribute("Project", path));

        static private XElement ImportGroup(string label)
            => new XElement("ImportGroup", new XAttribute("label", label));

        static private XComment Comment(string comment)
            => new XComment(comment);
    }
}
