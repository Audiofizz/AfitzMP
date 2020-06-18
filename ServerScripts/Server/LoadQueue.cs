using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public static class LoadQueue
    {
        public delegate void loadQueue(int toClient);

        private static loadQueue queueToLoad;

        public static void PreformLoadQueue(int toClient)
        {
            queueToLoad?.Invoke(toClient);
        }

        public static void AddLoadToQueue(loadQueue action)
        {
            queueToLoad += action;
        }
        public static void RemoveLoadFromQueue(loadQueue action)
        {
            queueToLoad -= action;
        }
    }
}
