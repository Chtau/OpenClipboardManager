using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace OCMHotKey
{
    internal sealed class HotKeyHandler : IDisposable
    {
        #region Singleton
        private static volatile HotKeyHandler instance;
        private static readonly object syncRoot = new Object();

        private HotKeyHandler()
        {
            var startupTcs = new TaskCompletionSource<object>();

            var state = System.Threading.Thread.CurrentThread.GetApartmentState();
            if (state == System.Threading.ApartmentState.STA)
                useOwnThread = false;

            if (useOwnThread)
            {
                Task.Run(() =>
                {
                    ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
                    wpfApp = new Application();
                    wpfApp.Startup += (s, e) => startupTcs.SetResult(null);
                    wpfApp.Run();
                });

                startupTcs.Task.Wait();
            } else
            {
                ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            }
        }

        public static HotKeyHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new HotKeyHandler();
                    }
                }

                return instance;
            }
        }
        #endregion

        private Application wpfApp;
        private bool useOwnThread = true;

        public event EventHandler<HotKey> HotKeyPressed;

        private Dictionary<int, HotKey> hotKeyItems = new Dictionary<int, HotKey>();

        private const int WmHotKey = 0x0312;
        
        public bool Register(HotKey hotKey)
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey((Key)hotKey.Key);
            int registerId = virtualKeyCode + ((int)hotKey.KeyModifiers * 0x10000);
            if (!hotKeyItems.ContainsKey(registerId) && !hotKeyItems.Any(x => x.Value.UniqueName.ToUpper() == hotKey.UniqueName.ToUpper()))
            {
                bool result = false;
                if (useOwnThread)
                {
                    wpfApp.Dispatcher.Invoke(() =>
                    {
                        result = Nativ.RegisterHotKey(IntPtr.Zero, registerId, (UInt32)hotKey.KeyModifiers, (UInt32)virtualKeyCode);
                    });
                } else
                {
                    result = Nativ.RegisterHotKey(IntPtr.Zero, registerId, (UInt32)hotKey.KeyModifiers, (UInt32)virtualKeyCode);
                }

                if (!hotKeyItems.ContainsKey(registerId))
                    hotKeyItems.Add(registerId, hotKey);
                return result;
            }
            return false;
        }

        public bool Unregister(string uniqueName)
        {
            if (hotKeyItems.Any(x => x.Value.UniqueName.ToUpper() == uniqueName.ToUpper()))
                return Unregister(hotKeyItems.First(x => x.Value.UniqueName.ToUpper() == uniqueName.ToUpper()).Value.Id);
            return false;
        }

        public bool Unregister(Guid id)
        {
            bool result = false;
            if (hotKeyItems.Any(x => x.Value.Id == id))
            {
                int registerId = hotKeyItems.First(x => x.Value.Id == id).Key;
                if (useOwnThread)
                {
                    wpfApp.Dispatcher.Invoke(() =>
                    {
                        result = Nativ.UnregisterHotKey(IntPtr.Zero, registerId);
                    });
                } else
                {
                    result = Nativ.UnregisterHotKey(IntPtr.Zero, registerId);
                }
                hotKeyItems.Remove(registerId);
            }
            return result;
        }

        public bool UnregisterAll()
        {
            bool result = true;
            List<int> removedKeys = new List<int>();
            foreach (var item in hotKeyItems)
            {
                wpfApp.Dispatcher.Invoke(() =>
                {
                    if (Nativ.UnregisterHotKey(IntPtr.Zero, item.Key))
                    {
                        removedKeys.Add(item.Key);
                    } else
                    {
                        result = false;
                    }
                });
            }
            if (!result)
            {
                foreach (var item in removedKeys)
                {
                    hotKeyItems.Remove(item);
                }
            } else
                hotKeyItems.Clear();
            return result;
        }

        public IEnumerable<HotKey> GetItems()
        {
            return from x in hotKeyItems
                   select x.Value;
        }

        private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    if (hotKeyItems.TryGetValue((int)msg.wParam, out HotKey hotKey))
                    {
                        if (hotKey.Action != null)
                        {
                            hotKey.Action.Invoke(hotKey);
                        }
                        HotKeyPressed?.Invoke(this, hotKey);
                        handled = true;
                    }
                }
            }
        }

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
        
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    UnregisterAll();
                }
                _disposed = true;
            }
        }
    }
}
