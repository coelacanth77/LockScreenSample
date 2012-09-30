using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace SampleTask
{
    public sealed class Class1 : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {

            var storage = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (!storage.Containers.ContainsKey("key"))
            {
                storage.CreateContainer("key", Windows.Storage.ApplicationDataCreateDisposition.Always);
            }
            string str = storage.Values["key"] as string;


            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(str));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("Toast Description"));


            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(30);
            toast.Failed += toast_Failed;

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            deferral.Complete();

            return;
        }

        void toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
