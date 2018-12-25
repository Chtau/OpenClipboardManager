using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMHotKey
{
    public sealed class Manager
    {
        #region Singleton
        private static volatile Manager instance;
        private static readonly object syncRoot = new Object();

        private Manager() { }

        public static Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Manager();
                    }
                }

                return instance;
            }
        }
        #endregion

        private List<HotKey> _hotKey = new List<HotKey>();

        public void Add(Enums.Key key, Enums.KeyModifier keyModifiers, Func<string> callback)
        {

        }

        public void Remove()
        {

        }

        public void Clear()
        {

        }
    }
}
