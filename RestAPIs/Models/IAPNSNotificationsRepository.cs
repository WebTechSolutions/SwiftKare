using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotiProject.Models
{
    public interface IAPNSNotificationsRepository
    {
        void sendNotification(APNSNotification model, String[] devicesIds, String cb, byte[] certificate, String password);        
    }
}
