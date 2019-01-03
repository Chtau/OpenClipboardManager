using Serilog;
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
            await DB.CreateTableAsync<Models.Favorite>();
            await DB.CreateTableAsync<Models.FavoriteContentText>();
            await DB.CreateTableAsync<Models.FavoriteContentImage>();
            await DB.CreateTableAsync<Models.FavoriteContentFile>();
        }

        #region Clip insert
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
        #endregion

        #region Clip delete
        public async Task DeleteClipText(Models.ClipText clipText)
        {
            await DB.DeleteAsync(clipText);
        }

        public async Task DeleteClipImage(Models.ClipImage clip)
        {
            await DB.DeleteAsync(clip);
        }

        public async Task DeleteClipFile(Models.ClipFile clip)
        {
            await DB.DeleteAsync(clip);
        }
        #endregion

        #region Clip get
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
        #endregion

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

        #region Favorites
        public async Task<List<Favorites.FavoriteItemViewModel>> Favorites()
        {
            var retVal = new List<Favorites.FavoriteItemViewModel>();
            var fav = await DB.Table<DAL.Models.Favorite>().ToListAsync();
            foreach (DAL.Models.Favorite item in fav)
            {
                if (item.Type == Models.Favorite.ContentType.Text)
                {
                    retVal.Add(new OCMApp.Favorites.FavoriteItemViewModel(item, await DB.Table<Models.FavoriteContentText>().FirstOrDefaultAsync(x => x.Id == item.FavoriteContentId)));
                } else if (item.Type == Models.Favorite.ContentType.Image)
                {
                    retVal.Add(new OCMApp.Favorites.FavoriteItemViewModel(item, await DB.Table<Models.FavoriteContentImage>().FirstOrDefaultAsync(x => x.Id == item.FavoriteContentId)));
                } else if (item.Type == Models.Favorite.ContentType.File)
                {
                    retVal.Add(new OCMApp.Favorites.FavoriteItemViewModel(item, await DB.Table<Models.FavoriteContentFile>().FirstOrDefaultAsync(x => x.Id == item.FavoriteContentId)));
                }
            }
            return retVal;
        }

        public async Task<bool> InsertFavorite(Models.ClipText clipText)
        {
            try
            {
                if (clipText != null)
                {
                    var itemContent = new Models.FavoriteContentText(clipText.Value);
                    await DB.InsertAsync(itemContent);
                    var item = new Models.Favorite
                    {
                        Type = Models.Favorite.ContentType.Text,
                        FavoriteContentId = itemContent.Id,
                    };
                    return true;
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to insert new Text Favorite");
            }
            return false;
        }

        public async Task<bool> InsertFavorite(Models.ClipImage clipImage)
        {
            try
            {
                if (clipImage != null)
                {
                    var itemContent = new Models.FavoriteContentImage(clipImage.Value, clipImage.FormatType);
                    await DB.InsertAsync(itemContent);
                    var item = new Models.Favorite
                    {
                        Type = Models.Favorite.ContentType.Text,
                        FavoriteContentId = itemContent.Id,
                    };
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to insert new Image Favorite");
            }
            return false;
        }

        public async Task<bool> InsertFavorite(Models.ClipFile clipFile)
        {
            try
            {
                if (clipFile != null)
                {
                    var itemContent = new Models.FavoriteContentFile(clipFile.GetListValue());
                    await DB.InsertAsync(itemContent);
                    var item = new Models.Favorite
                    {
                        Type = Models.Favorite.ContentType.Text,
                        FavoriteContentId = itemContent.Id,
                    };
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to insert new File Favorite");
            }
            return false;
        }

        public async Task<bool> UpdateFavorite(Favorites.FavoriteItemViewModel favorite)
        {
            try
            {
                if (favorite != null)
                {
                    if (favorite.Favorite.Type == Models.Favorite.ContentType.Text)
                        await DB.UpdateAsync(favorite.FavoriteContentText);
                    else if (favorite.Favorite.Type == Models.Favorite.ContentType.Image)
                        await DB.UpdateAsync(favorite.FavoriteContentImage);
                    else if (favorite.Favorite.Type == Models.Favorite.ContentType.File)
                        await DB.UpdateAsync(favorite.FavoriteContentFile);
                    await DB.UpdateAsync(favorite.Favorite);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update Favorite view model");
            }
            return false;
        }
        #endregion
    }
}
