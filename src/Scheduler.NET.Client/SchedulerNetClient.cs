using Microsoft.AspNetCore.SignalR.Client;
using Scheduler.NET.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.NET.Client
{
	public class SchedulerNETClient
	{
		private readonly Dictionary<string, Type> _classNameMapTypes = new Dictionary<string, Type>();
		private ConcurrentDictionary<string, object> _runningJobs = new ConcurrentDictionary<string, object>();
		private int _retryTimes;

		public string Group { get; set; }
		public string Service { get; set; }
		public bool BypassRuning { get; set; } = true;
		public int RetryTimes { get; set; } = 3600;

		public SchedulerNETClient()
		{
		}

		public SchedulerNETClient(string group, string service) : this()
		{
			Group = group;
			Service = new Uri(service).ToString();
		}

		public void Init()
		{
			CheckArguments();
			DetectJobs();
			if (_classNameMapTypes.Count == 0)
			{
				Debug.Print("Detected none job in this application.");
				return;
			}
			_runningJobs.Clear();


			RemoteSchedulerNET();

		}

		private void RemoteSchedulerNET()
		{
			try
			{
				var connection = new HubConnectionBuilder()
										.WithUrl($"{Service}client/?group={Group}")
										.Build();
				connection.Closed += e =>
				{
					if (e == null || ((WebSocketException)e).WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
					{
						RemoteSchedulerNET();
					}
					return Task.CompletedTask;
				};
				connection.On<JobContext, string>("Fire", (context, batchId) =>
				{
					try
					{
						bool shouldFire = false;
						var className = context?.ClassName;
						if (string.IsNullOrWhiteSpace(className) || !_classNameMapTypes.ContainsKey(className) || (BypassRuning && _runningJobs.ContainsKey(className)))
						{
							return;
						}
						if ((context.FireTime - DateTime.Now).TotalSeconds <= 10)
						{
							shouldFire = true;
						}
						else
						{
							Debug.Print("Fire job timeout.");
						}
						if (shouldFire)
						{
							connection.SendAsync("FireCallback", batchId, context.Id, JobStatus.Running).Wait();

							var jobType = _classNameMapTypes[className];
							_runningJobs.TryAdd(className, null);
							Task.Factory.StartNew(() =>
							{
								bool success = false;
								try
								{
									var jobObject = (IJobProcessor)Activator.CreateInstance(jobType);
									success = jobObject.Process(context);
								}
								catch
								{
									// TODO: LOG
								}
								finally
								{
									connection.SendAsync("Complete", batchId, context.Id, success).Wait();
								}
							}).ContinueWith((t) =>
							{
								object j;
								_runningJobs.TryRemove(className, out j);
							});
						}
						else
						{
							connection.SendAsync("FireCallback", batchId, context.Id, JobStatus.Bypass).Wait();
						}
					}
					catch (Exception e)
					{
						Debug.Print($"Fire job failed: {e}");
					}
				});
				connection.On<bool>("WatchCallback", (isSuccess) =>
				{
					if (!isSuccess)
					{
						connection.StopAsync().Wait();
						connection.DisposeAsync().Wait();
						throw new SchedulerException("Watch failed.");
					}
				});
				connection.StartAsync().Wait();
				connection.SendAsync("Watch", _classNameMapTypes.Keys.ToArray()).Wait();
			}
			catch (Exception e) when (e.InnerException?.InnerException is SocketException)
			{
				var exception = (SocketException)e.InnerException.InnerException;
				if (exception.SocketErrorCode == SocketError.TimedOut || exception.SocketErrorCode == SocketError.ConnectionRefused)
				{
					Thread.Sleep(1000);
					var times = Interlocked.Increment(ref _retryTimes);
					if (times <= RetryTimes)
					{
						Console.WriteLine("Retry to connect scheduler.net server.");
						RemoteSchedulerNET();
					}
				}
			}
		}



		private void DetectJobs()
		{
			_classNameMapTypes.Clear();
			var jobProcessorType = typeof(IJobProcessor);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.FullName != "Scheduler.NET.Client.SimpleJobProcessor"
						&& type.FullName != "Scheduler.NET.Client.IJobProcessor"
						&& jobProcessorType.IsAssignableFrom(type))
					{
						_classNameMapTypes.Add(type.FullName, type);
					}
				}
			}
		}

		private void CheckArguments()
		{
			if (string.IsNullOrWhiteSpace(Group))
			{
				throw new ArgumentNullException($"{nameof(Group)}");
			}
			if (string.IsNullOrWhiteSpace(Service))
			{
				throw new ArgumentNullException($"{nameof(Service)}");
			}
		}
	}
}
