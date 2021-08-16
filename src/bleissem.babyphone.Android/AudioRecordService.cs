using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using bleissem.babyphone.Core;
using bleissem.babyphone.Core.Events;
using bleissem.babyphone.Core.Messages;
using DryIoc;
using Java.Util.Concurrent;
using Prism.DryIoc;
using Prism.Events;

namespace bleissem.babyphone.Droid
{
    [Service]
    public class AudioRecordService : Service
    {
        private readonly IEventAggregator _eventAggregator;
        public AudioRecordService()
        {
            _eventAggregator = App.Current.Container.GetContainer().Resolve<IEventAggregator>();
        }

        private Handler _handler;
        private Action _runnable;
        private bool isStarted;
        private int DELAY_BETWEEN_LOG_MESSAGES = 500;
        private int NOTIFICATION_SERVICE_ID = 1001;
        private int NOTIFICATION_AlARM_ID = 1002;
        private string NOTIFICATION_CHANNEL_ID = "1003";
        private string NOTIFICATION_CHANNEL_NAME = "MyChannel";

        public override void OnCreate()
        {
            base.OnCreate();

            _handler = new Handler();
            Random r = new Random();
            _runnable = new Action(() =>
            {
                while (isStarted)
                {

                    _eventAggregator.GetEvent<AudioRecordEvent>().Publish(new AudioRecordMessage()
                    {
                        Level = r.Next()
                    });
                }
            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (!isStarted)
            {
                CreateNotificationChannel();
                ShowServiceStarted();

                _handler.PostDelayed(_runnable, DELAY_BETWEEN_LOG_MESSAGES);
                isStarted = true;
            }
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            _handler.RemoveCallbacks(_runnable);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(NOTIFICATION_SERVICE_ID);
            isStarted = false;
            _handler.Dispose();
            _handler = null;
            base.OnDestroy();
        }

        private void CreateNotificationChannel()
        {
            NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.None);
            notificationChannel.EnableLights(false);
            notificationChannel.EnableVibration(false);
            notificationChannel.SetShowBadge(false);

            NotificationManager notificationManager = (NotificationManager)this.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(notificationChannel);
        }

        private NotificationCompat.Builder CreateNotificationBuilder(string message)
        {
            return new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                   .SetDefaults((int)NotificationDefaults.Lights)
                   .SetPriority((int)NotificationImportance.Low)
                   .SetSmallIcon(Resource.Drawable.Icon)
                   .SetSound(null, 0)
                   .SetChannelId(NOTIFICATION_CHANNEL_ID)
                   .SetPriority(NotificationCompat.PriorityDefault)
                   .SetAutoCancel(false)
                   .SetContentTitle("Babyphone")
                   .SetContentText(message)
                   .SetVisibility((int)NotificationVisibility.Secret)
                   .SetOngoing(true);
        }

        private void ShowServiceStarted()
        {
            var builder = CreateNotificationBuilder("babyphone service started");

            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
            StartForeground(NOTIFICATION_SERVICE_ID, builder.Build());
        }        
    }
}