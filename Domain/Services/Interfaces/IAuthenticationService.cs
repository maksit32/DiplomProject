﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<string> AuthenticateUserAsync(WebsiteLoginUserDto login, CancellationToken ct);
	}
}
