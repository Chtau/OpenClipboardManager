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

        public Task<List<Models.Summary>> GetSummary()
        {
            return DB.QueryAsync<Models.Summary>("SELECT ApplicationName as Application, SUM(1) as Total, SUM(1) as Text, 0 as Image, 0 as File FROM ClipText GROUP BY ApplicationName"
                + " UNION SELECT ApplicationName as Application, SUM(1) as Total, 0 as Text, SUM(1) as Image, 0 as File FROM ClipImage GROUP BY ApplicationName"
                + " UNION SELECT ApplicationName as Application, SUM(1) as Total, 0 as Text, 0 as Image, SUM(1) as File FROM ClipFile GROUP BY ApplicationName");
        }
    }
}
