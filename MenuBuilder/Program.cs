using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 7)
            {
                PrintUsage();
                return;
            }

            string defaultLanguage = args[0];
            List<string> languages = new List<string>();
            languages.Add(defaultLanguage);
            bool languageDependent = true;
            string sortOrder = "title";
            string path = "";
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "/N")
                {
                    languageDependent = false;
                }
                else if (args[i] == "/L")
                {
                    if (i + 1 > args.Length)
                    {
                        Console.WriteLine("Missing languages\n");
                        PrintUsage();
                        return;
                    }

                    string[] langs = args[i + 1].Split(new Char[] { ',' });
                    foreach (var lang in langs)
                    {
                        if (languages.Contains(lang))
                        {
                            Console.WriteLine("Language " + lang + " already exists");
                            PrintUsage();
                            return;
                        }
                        languages.Add(lang);
                    }
                    i++;
                }
                else if (args[i] == "/S") 
                {
                    if (i + 1 > args.Length)
                    {
                        Console.WriteLine("Missing sort order\n");
                        PrintUsage();
                        return;
                    }
                    sortOrder = args[i + 1];

                    if (!(sortOrder == "file" || sortOrder == "title" || sortOrder == "matrix"))
                    {
                        Console.WriteLine(string.Format("Invalid sort order {0}\n", sortOrder));
                        PrintUsage();
                        return;
                    }

                    i++;
                }
                else
                {
                    if (i != (args.Length - 1))
                    {
                        Console.WriteLine("Invalid parameter " + args[i + 1]);
                        PrintUsage();
                        return;
                    }

                    path = args[i];

                    if (string.IsNullOrWhiteSpace(path)) continue;

                    if (!System.IO.Path.IsPathRooted(path))
                    {
                        path = System.IO.Path.Combine(Environment.CurrentDirectory, path);
                    }
                }
            }

            if (!System.IO.Directory.Exists(path))
            {
                Console.WriteLine("Path " + path + " dose not exisit.");
                PrintUsage();
                return;
            }

            GenerateMenuFile(path, defaultLanguage, languages, languageDependent, sortOrder);

        }

        /// <summary>
        /// Creates the manu.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultLanguage"></param>
        /// <param name="langs"></param>
        /// <param name="langDependent"></param>
        static void GenerateMenuFile(string path, string defaultLanguage, List<string> langs, bool langDependent, string sortOrder)
        {
            DatabaseSpider spider;
            spider = new DatabaseSpider();
            IItemHandler handler = new AliasFileHandler();
            handler.Initialize(defaultLanguage);
            spider.Handles.Add(handler);
            handler = new LinkFileHandler();
            handler.Initialize(defaultLanguage);
            spider.Handles.Add(handler);
            handler = new PxFileHandler();
            handler.Initialize(defaultLanguage);
            spider.Handles.Add(handler);
            
            spider.Builders.Add(new MenuBuilder(langs.ToArray(), langDependent, defaultLanguage) { SortOrder = GetSortOrder(sortOrder) });
            spider.Search(path);

            foreach (var msg in spider.Messages)
            {
                Console.WriteLine(msg.MessageType + " " + msg.Message);
            }         
        }

        private static Func<PCAxis.Paxiom.PXMeta, string, string> GetSortOrder(string sortOrder)
        {
            switch (sortOrder)
            {
                case "matrix":
                    return (meta, path) => meta.Matrix;
                case "file":
                    return (meta, path) => System.IO.Path.GetFileNameWithoutExtension(path);
                case "title":
                default:
                    return (meta, path) => !string.IsNullOrEmpty(meta.Description) ? meta.Description : meta.Title;
            }
        }

        /// <summary>
        /// Prints usage
        /// </summary>
        static void PrintUsage()
        {
            Console.WriteLine("Creates a Menu.xml file for PX-Web ");
            Console.WriteLine();
            Console.WriteLine("MenuBuilder 'dafault language' [/L 'list of languages'] [/N] [/S <title|matrix|file>] 'path'");
            Console.WriteLine();
            Console.WriteLine("dafault language    the default language of the database.");
            Console.WriteLine("/L                  Additional languages other than the default language that");
            Console.WriteLine("                    the menu should be generated for.");
            Console.WriteLine("                    The list should be comma separated");
            Console.WriteLine("/N                  If all files in the database should be included even if");
            Console.WriteLine("                    they are not translated");
            Console.WriteLine("/S                  Sort order could  be one of the following values");
            Console.WriteLine("                    > title");
            Console.WriteLine("                    > matrix");
            Console.WriteLine("                    > file");
            Console.WriteLine("path                Name of the folder for the PX-database");
        }
    }
}
