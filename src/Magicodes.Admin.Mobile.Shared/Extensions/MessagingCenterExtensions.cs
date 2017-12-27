using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Magicodes.Admin.Extensions
{
    public static class MessagingCenterExtensions
    {
        /// <summary>
        /// Subscribes to a message with preventing multiple subscription.
        /// </summary>
        public static void SubscribeSafe<TArgs>(this IMessagingCenter messagingCenter,
            object subscriber,
            string message,
            Action<TArgs> callback,
            TArgs source = null) where TArgs : class
        {
            MessagingCenter.Unsubscribe<TArgs>(subscriber, message);
            MessagingCenter.Subscribe(subscriber, message, callback, source);
        }

        /// <summary>
        /// Subscribes to a message with preventing multiple subscription with async calback
        /// </summary>
        public static void SubscribeSafe<TArgs>(this IMessagingCenter messagingCenter,
            object subscriber,
            string message,
            Func<TArgs, Task> callback,
            TArgs source = null) where TArgs : class
        {
            MessagingCenter.Unsubscribe<TArgs>(subscriber, message);
            MessagingCenter.Subscribe(subscriber, message, async (arg) =>
            {
                await callback(arg);
            }, source);
        }


        /// <summary>
        /// Subscribes to a message with preventing multiple subscription.
        /// </summary>
        public static void SubscribeSafe<TSender, TArgs>(this IMessagingCenter messagingCenter,
            object subscriber,
            string message,
            Action<TSender, TArgs> callback,
            TSender source = null) where TSender : class
        {
            MessagingCenter.Unsubscribe<TSender>(subscriber, message);
            MessagingCenter.Subscribe(subscriber, message, callback, source);
        }
    }
}