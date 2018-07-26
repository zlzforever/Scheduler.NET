using Scheduler.NET.Common;
using System;

namespace Scheduler.NET.Client
{
	public interface IClient
	{
		string Create<T>(T job) where T : IJob;
		void Delete<T>(string id) where T : IJob;
		void Update<T>(T job) where T : IJob;
		void Trigger<T>(string id) where T : IJob;
	}
}
