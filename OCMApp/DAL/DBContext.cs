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
        internal SQLiteAsyncConnection DB = null;

        public DBContext(string path)
        {
            DB = new SQLiteAsyncConnection(path);
            Task.Run(OnBuildModel).Wait();
        }

        private async Task OnBuildModel()
        {
            await DB.CreateTableAsync<Models.ClipText>();
            await DB.CreateTableAsync<Models.ClipImage>();
            await DB.CreateTableAsync<Models.ClipFile>();
        }

        public void InsertClipText(Models.ClipText clipText)
        {
            DB.InsertAsync(clipText);
        }

        public Task<List<Models.ClipText>> GetClipText()
        {
            return DB.Table<Models.ClipText>().ToListAsync();
        }

        public Task<List<Models.ClipImage>> GetClipImage()
        {
            return DB.Table<Models.ClipImage>().ToListAsync();
        }

        public Task<List<Models.ClipFile>> GetClipFile()
        {
            return DB.Table<Models.ClipFile>().ToListAsync();
        }
    }
}
