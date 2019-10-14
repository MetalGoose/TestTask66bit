using System;
using System.Reflection;

namespace TestTask66bit
{
    public class ClassInfo
    {
        public Type Class { get; set; }
        public MethodInfo[] PublicMethods { get; set; }
        public MethodInfo[] PrivateMethods { get; set; }
    }
}
