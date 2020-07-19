using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyProject.Domain;
using MyProject.Domain.Core;
using MyProject.Web.Core;

namespace SafeBaby.Web.Core
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public abstract class ApiControllerBase : ControllerBase
	{
		protected readonly MyProjectContext Context;
		protected string Username { get; }

		protected ApiControllerBase(MyProjectContext context, IHttpContextAccessor contextAccessor)
		{
			this.Context = context;
			this.Username = contextAccessor.GetUserName();
		}
	}

	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public abstract class ApiControllerBase<TDomainObject> : Controller
		where TDomainObject : DomainObject
	{
		protected readonly MyProjectContext Context;
		protected string Username { get; }
		public IPAddress PrinterIpAddress { get; set; }
		public bool IsUnitTest { get; set; } = false;

		protected ApiControllerBase(MyProjectContext context, IHttpContextAccessor contextAccessor)
		{
			this.Context = context;
			this.Username = contextAccessor.GetUserName();
		}

		[HttpGet]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public abstract ActionResult<IQueryable<TDomainObject>> Get();

		[HttpGet("{key}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public abstract Task<ActionResult<TDomainObject>> Get(Guid key);

		[HttpPost]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		public async Task<ActionResult> Post([FromBody] TDomainObject model)
		{
			await this.Context.Set<TDomainObject>().AddAsync(model);
			await this.Context.SaveChangesAsync(this.Username);

			return CreatedAtRoute(new { id = model.GetId() }, model);
		}

		[HttpPut("{key}")]
		[ProducesResponseType(202)]
		[ProducesResponseType(404)]
		public async Task<ActionResult> Put(Guid key, [FromBody] TDomainObject model)
		{
			var updateTarget = await this.Context.Set<TDomainObject>().FindAsync(key);
			if (updateTarget == null)
			{
				return NotFound();
			}

			this.Context.Entry(updateTarget).UpdateRecord(model);
			await this.Context.SaveChangesAsync(this.Username);

			return AcceptedAtRoute(new { key }, model);
		}

		[HttpPatch("{key}")]
		public async Task<ActionResult> Patch(Guid key, [FromBody] JsonPatchDocument<TDomainObject> patch)
		{
			var target = await this.Context.Set<TDomainObject>().FindAsync(key);
			if (target == null)
			{
				return NotFound();
			}

			patch.ApplyTo(target);
			await this.Context.SaveChangesAsync(this.Username);

			return AcceptedAtRoute(new { key }, target);
		}

		[HttpDelete("{key}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<ActionResult> Delete(Guid key)
		{
			var target = await this.Context.Set<TDomainObject>().FindAsync(key);
			target.Deactivate();
			await this.Context.SaveChangesAsync(this.Username);

			return Ok();
		}

		protected async Task<bool> TryRefreshPrinterIpAddressAsync()
		{
			if (this.IsUnitTest)
			{
				return true;
			}
			var savedIp = this.Request?.Cookies["savedPrinter"];
			if (!string.IsNullOrEmpty(savedIp) && IPAddress.TryParse(savedIp, out var parsedIp))
			{
				this.PrinterIpAddress = parsedIp;
				return true;
			}
			return false;
		}
	}
}