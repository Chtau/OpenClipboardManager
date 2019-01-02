using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Favorites
{
    public class FavoriteItemViewModel
    {
        public DAL.Models.Favorite Favorite { get; private set; }
        public DAL.Models.FavoriteContentText FavoriteContentText { get; private set; }
        public DAL.Models.FavoriteContentImage FavoriteContentImage { get; private set; }
        public DAL.Models.FavoriteContentFile FavoriteContentFile { get; private set; }

        public bool IsTextContent
        {
            get
            {
                if (Favorite != null)
                {
                    if (Favorite.Type == DAL.Models.Favorite.ContentType.File || Favorite.Type == DAL.Models.Favorite.ContentType.Text)
                        return true;
                }
                return false;
            }
        }

        public string TextContent
        {
            get
            {
                if (FavoriteContentText != null)
                    return FavoriteContentText.Content;
                else if (FavoriteContentFile != null)
                    return FavoriteContentText.Content;
                return null;
            }
        }

        public byte[] ImageContent
        {
            get
            {
                if (FavoriteContentImage != null)
                    return FavoriteContentImage.Content;
                return null;
            }
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentText favoriteContentText)
        {
            Favorite = favorite;
            FavoriteContentText = favoriteContentText;
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentImage favoriteContentImage)
        {
            Favorite = favorite;
            FavoriteContentImage = favoriteContentImage;
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentFile favoriteContentFile)
        {
            Favorite = favorite;
            FavoriteContentFile = favoriteContentFile;
        }
    }
}
