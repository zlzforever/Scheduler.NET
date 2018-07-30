using Microsoft.AspNetCore.SignalR.Client;
using Scheduler.NET.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.NET.Client
{
	/// <summary>
	/// Scheduler.NET 客户端
	/// </summary>
	public interface ISchedulerNetClient
	{
		/// <summary>
		/// 初始化并启动
		/// </summary>
		void Init();
	}

	/// <summary>
	/// Scheduler.NET 客户端
	/// </summary>
	public class SchedulerNetClient : ISchedulerNetClient
	{
		private readonly Dictionary<string, Type> _classNameMapTypes = new Dictionary<string, Type>();
		private readonly ConcurrentDictionary<string, object> _runningJobs = new ConcurrentDictionary<string, object>();
		private int _retryTimes;

		/// <summary>
		/// 任务分组
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Scheduler.NET 服务地址
		/// </summary>
		public string Service { get; set; }

		/// <summary>
		/// 是否忽略正在运行的任务
		/// </summary>
		public bool BypassRunning { get; set; } = true;

		/// <summary>
		/// 服务连接重试次数
		/// </summary>
		public int RetryTimes { get; set; } = 3600;

		/// <summary>
		/// 构造方法
		/// </summary>
		public SchedulerNetClient()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="service">任务分组</param>
		/// <param name="group">Scheduler.NET 服务地址</param>
		public SchedulerNetClient(string service, string group) : this()
		{
			Group = group;
			Service = new Uri(service).ToString();
		}

		/// <summary>
		/// 初始化并启动
		/// </summary>
		public void Init()
		{
			CheckArguments();
			ScanAssemblies();
			if (_classNameMapTypes.Count == 0)
			{
				Debug.WriteLine("Detected none job in this application.");
				return;
			}
			_runningJobs.Clear();
			ConnectSchedulerNet();
		}

		private void ConnectSchedulerNet()
		{
			try
			{
				var connection = new HubConnectionBuilder()
										.WithUrl($"{Service}client/?group={Group}")
										.Build();
				OnClose(connection);
				OnFire(connection);
				OnWatchCallback(connection);
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
						Debug.WriteLine("Retry to connect scheduler.net server.");
						ConnectSchedulerNet();
					}
				}
			}
		}

		private void OnWatchCallback(HubConnection connection)
		{
			connection.On<bool>("WatchCallback", (isSuccess) =>
			{
				if (!isSuccess)
				{
					connection.StopAsync().Wait();
					connection.DisposeAsync().Wait();
					throw new SchedulerNetException("Watch failed.");
				}
			});
		}

		private void OnClose(HubConnection connection)
		{
			connection.Closed += e =>
			{
				if (e == null || ((WebSocketException)e).WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
				{
					ConnectSchedulerNet();
				}
				return Task.CompletedTask;
			};
		}

		private void OnFire(HubConnection connection)
		{
			connection.On<JobContext, string>("Fire", (context, batchId) =>
			{
				var className = context?.Name;
				try
				{
					bool shouldFire = false;
					if (string.IsNullOrWhiteSpace(className) || !_classNameMapTypes.ContainsKey(className) || (BypassRunning && _runningJobs.ContainsKey(className)))
					{
						return;
					}
					if ((context.FireTime - DateTime.Now).TotalSeconds <= 10)
					{
						shouldFire = true;
					}
					else
					{
						Debug.WriteLine($"Fire job timeout: {className}.");
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
							_runningJobs.TryRemove(className, out _);
						});
					}
					else
					{
						connection.SendAsync("FireCallback", batchId, context.Id, JobStatus.Bypass).Wait();
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine($"Fire job {className} failed: {e}");
				}
			});
		}

		private void ScanAssemblies()
		{
			_classNameMapTypes.Clear();
			var jobProcessorType = typeof(IJobProcessor);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (string.IsNullOrWhiteSpace(type.FullName))
					{
						continue;
					}
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
