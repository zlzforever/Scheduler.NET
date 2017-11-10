using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;

namespace Scheduler.NET.Portal.Controllers
{

	[Produces("application/json")]
	[Route("api/Task")]
	public class TaskController : Controller
	{

		private IJobManager _JobManager { get; set; }

		public TaskController(IJobManager _jobManager)
		{
			_JobManager = _jobManager;
		}

		// GET: api/Task/5
		[HttpGet("{id}", Name = "Get")]
		public object Get(String id)
		{
			return "value";
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		// POST: api/Task
		[HttpPost]
		public void Add([FromBody]SpiderJob value)
		{
			try
			{
				if (value != null)
				{
					_JobManager.AddOrUpdateHFJob(value);
				}
			}
			catch (Exception ex)
			{
				//invoke
				return;
			}
			//failed invoke
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		// PUT: api/Task/5
		[HttpPut("{id}")]
		public void Modify(int id, [FromBody]string value)
		{

		}

		/// <summary>
		/// 删除任务
		/// </summary>
		/// <param name="id"></param>
		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public void Remove(int id)
		{

		}
	}
}
