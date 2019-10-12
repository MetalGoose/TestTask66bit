using System;
using System.Reflection;

namespace TestTask66bit
{
    public class ClassInfo
    {
        public string AssemblyName { get; set; }
        public Type Class { get; set; }
        public MethodInfo[] PublicMethods { get; set; }
        public MethodInfo[] PrivateMethods { get; set; }

        public ClassInfo() { }

        public ClassInfo(string assemblyName)
        {
            AssemblyName = assemblyName;
        }
    }
}
