using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Domain.Services.Interfaces
{
	public interface IFillDataService
	{
		Task<string> FillSNODataAsync(string lowerCaseMessage, long chatId, CancellationToken token);
		Task<string> FillSMUDataAsync(string lowerCaseMessage, long chatId, CancellationToken token);
	}
}