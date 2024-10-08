﻿using PosMaster.Dal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace PosMaster.ViewModels
{
    public class BaseViewModel
    {
        public BaseViewModel()
        {
            Status = EntityStatus.Active;
        }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public bool Success { get; set; }
        [HiddenInput]
        public bool IsEditMode { get; set; }
        public EntityStatus Status { get; set; }
        public string Personnel { get; set; }
        [HiddenInput]
        public Guid Id { get; set; }
        [HiddenInput]
        public Guid ClientId { get; set; }
        [HiddenInput]
        public Guid InstanceId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
