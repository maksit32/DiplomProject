using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class TelegramUserDto
	{
        public long ChatId { get; set; }
        public string Password { get; set; }

        public TelegramUserDto(long chatId, string password)
        {
            ChatId = chatId;
            Password = password;
        }
    }
}
