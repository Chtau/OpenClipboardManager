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
