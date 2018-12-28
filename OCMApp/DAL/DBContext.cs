using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL
{
    public class DBContext
    {
        internal SQLiteConnection DB = null;

        public DBContext(string path)
        {
            DB = new SQLiteConnection(path);
            OnBuildModel();
        }

        private void OnBuildModel()
        {
            DB.CreateTable<Models.ClipText>();
            DB.CreateTable<Models.ClipImage>();
            DB.CreateTable<Models.ClipFile>();
        }

        public void InsertClipText(Models.ClipText clipText)
        {
            DB.Insert(clipText);
        }

        public IQueryable<Models.ClipText> GetClipText()
        {
            return DB.Table<Models.ClipText>().AsQueryable();
        }
    }
}
