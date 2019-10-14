using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

namespace TestTask66bit
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Enter the path to the directory with the dll Assembly: ");
            string path = Console.ReadLine();
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    string[] fileNames = Directory.GetFiles(path, "*.dll");
                    PrintInfoToConsole(fileNames);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n" + e.StackTrace + "\n" + e.Source);
                }
            }
            else
            {
                Console.WriteLine("The file path cannot be empty");
            }

            Console.Write("Press any key to Exit");
            Console.ReadKey();
        }

        private static void PrintInfoToConsole(string[] filePaths)
        {
            if (filePaths.Length != 0)
            {
                foreach(string path in filePaths)
                {
                    List<ClassInfo> classes = Parse(path);
                    
                    //Sort by Class.Name
                    classes.Sort(delegate(ClassInfo firstClassInfo, ClassInfo secondClassInfo)
                    {
                        return firstClassInfo.Class.Name.CompareTo(secondClassInfo.Class.Name);
                    });

                    foreach (ClassInfo classInfo in classes)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Class " + classInfo.Class.Name);
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
        private static List<ClassInfo> Parse(string pathToFile)
        {
            List<ClassInfo> result = new List<ClassInfo>();

            List<Type> classes = GetClassInfos(pathToFile);

            foreach (var type in classes)
            {
                ClassInfo classInfo = new ClassInfo
                {
                    Class = type,
                    PrivateMethods = GetMethodInfos(type, true),
                    PublicMethods = GetMethodInfos(type)
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
        private static MethodInfo[] GetMethodInfos(Type classType, bool getPrivate = false)
        {
            try
            {
                if (!classType.IsClass) { throw new ApplicationException($"Object {classType} is not a class"); }

                var methodInfos = getPrivate ? classType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance) : classType.GetMethods();

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
        private static List<Type> GetClassInfos(string pathToFile)
        {
            if (string.IsNullOrEmpty(pathToFile))
            {
                throw new ApplicationException("File path cannot be empty");
            }

            Assembly assembly = Assembly.LoadFile(pathToFile);

            List<Type> foundClasses = new List<Type>();
            List<Type> foundTypes;

            try
            {
                foundTypes = assembly.GetTypes().ToList();
            }
            // The array returned by the Types property of ReflectionTypeLoadException contains a Type object for each type that was loaded,
            // and a null object for each type that failed to load
            catch (ReflectionTypeLoadException e)
            {
                foundTypes = e.Types.ToList();
            }
            
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
