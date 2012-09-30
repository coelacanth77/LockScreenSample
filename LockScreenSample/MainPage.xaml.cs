using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LockScreenSample
{
    /// <summary>
    /// ロックスクリーンとタイマートリガーのサンプルアプリ
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public BackgroundTaskRegistration regist;

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        void regist_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
        }

        void regist_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
        }

        /// <summary>
        /// バッジの追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addBadgeButton_Click(object sender, RoutedEventArgs e)
        {
            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent(6);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());
        }

        /// <summary>
        /// バッジのクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearBadgeButton_Click(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }

        /// <summary>
        /// メッセージの追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addMessageButton_Click(object sender, RoutedEventArgs e)
        {
            ITileWideSmallImageAndText03 tileContent = TileContentFactory.CreateTileWideSmallImageAndText03();
            tileContent.TextBodyWrap.Text = "このメッセージはタイルとロックスクリーンに表示されます。";
            tileContent.Image.Src = "ms-appx:///Assets/24.png";
            tileContent.RequireSquareContent = false;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
        }

        /// <summary>
        /// メッセージのクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMessageButton_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        private async void addTimerTrigger_Click(object sender, RoutedEventArgs e)
        {
            // 先に登録されたタスクをいったん削除する
            if (BackgroundTaskRegistration.AllTasks.Count > 1)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    task.Value.Unregister(true);
                }
            }

            TimeTrigger trigger = new TimeTrigger(15, true);
            var taskBuilder = new BackgroundTaskBuilder();

            taskBuilder.Name = "SampleTask";
            taskBuilder.TaskEntryPoint = "SampleTask.Class1";
            taskBuilder.SetTrigger(trigger);

            regist = taskBuilder.Register();

            // ロック画面に表示する許可を求めるダイアログを表示する
            await BackgroundExecutionManager.RequestAccessAsync();

            regist.Completed += regist_Completed;
            regist.Progress += regist_Progress;

            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (!storage.Containers.ContainsKey("key"))
            {
                storage.CreateContainer("key", Windows.Storage.ApplicationDataCreateDisposition.Always);
            }
            storage.Values["key"] = "Time Trigger Sample";
        }
    }
}
