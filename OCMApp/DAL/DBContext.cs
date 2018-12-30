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

        public void InsertClipImage(Models.ClipImage clip)
        {
            DB.InsertAsync(clip);
        }

        public void InsertClipFile(Models.ClipFile clip)
        {
            DB.InsertAsync(clip);
        }

        public Task<List<Models.ClipText>> GetClipText()
        {
            return DB.Table<Models.ClipText>().OrderByDescending(x => x.DateCreated).ToListAsync();
        }

        public Task<List<Models.ClipImage>> GetClipImage()
        {
            return DB.Table<Models.ClipImage>().OrderByDescending(x => x.DateCreated).ToListAsync();
        }

        public Task<List<Models.ClipFile>> GetClipFile()
        {
            return DB.Table<Models.ClipFile>().OrderByDescending(x => x.DateCreated).ToListAsync();
        }

        public Task<List<Models.Summary>> GetSummary()
        {
            return DB.QueryAsync<Models.Summary>("SELECT Application, SUM(Total) as Total, SUM(Text) as Text, SUM(Image) as Image, SUM(File) as File FROM ( " +
                "SELECT ApplicationName as Application, SUM(1) as Total, SUM(1) as Text, 0 as Image, 0 as File FROM ClipText GROUP BY ApplicationName"
                + " UNION SELECT ApplicationName as Application, SUM(1) as Total, 0 as Text, SUM(1) as Image, 0 as File FROM ClipImage GROUP BY ApplicationName"
                + " UNION SELECT ApplicationName as Application, SUM(1) as Total, 0 as Text, 0 as Image, SUM(1) as File FROM ClipFile GROUP BY ApplicationName"
                + " ) GROUP BY Application");
        }

        public async Task<Models.LastClip> GetLastValues()
        {
            var text = await DB.Table<Models.ClipText>().OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            var image = await DB.Table<Models.ClipImage>().OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            var file = await DB.Table<Models.ClipFile>().OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            return new Models.LastClip(text, image, file);
        }
    }
}
