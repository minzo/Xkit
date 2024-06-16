using System;
using System.Collections.Concurrent;

#nullable enable

namespace Corekit.Models
{
    public class InheritanceObjectManager
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceObjectManager()
        {
            this._TypeInfo = new ConcurrentDictionary<string, InheritanceObjectTypeInfo>();

            this.RegisterTypeInfo(sTypeInfo_String);
            this.RegisterTypeInfo(sTypeInfo_S32);
            this.RegisterTypeInfo(sTypeInfo_U32);
            this.RegisterTypeInfo(sTypeInfo_F32);
            this.RegisterTypeInfo(sTypeInfo_F64);
        }

        /// <summary>
        /// 
        /// </summary>
        public InheritanceObjectTypeInfo CreateTypeInfo(string typeName)
        {
            if (!this._TypeInfo.TryGetValue(typeName, out InheritanceObjectTypeInfo? typeInfo))
            {
                typeInfo = new InheritanceObjectTypeInfo(typeName);
                this._TypeInfo.TryAdd(typeName, typeInfo);
            }
            return typeInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public InheritanceObjectTypeInfo? GetTypeInfo(string typeName)
        {
            if (this._TypeInfo.TryGetValue(typeName, out InheritanceObjectTypeInfo? typeInfo))
            {
                return typeInfo;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterTypeInfo(InheritanceObjectTypeInfo typeInfo)
        {
            this._TypeInfo.TryAdd(typeInfo.Name, typeInfo);
        }

        private readonly ConcurrentDictionary<string, InheritanceObjectTypeInfo> _TypeInfo;

        private static readonly InheritanceObjectTypeInfo sTypeInfo_String = new InheritanceObjectTypeInfo(InheritanceObjectBuildInTypeName.String);
        private static readonly InheritanceObjectTypeInfo sTypeInfo_S32 = new InheritanceObjectTypeInfo(InheritanceObjectBuildInTypeName.S32);
        private static readonly InheritanceObjectTypeInfo sTypeInfo_U32 = new InheritanceObjectTypeInfo(InheritanceObjectBuildInTypeName.U32);
        private static readonly InheritanceObjectTypeInfo sTypeInfo_F32 = new InheritanceObjectTypeInfo(InheritanceObjectBuildInTypeName.F32);
        private static readonly InheritanceObjectTypeInfo sTypeInfo_F64 = new InheritanceObjectTypeInfo(InheritanceObjectBuildInTypeName.F64);
    }

    /// <summary>
    /// 組み込みタイプ名
    /// </summary>
    public static class InheritanceObjectBuildInTypeName
    {
        public static string String = "string";
        public static string S32 = "s32";
        public static string U32 = "u32";
        public static string F32 = "f32";
        public static string F64 = "f64";
    }
}
