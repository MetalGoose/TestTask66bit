using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace TestTask66bit
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\NET\Repos\Iskra\Aistenok\bin\Debug";

            string[] fileNames = Directory.GetFiles(path, "*.dll");

            PrintInfoToConsole(fileNames);
            Console.ReadKey();
        }

        public static void PrintInfoToConsole(string[] filePaths)
        {
            if (filePaths != null)
            {
                foreach(string path in filePaths)
                {
                    List<ClassInfo> classes = Parse(path);

                    foreach (ClassInfo classInfo in classes)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Class " + classInfo.Class.Name);
                        Console.WriteLine();

                        foreach (MethodInfo method in classInfo.PrivateMethods)
                        {
                            if (method.IsFamily)
                            {
                                Console.WriteLine($"protected {method.Name}");
                            }
                        }

                        foreach (MethodInfo method in classInfo.PublicMethods)
                        {
                            Console.WriteLine($"public {method.Name}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No files found");
            }
        }

        /// <summary>
        /// Returns objects of type ClassInfo with information about the class itself and the methods contained in it.
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public static List<ClassInfo> Parse(string pathToFile)
        {
            List<ClassInfo> result = new List<ClassInfo>();

            List<Type> classes = GetClassInfos(pathToFile);

            for (int i = 0; i < classes.Count; i++)
            {
                ClassInfo classInfo = new ClassInfo
                {
                    Class = classes[i],
                    PrivateMethods = GetMethodInfos(classes[i], true),
                    PublicMethods = GetMethodInfos(classes[i])
                };
                result.Add(classInfo);
            }
            return result;
        }

        /// <summary>
        /// Returns an array with methods found in the class
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="getPrivate"></param>
        /// <remarks>
        /// if getPrivate is true returns both private and protected methods
        /// </remarks>
        public static MethodInfo[] GetMethodInfos(Type classType, bool getPrivate = false)
        {
            MethodInfo[] methodInfos;

            try
            {
                if (!classType.IsClass) { throw new ApplicationException($"Object {classType} is not a class"); }

                methodInfos = getPrivate == true ? classType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance) : classType.GetMethods();

                return methodInfos;
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message + "" + e.StackTrace + "" + e.Source);
            }

        }

        /// <summary>
        /// Returns a list with the found types of type Class.If there are none, returns an empty list
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public static List<Type> GetClassInfos(string pathToFile)
        {
            if (string.IsNullOrEmpty(pathToFile))
            {
                throw new ApplicationException("File path cannot be empty");
            }

            Assembly assembly = Assembly.LoadFile(pathToFile);

            List<Type> foundClasses = new List<Type>();
            Type[] foundTypes; 
            
            try { foundTypes = assembly.GetTypes(); }
            // The array returned by the Types property of ReflectionTypeLoadException contains a Type object for each type that was loaded,
            // and a null object for each type that failed to load
            catch (ReflectionTypeLoadException e) { foundTypes = e.Types; }

            foreach (Type type in foundTypes)
            {
                if (type != null)
                {
                    if (type.IsClass)
                    {
                        foundClasses.Add(type);
                    }
                }
            }
            return foundClasses;
        }
    }
}
