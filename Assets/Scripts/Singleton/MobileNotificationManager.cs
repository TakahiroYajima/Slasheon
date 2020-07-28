using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Slasheon.Notification
{
    public class MobileNotificationManager : SingletonMonoBehaviour<MobileNotificationManager>
    {
        private string m_channelId = "channelId";

        private void Awake()
        {
#if UNITY_ANDROID
        // 通知用のチャンネルを作成する
        var c = new AndroidNotificationChannel
        {
            Id = m_channelId,
            Name = "【ここにチャンネル名】",
            Importance = Importance.High,
            Description = "【ここに説明文】",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
#elif UNITY_IOS

#endif
        }

        /// <summary>
        /// 通知を設定
        /// </summary>
        public void SetNotification(string title, string message, string smallIcon, string largeIcon, DateTime dateTime)
        {
#if UNITY_ANDROID
        // 通知を送信する
        var n = new AndroidNotification
        {
            Title = title,
            Text = message,
            SmallIcon = "icon_0",
            LargeIcon = "icon_1",
            FireTime = DateTime.Now.AddSeconds(10), // 10 秒後に通知
        };
        AndroidNotificationCenter.SendNotification(n, m_channelId);
    
#elif UNITY_IOS
            Debug.Log("iosの通知設定");
            var n = new iOSNotification
            {
                Title = title,
                Body = message,
                ShowInForeground = true,
                Badge = 1,
                Trigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = new TimeSpan(0, 0, 10),
                    Repeats = false
                }
            };
#endif
        }
    }
}