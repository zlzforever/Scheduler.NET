# Scheduler.NET

Scheduler.NET is a distributed scheduler system. It support http call back job and long polling job.

### DESIGN

![DESIGN](https://github.com/zlzforever/Scheduler.NET/blob/master/images/1.png)

### SETUP SERVICE

1. Create a ASP.NET Core MVC project
2. Install Scheduler.NET package from NUGET
3. AddSchedulerNet after AddMvc

	services.AddMvc().AddSchedulerNet(Configuration).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

4. Add UseSchedulerNet in Configure method
	
	app.UseSchedulerNet();

5. Update config file:

		"SchedulerNET": {
			"HangfireStorageType": "SqlServer",
			"HangfireConnectionString": "Server=.\\SQLEXPRESS;Database=Scheduler.NET;Trusted_Connection=True;MultipleActiveResultSets=true;",
			"UseToken": false,
			"Tokens": [ "a1", "a2" ],
			"TokenHeader": "SchedulerNET",
			"IgnoreCrons": [ "* * * * *" ]
		}

6. Create database: Scheduler.NET in MSSQL
7. Run the web application from port 5000

### SETUP CLIENT

1. Create a dotnet core Console project
2. Install Scheduler.NET.Client package from NUGET
3. Add a test processor class ConsoleJobProcessor and update main method

		public class ConsoleJobProcessor : SimpleJobProcessor
		{
			public override bool Process(JobContext context)
			{
				Console.WriteLine(JsonConvert.SerializeObject(context));
				return true;
			}
		}

		static void Main(string[] args)
		{
			SchedulerNetHelper api = new SchedulerNetHelper("http://localhost:5000");
			api.CreateJob(new Job { Name = typeof(ConsoleJobProcessor).FullName, Cron = "*/1 * * * *", Group = "Test", Content = "aaa" });
			SchedulerNetClient client = new SchedulerNetClient("Test", "http://localhost:5000");
			client.Init();
			Console.Read();
		}

4. Run the console project

### CallbackJob

1. Create a callbackjob:

			SchedulerNetHelper api = new SchedulerNetHelper("http://localhost:5000");
			api.CreateCallbackJob(new CallbackJob { Name = "job1", Content = "{}", Cron = "*/1 * * * *", Group = "group1", Method = HttpMethod.Get, Url = "http://www.baidu.com" });

2. Then the scheduler.net will request http://www.baidu.com every 1 minutes.