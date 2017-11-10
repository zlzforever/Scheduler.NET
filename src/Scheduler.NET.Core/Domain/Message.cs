using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Scheduler.NET.Core.Domain
{

	public class Message : Message<String>
	{
		public Message(HttpStatus status, String t) : base(status, t)
		{ }
	}

	[DataContract(Name = "message")]
	public class Message<T> where T : class
	{
		public Message() : base()
		{ }

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

	public class Messager
	{
		public static Message<T> GetOKMessage<T>(T _t, String msg) where T : class
		{
			Message<T> message = new Message<T>(HttpStatus.Ok, _t);
			message.Msg = msg;
			return message;
		}

		public static Message GetOKMessage(String msg)
		{
			Message message = new Message(HttpStatus.Ok, msg);
			return message;
		}

		public static Message GetFailMessage(String msg)
		{
			Message message = new Message(HttpStatus.Error, msg);
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
