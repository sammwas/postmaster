using System;

namespace PosMaster.Dal
{
	public abstract class BaseEntity
	{
		public BaseEntity()
		{
			DateCreated = DateTime.UtcNow;
			Status = EntityStatus.Active;
		}

		public Guid Id { get; set; }
		public string Code { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateLastModified { get; set; }
		public Guid ClientId { get; set; }
		public Guid InstanceId { get; set; }
		public string Personnel { get; set; }
		public string LastModifiedBy { get; set; }
		public string Notes { get; set; }
		public EntityStatus Status { get; set; }

	}

	public enum EntityStatus
	{
		Inactive,
		Active,
		Closed
	}

	public enum DataSource
	{
		Web,
		Mobile
	}

	public enum AlertLevel
	{
		Success,
		Warning,
		Error
	}
}
