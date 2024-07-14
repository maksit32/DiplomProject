using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



//уведомления пользователей из БД о событиях или изменениях
namespace Domain.Services.Interfaces
{
    public interface INotifyService
    {
        Task NotifyLastAddEventUsersAsync(string notifyMessage);
        Task NotifyEventChangingUsersAsync(ScienceEvent sEvent, string notifyMessage);
        Task NotifySubUsersAsync(string notifyMessage);
        Task NotifyAllUsersAsync(string notifyMessage);
        Task NotifyAdminsAsync(string notifyMessage);
    }
}
