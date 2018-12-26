using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Helper
{
    public static class ComboBoxBindingModelBuilder
    {
        public static List<KeyValuePair<int, string>> FromEnum(Type @enum)
        {
            List<KeyValuePair<int, string>> returnValue = new List<KeyValuePair<int, string>>();
            var values = Enum.GetValues(@enum);

            foreach (int eValue in values)
            {
                returnValue.Add(
                    new KeyValuePair<int, string>(
                        eValue, 
                        Internal.Global.Instance.Localize.GetText(Enum.GetName(@enum, eValue))
                    )
                );
            }
            return returnValue;
        }

    }
}
