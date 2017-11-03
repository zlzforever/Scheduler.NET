using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.NET.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Scheduler;
using DotnetSpider.Enterprise.Core.Caching;
using Scheduler.NET.Core.Utils;

namespace Scheduler.NET.Scheduler.Controllers
{
	[Produces("application/json")]
	[Route("api/Task")]
	public class TaskController : Controller
	{

		// GET: api/Task/5
		[HttpGet("{id}", Name = "Get")]
		public object Get(String id)
		{
			String[] arrays = id.Split("|");
			SpiderJob _job = new SpiderJob() { TaskId = arrays[0], Cron = arrays[1] };
			HangFireJobManager.EnqueueHFJob(_job);

			HangFireJobManager.AddHFJob(_job.TaskId,_job);

			StackRedisUtil.Set("_S01", "SSS_002");
			var rent = StackRedisUtil.Get("_S01");

			return "value";
		}

		/// <summary>
		/// 添加任务
		/// </summary>
		/// <param name="value"></param>
		// POST: api/Task
		[HttpPost]
		public void Add([FromBody]string value)
		{

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
