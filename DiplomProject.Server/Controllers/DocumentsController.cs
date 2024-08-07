using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class DocumentsController : ControllerBase
	{

		public DocumentsController() 
		{ 

		}

	}
}
