﻿using PosMaster.Dal;
using System;

namespace PosMaster.ViewModels
{
	public class PoGrnProductViewModel
	{
		public PoGrnProductViewModel()
		{

		}
		public PoGrnProductViewModel(PoGrnProduct product, bool isPo)
		{
			ProductId = product.ProductId;
			DocumentId = isPo ? product.PurchaseOrderId : product.GoodReceivedNoteId;
			UnitPrice = isPo ? product.PoUnitPrice : product.GrnUnitPrice;
			Quantity = isPo ? product.PoQuantity : product.GrnQuantity;
			Notes = isPo ? product.PoNotes : product.GrnNotes;
			ProductName = product.Product == null ? "" : $"{product.Product.Code} - {product.Product.Name}";
		}

		public Guid DocumentId { get; set; }
		public Guid ProductId { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Quantity { get; set; }
		public string Notes { get; set; }
		public string ProductName { get; set; }
	}
}
