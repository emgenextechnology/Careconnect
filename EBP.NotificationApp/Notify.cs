using EBP.NotificationApp.Database;
using EBP.NotificationApp.Entity;
using EBP.NotificationApp.Enums;
using EBP.NotificationApp.Helper;
using EBP.NotificationApp.Helper.Notification;
using EBP.NotificationApp.Repository;
using EBP.NotificationApp.Response;
using EBP.NotificationApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace EBP.NotificationApp
{
    public partial class Notify : Form
    {
        public Notify()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DialogResult dialogueResult = MessageBox.Show("Do you want to start the scheduler?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogueResult == DialogResult.Yes)
            {
                SendNotification();
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = (int)(float.Parse(ConfigurationManager.AppSettings["ImportInterval"]) * 60000);
                timer.Tick += timer_Tick;
                timer.Start();
            }

        }
        void timer_Tick(object sender, EventArgs e)
        {
            new Exception("Timer tick").Log();
            SendNotification();
        }

        private void SendNotification()
        {
            new Exception("Process Started").Log();

            CRMStagingEntities n = new CRMStagingEntities();
            var response = new List<EntityNotificationUsers>();
            var repository = new RepositoryNotification();

            // Get All Notifications which have IspushSent Status true
            response = repository.GetAllUserNotifications();
            
            foreach (var item in response)
            {
                // Get Details of Notification and user for pushing 
                var dataNotification = repository.GetNotificationById(item.NotificationId);

                if (dataNotification.Model == null)
                    break;

                long NotificationId = dataNotification.Model.NotificationId;
                int ToUserId = dataNotification.Model.UserId;
                int TargetId = dataNotification.Model.TargetId;
                int TargetType = dataNotification.Model.TargetTypeId;
                string Message = dataNotification.Model.Message;
                //string Subject=null;
                //if(TargetType==(int)NoteType.Task)

                //{
                //var TaskModel = repository.GetTaskById(TargetId);
                //Subject = TaskModel.Model.Subject;
                //}
                //get Notification UnReadCount

                var unReadNotify = repository.GetUnReadNotificationCount(item.UserId);

                int unReadCount = unReadNotify.Model != null ? unReadNotify.Model.NotificationCount : 0;
                //Payload Format
                var notifyObj = new ApplePushPayLoad
                {
                    aps = new aps
                    {
                        alert = Message,
                        badge = unReadCount == 0 ? 1 : unReadCount,
                        sound = "default"
                    },
                    NotificationId = NotificationId,
                    TargetId = TargetId,
                    TargetType = TargetType
                };


                //  For Getting User DeviceId Details
                var repositoryUsers = new RepositoryUsers();
                var userDetails = repositoryUsers.GetUserDeviceIdById(item.UserId);
                if (userDetails.Model != null)
                {
                    var toDeviceId = userDetails.Model.DeviceId;

                    if (toDeviceId == null)
                        continue;

                    // Push Message
                    string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                    string stringPath = Path.Combine(appPath, "AppleCertificate", "Certificates.p12");
                    new Exception("certificate path:"+stringPath).Log();
                    //NotificationHelper.pushMessage(toDeviceId, notifyObj, stringPath);

                    bool pushStatus = NotificationHelper.pushMessage(toDeviceId, notifyObj, stringPath);
                    if (pushStatus == true)
                    {
                        // Update Notification IsPushStatus
                        var updatePushStatus = repository.UpdateNotificationPushStatus(NotificationId, ToUserId);
                        new EventsLog().Write("Push success");
                        new Exception("Push success:- \tDeviceId:" + toDeviceId + "\tUser: "+ ToUserId +"\tMessage:" +Message).Log();
                    }
                    else
                        new Exception("Push failed:- \tDeviceId:" + toDeviceId + "\tUser: " + ToUserId + "\tMessage:" + Message).Log();
                    
                }
                // var updatePushStatus = repository.UpdateNotificationPushStatus(NotificationId, ToUserId);

            }

        }

    }
}
