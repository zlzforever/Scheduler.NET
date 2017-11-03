using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Scheduler.NET.Core.Domain
{

	[DataContract(Name = "message")]
	public class Message<T> where T : class
	{
		public Message(HttpStatus status, T t) : base()
		{
			this.Data = t;
			this.Status = Status;
		}

		public Message(HttpStatus status, String _msg) : base()
		{
			this.Msg = _msg;
			this.Status = Status;
		}

		[DataMember(Name = "status")]
		public HttpStatus Status { get; set; }

		[DataMember(Name = "data")]
		public T Data { get; set; }

		[DataMember(Name = "message")]
		public String Msg { get; set; }
	}

	public class Messages
	{
		public static Message<T> GetOKMessage<T>(T _t, String msg) where T : class
		{
			Message<T> message = new Message<T>(HttpStatus.Ok, _t);
			message.Msg = msg;
			return message;
		}

		public static Message<T> GetFailMessage<T>(String msg) where T : class
		{
			Message<T> message = new Message<T>(HttpStatus.Error, msg);
			return message;
		}

	}

	public enum HttpStatus
	{
		Ok,
		Error
	}

}
