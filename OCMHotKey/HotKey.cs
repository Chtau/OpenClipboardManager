using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMHotKey
{
    public class HotKey
    {
        public Enums.Key Key { get; private set; }
        public Enums.KeyModifier KeyModifiers { get; private set; }
        public Action<HotKey> Action { get; private set; }
        public string UniqueName { get; private set; }
        public Guid Id { get; private set; }

        public HotKey(Enums.Key key, Enums.KeyModifier keyModifiers, Action<HotKey> action, string uniqueName = null)
        {
            Id = Guid.NewGuid();
            Key = key;
            Action = action;
            KeyModifiers = keyModifiers;
            if (string.IsNullOrWhiteSpace(uniqueName))
                uniqueName = Id.ToString().ToUpper();
            UniqueName = uniqueName;
        }
    }
}
