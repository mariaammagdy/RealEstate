﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Payment : BaseEntity<Guid>
    {
        [Required]
        public Guid ContractId { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }  // CreditCard, BankTransfer, ...

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }  // Optional reference number for tracking

        public bool? IsLate { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Late fee must be a positive value.")]
        public decimal? LateFee { get; set; }  // Nullable if no late fee is applicable

        // Set the default value for Status to Pending
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Required, Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }  // Payment amount with validation

    }

    // Enum to represent payment status
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
}