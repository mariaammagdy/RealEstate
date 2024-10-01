﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.DTOModels;
using Microsoft.AspNetCore.Http;
using PresentationLayer.helper;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace PresentationLayer.Controllers
{
  //  [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly PropertyService _propertyService;
        private readonly UserService _userService;
        private readonly ContractService _contractService;
		private readonly PaymentService _paymentService;
        private readonly MyDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(PropertyService propertyService, UserService userService , 
            ContractService contractService, PaymentService paymentService,MyDbContext context ,IWebHostEnvironment webHostEnvironment)
        {
            _propertyService = propertyService;
            _userService = userService;
            _contractService = contractService;
            _paymentService = paymentService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Property CRUD Operations
        public async Task<IActionResult> ListProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return View(properties);
        }

        public async Task<IActionResult> CreateProperty(PropertyDTO propertyDto)
        {
          
            if (propertyDto.PropertyPicture != null)
            {

                var fileName = UploadFile.UploadImage("PropertyPicture", propertyDto.PropertyPicture);
                propertyDto.PropertyPictureUrl = fileName;
            }
         
            if (ModelState.IsValid)
            {
                await _propertyService.CreatePropertyAsync(propertyDto);
                return RedirectToAction("ListProperties");
            }
            return View(propertyDto);
        }

        public async Task<IActionResult> EditProperty(Guid id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        [HttpPost]
        public async Task<IActionResult> EditProperty(PropertyDTO propertyDto)
        {
            if (propertyDto.PropertyPicture != null)
            {

                var fileName = UploadFile.UploadImage("PropertyPicture", propertyDto.PropertyPicture);
                propertyDto.PropertyPictureUrl = fileName;
            }
            if (ModelState.IsValid)
            {
                await _propertyService.UpdatePropertyAsync(propertyDto);
                return RedirectToAction("ListProperties");
            }
            return View(propertyDto);
        }
    
       
        public async Task<IActionResult> SoftDeleteProperty(Guid id)
        {
            await _propertyService.SoftDeletePropertyAsync(id);
            return RedirectToAction("ListProperties");
        }

        public async Task<IActionResult> ShowProperty(Guid id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        // User Listing
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // Soft Delete User
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _userService.SoftDeleteUserAsync(id);
            return RedirectToAction("ListUsers");
        }

 
        // User Details
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> ListContracts()
        {

            var contracts = await _contractService.GetAllContractsAsync();
            return View(contracts);
        }
        public async Task<IActionResult> ContractDetails(Guid id)
        {

            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }
		public async Task<IActionResult> ListPayments()
		{

			var payment = await _paymentService.GetAllPaymentsAsync();
			return View(payment);
		}
        public async Task<IActionResult> PaymentDetails(Guid id)
        {
            var pay = await _paymentService.GetPaymenttByIdAsync(id);
            if (pay == null)
            {
                return NotFound();
            }
            return View(pay);
        }

        public async Task<IActionResult> Terminate(Guid id)
        {
            try
            {
                await _contractService.TerminateAsync(id);
                TempData["SuccessMessage"] = "Contract terminated successfully.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("ListContracts");
        }
        public async Task<IActionResult> DownloadFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "properties", "Residential Lease Agreement.pdf");
            var memory = new MemoryStream();
            using(var stream = new FileStream(path,FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "application/pdf";
            var fileName = Path.GetFileName(path);
            return File(memory , contentType, fileName);
        }
    


    }
}
