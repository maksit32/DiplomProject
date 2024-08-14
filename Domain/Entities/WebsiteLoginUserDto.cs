using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class WebsiteLoginUserDto
	{
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public WebsiteLoginUserDto(string phoneNumber, string password)
        {
            PhoneNumber = phoneNumber;
            Password = password;
        }
    }
}
