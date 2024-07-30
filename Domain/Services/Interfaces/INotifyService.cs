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
        Task NotifyLastAddEventUsersAsync(string notifyMessage, CancellationToken token);
        Task NotifyEventChangingUsersAsync(ScienceEvent sEvent, string notifyMessage, CancellationToken token);
        Task NotifySubUsersAsync(string notifyMessage, CancellationToken token);
        Task NotifyAllUsersAsync(string notifyMessage, CancellationToken token);
        Task NotifyAdminsAsync(string notifyMessage, CancellationToken token);
        Task<string> GetInfoAboutTgUserAsync(long chatId, CancellationToken token);
	}
}
