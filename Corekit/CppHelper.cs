using Corekit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corekit
{
    public class CppHelper
    {
        public class Definition
        {
            /// <summary>
            /// ns1::ns2::Name
            /// </summary>
            public string ClassName { get; private set; }

            /// <summary>
            /// ns1 ns2 Name
            /// </summary>
            public IEnumerable<string> NameSpace { get; private set; }

            /// <summary>
            /// "ns1::ns2"
            /// </summary>
            public string NameSpaceStr { get; }

            /// <summary>
            /// Name
            /// </summary>
            public string TypeName { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Definition(string className)
            {
                var elemenets  = className.Split(NameSpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                this.ClassName = className;
                this.NameSpace = elemenets.TakeWhile((str, idx) => idx < (elemenets.Length - 1)).ToList();
                this.TypeName  = elemenets.LastOrDefault();
                this.NameSpaceStr = string.Join("::", this.NameSpace);
            }

            public StringBuilder AppendClassSyntax(StringBuilder builder)
            {
                builder.AppendLine($"class {this.TypeName};");
                return builder;
            }

            public StringBuilder AppendNamespaceSyntax(StringBuilder builder)
            {
                foreach(var nameSpace in this.NameSpace)
                {
                    builder.AppendLine($"namespace {nameSpace} {{");
                }
                return builder;
            }

            public StringBuilder AppendNameSpaceCloseSyntax(StringBuilder builder)
            {
                foreach (var nameSpace in this.NameSpace)
                {
                    builder.AppendLine($"}}");
                }
                return builder;
            }

            public StringBuilder AppendCreateFuncDeclaration(StringBuilder builder)
            {
                builder.AppendLine($"{this.ClassName}* create{this.TypeName}(void* ptr);");
                return builder;
            }

            public StringBuilder AppendCreateFuncName(StringBuilder builder)
            {
                builder.AppendLine($"create{this.TypeName}");
                return builder;
            }

            private static readonly string[] NameSpaceSeparator = { "::" };
        }


        public static void Run()
        {
            var definitions = new[] {
                new Definition("game::physics::Component1"),
                new Definition("game::physics::Component2"),
                new Definition("game::phive::Component"),
                new Definition("engine::physics::Component")
            };

            var builder = new StringBuilder();
            var nameSpaces = definitions.GroupBy(i => i.NameSpaceStr);

            foreach (var ns in nameSpaces)
            {
                ns.First().AppendNamespaceSyntax(builder);
                ns.ForEach(i => i.AppendClassSyntax(builder));
                ns.First().AppendNameSpaceCloseSyntax(builder);
            }

            builder.AppendLine("namespace {");
            definitions.ForEach(i => i.AppendCreateFuncDeclaration(builder));
            builder.AppendLine("}");

            Console.WriteLine(builder.ToString());
        }
    }
}
