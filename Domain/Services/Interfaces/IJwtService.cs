﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface IJwtService
	{
		string GenerateJwtToken(string phoneNumber);
	}
}
