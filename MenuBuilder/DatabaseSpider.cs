using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuBuilder
{
    /// <summary>
    /// DatabaseSpider iterates through a file database and ...TODO
    /// </summary>
    public class DatabaseSpider
    {
        public DatabaseSpider()
        {
            logger = new DatabaseLogger(LogMessage);
        }

        private List<IItemHandler> _handlers = new List<IItemHandler>();
        public List<IItemHandler> Handles { get { return _handlers; } }

        private List<IDatabaseBuilder> _builders = new List<IDatabaseBuilder>();
        public List<IDatabaseBuilder> Builders { get { return _builders; } }

        private List<DatabaseMessage> _messages = new List<DatabaseMessage>();
        public List<DatabaseMessage> Messages { get { return _messages; } }

        private DatabaseLogger logger;
        private void LogMessage(DatabaseMessage msg)
        {
            Messages.Add(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPath">The root path of the database</param>
        public void Search(string startPath)
        {
            //Sort handlers after priority
            Handles.OrderBy(x => x.Priority);

            foreach (var builder in Builders)
            {
                builder.BeginBuild(startPath, logger);
            }

            SearchRecursive(startPath);

            foreach (var builder in Builders)
            {
                builder.EndBuild(startPath);
            }
        }

        /// <summary>
        /// Searches recursively the file database
        /// </summary>
        /// <param name="path">The path to search</param>
        private void SearchRecursive(string path)
        {
            //allocates a new menu
            SignalStartNewLevel(path);

            foreach (var item in System.IO.Directory.GetFiles(path))
            {
                IItemHandler handler = GetHandler(item);
                if (handler != null)
                {
                    object obj = handler.Handle(item, logger);
                    SignalNewItem(obj, item);
                }
                else
                {
                    //TODO LOGG
                }
            }

            foreach (var item in System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly))
            {
                SearchRecursive(item);
            }

            //
            SignalStopNewLevel(path);
        }

        private IItemHandler GetHandler(string path)
        {
            foreach (var item in Handles)
            {
                if (item.CanHandle(path)) return item;
            }
            //Return 
            return null;
        }

        private void SignalStartNewLevel(string path)
        {
            //string name = System.IO.Path.GetFileName(path);
            foreach (var item in Builders)
            {
                //item.BeginNewLevel(name);
                item.BeginNewLevel(path);
            }
        }

        private void SignalStopNewLevel(string path)
        {
            //string name = System.IO.Path.GetDirectoryName(path);
            foreach (var item in Builders)
            {
                //item.EndNewLevel(name);
                item.EndNewLevel(path);
            }
        }

        private void SignalNewItem(object newItem, string path)
        {
            foreach (var item in Builders)
            {
                item.NewItem(newItem, path);
            }
        }
    }
}
