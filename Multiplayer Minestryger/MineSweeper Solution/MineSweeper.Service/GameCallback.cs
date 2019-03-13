using System.Collections.Generic;
using System.Threading;

namespace MineSweeper.Service
{
    public class GameCallback
    {
        private static Dictionary<int, GameCallback> gameCallbackInstances = new Dictionary<int, GameCallback>();
        private static ReaderWriterLock gameCallbackinstanceLock = new ReaderWriterLock();
        private static readonly int LockTimeout = 50000;

        private List<GameService> serviceInstances;
        private List<GameService> blackList;
        private ReaderWriterLock serviceInstanceLock;

        public static GameCallback GetGameCallbackInstance(int gameId)
        {
            GameCallback gameCallback = null;
            gameCallbackinstanceLock.AcquireReaderLock(LockTimeout);
            if (gameCallbackInstances.TryGetValue(gameId, out gameCallback))
            {
                gameCallbackinstanceLock.ReleaseReaderLock();
            }
            else
            {
                gameCallbackinstanceLock.UpgradeToWriterLock(LockTimeout);
                gameCallback = new GameCallback();
                gameCallbackInstances.Add(gameId, gameCallback);
                gameCallbackinstanceLock.ReleaseWriterLock();
            }
            return gameCallback;
        }

        private GameCallback()
        {
            serviceInstances = new List<GameService>();
            blackList = new List<GameService>();
            serviceInstanceLock = new ReaderWriterLock();
        }

        public void AddCallback(GameService serviceInstance)
        {
            serviceInstanceLock.AcquireWriterLock(LockTimeout);
            serviceInstances.Add(serviceInstance);
            serviceInstanceLock.ReleaseWriterLock();
        }

        public void Update(bool lockList)
        {
            ServiceInstanceReaderLock(lockList);
            foreach (GameService serviceInstance in serviceInstances)
            {
                if (!blackList.Contains(serviceInstance))
                {
                    serviceInstance.Update();
                }
            }
            ServiceInstanceReaderUnlock(lockList);
        }

        public void SendConsoleMessageToAll(string message, bool lockList)
        {
            ServiceInstanceReaderLock(lockList);
            foreach (GameService serviceInstance in serviceInstances)
            {
                if (!blackList.Contains(serviceInstance))
                {
                    serviceInstance.SendConsoleMessage(message);
                }
            }
            ServiceInstanceReaderUnlock(lockList);
        }

        public void Disconnect(GameService serviceInstance)
        {
            serviceInstanceLock.AcquireWriterLock(LockTimeout);
            blackList.Add(serviceInstance);
            serviceInstanceLock.ReleaseWriterLock();
        }

        public void CheckAllPlayersReady(bool lockList)
        {
            ServiceInstanceReaderLock(lockList);
            foreach (GameService serviceInstance in serviceInstances)
            {
                if (!blackList.Contains(serviceInstance))
                {
                    serviceInstance.CheckAllPlayerReadiness();
                }
            }
            ServiceInstanceReaderUnlock(lockList);
        }

        private void ServiceInstanceReaderLock(bool lockList)
        {
            if (lockList)
            {
                serviceInstanceLock.AcquireReaderLock(LockTimeout);
            }
        }

        private void ServiceInstanceReaderUnlock(bool unlockList)
        {
            if (unlockList)
            {
                serviceInstanceLock.ReleaseReaderLock();
            }
        }
    }
}
