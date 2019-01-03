using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class Favorite
    {
        public enum ContentType
        {
            Text,
            Image,
            File
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        public int Key { get; set; }
        public int Modifier1 { get; set; }
        public int Modifier2 { get; set; }
        public Guid FavoriteContentId { get; set; }
        public ContentType Type { get; set; }

        public Favorite()
        {
            Id = Guid.NewGuid();
        }

        public OCMHotKey.Enums.Key GetKey()
        {
            return (OCMHotKey.Enums.Key)Key;
        }

        public OCMHotKey.Enums.KeyModifier GetModifier()
        {
            OCMHotKey.Enums.KeyModifier keyModifier = OCMHotKey.Enums.KeyModifier.None;
            bool hasMod1 = false;
            if (Enum.TryParse(Modifier1.ToString(), out OCMHotKey.Enums.KeyModifier modi1))
            {
                hasMod1 = true;
                keyModifier = modi1;
            }
            if (Enum.TryParse(Modifier2.ToString(), out OCMHotKey.Enums.KeyModifier modi2))
            {
                if (hasMod1)
                {
                    keyModifier = modi1 | modi2;
                }
                else
                    keyModifier = modi2;
            }
            return keyModifier;
        }
    }
}
