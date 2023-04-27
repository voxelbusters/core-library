using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    internal class AsyncOperationManager : PrivateSingletonBehaviour<AsyncOperationManager>
    {
        #region Properties

        private List<IAsyncOperationUpdateHandler> ActiveObjects { get; set; }

        private List<IAsyncOperationUpdateHandler> CompletedObjects { get; set; }

        #endregion

        #region Static methods

        public static void ScheduleUpdate(IAsyncOperationUpdateHandler updateHandler)
        {
            Assert.IsArgNotNull(updateHandler, nameof(updateHandler));

            if (TryGetSingleton(out AsyncOperationManager manager))
            {
                manager.ActiveObjects.Add(updateHandler);
            }
        }

        public static void UnscheduleUpdate(IAsyncOperationUpdateHandler updateHandler)
        {
            Assert.IsArgNotNull(updateHandler, nameof(updateHandler));

            // add object to remove list
            if (TryGetSingleton(out AsyncOperationManager manager))
            {
                manager.CompletedObjects.Add(updateHandler);
            }
        }

        #endregion

        #region Unity methods

        private void Update()
        {
            RemoveCompletedObjects();
            TickActiveObjects();
        }

        #endregion

        #region Base class methods

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();

            // Set properties
            ActiveObjects     = new List<IAsyncOperationUpdateHandler>(8);
            CompletedObjects  = new List<IAsyncOperationUpdateHandler>(8);
        }

        #endregion

        #region Private methods

        private void RemoveCompletedObjects()
        {
            // remove completed objects
            for (int iter = 0; iter < CompletedObjects.Count; iter++)
            {
                var     updateHandler   = CompletedObjects[iter];
                ActiveObjects.Remove(updateHandler);
            }

            CompletedObjects.Clear();
        }

        private void TickActiveObjects()
        {
            // call update function for active operations
            for (int iter = 0; iter < ActiveObjects.Count; iter++)
            {
                var     updateHandler   = ActiveObjects[iter];
                updateHandler.Update();
            }
        }
         
        #endregion
    }
}