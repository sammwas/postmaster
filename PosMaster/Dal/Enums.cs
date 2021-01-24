﻿namespace PosMaster.Dal
{
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

	public enum SmsProvider
	{
		AfricasTalking,
		Cosmere
	}

	public enum AlertLevel
	{
		Success,
		Warning,
		Error
	}

	public enum Document
	{
		Receipt,
		Grn,
		Po,
		Invoice
	}
}
