using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMHotKey
{
    public class Manager : IDisposable
    {
        public event EventHandler<HotKey> HotKeyPressed;

        public Manager()
        {
            HotKeyHandler.Instance.HotKeyPressed += Instance_HotKeyPressed;
        }

        private void Instance_HotKeyPressed(object sender, HotKey e)
        {
            HotKeyPressed?.Invoke(this, e);
        }

        public void Add(HotKey hotKey)
        {
            HotKeyHandler.Instance.Register(hotKey);
        }

        public void Add(Enums.Key key, Enums.KeyModifier keyModifiers, Action<HotKey> callback)
        {
            HotKeyHandler.Instance.Register(new HotKey(key, keyModifiers, callback));
        }

        public void Remove(Guid id)
        {
            HotKeyHandler.Instance.Unregister(id);
        }

        public void Clear()
        {
            HotKeyHandler.Instance.UnregisterAll();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HotKeyHandler.Instance.HotKeyPressed -= Instance_HotKeyPressed;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
