using System;
using System.IO;
using System.Linq;

namespace AddNewComponentInBlazor
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new Exception("Please provde a file name!");
            }

            CreateDotRazorFile(filename);
            CreateDotRazorDotCsFile(filename, GetNamespace());
        }

        /// <summary>
        /// Method will create the .razor file automatically connected to the .razor.cs file backend
        /// </summary>
        /// <param name="filename"></param>
        private static void CreateDotRazorFile (string filename)
        {
            string[] lines = new string[]
            {
                "@inherits " + filename + "Base",
                "<h3>" + filename + "</h3>"
            };

            File.WriteAllLines(filename + ".razor", lines);
        }

        /// <summary>
        /// Method will create a .razor.cs file with a classname + "Base" that automatically inherits ComponentBase
        /// </summary>
        /// <param name="filename"></param>
        private static void CreateDotRazorDotCsFile (string filename, string calculatedNamespace)
        {
            string[] lines = new string[]
            {
                "using Microsoft.AspNetCore.Components;",
                "\n",
                "namespace " + calculatedNamespace,
                "{",
                "\tpublic class " + filename + "Base : ComponentBase",
                "\t{",
                "\t\t",
                "\t}",
                "}"
            };

            File.WriteAllLines(filename + ".razor.cs", lines);
        }




        /// <summary>
        /// Method recursively goes upwards until it finds the .csproj file.
        /// It then returns the namespace as {{.csprojFilenameWithoutExtension}}.{{each}}.{{subfolder}}.{{it}}.{{went}}.{{through}}
        /// </summary>
        /// <param name="path">Empty by default. If provided, it will start the search from the provided path.</param>
        /// <returns>The namespace for the classes.</returns>
        private static string GetNamespace(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                // default value, should look from current folder downwards
                path = Directory.GetCurrentDirectory();
            }

            // Check if there is a .csproj file in given folder
            // If so, calculate the namespace and finish the search
            var csProjFileName = Directory.GetFiles(path).FirstOrDefault(f => f.EndsWith(".csproj"));
            if (csProjFileName != null)
            {
                return Path.GetFileName(csProjFileName).Substring(0, Path.GetFileName(csProjFileName).LastIndexOf(".csproj"));
            } else
            {
                var pathInfo = Directory.GetParent(path);
                if (pathInfo == null)
                {
                    throw new Exception("Got to root without finding a .csproj file. Please make sure you are inside a C# project folder or any of its subfolders");
                } else
                {
                    var currentFolderName = Path.GetFileName(path);
                    path = pathInfo.FullName.TrimEnd('\\');
                    return GetNamespace(path) + "." + currentFolderName;
                }
            }
        }
    }
}
