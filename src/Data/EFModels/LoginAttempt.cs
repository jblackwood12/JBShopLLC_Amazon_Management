using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	public class LoginAttempt
	{
		public int LoginAttemptId { get; set; }

		[Required]
		[StringLength(255)]
		public string Username { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime LoginAttemptDateTime { get; set; }

		public bool IsSuccessful { get; set; }
	}
}
