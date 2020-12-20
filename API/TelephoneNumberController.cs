using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBook.API
{
    [Route("api/contacts/{contactId}/telephonenumbers")]
    [ApiController]
    public class TelephoneNumberController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public TelephoneNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
		[HttpGet]
		public IActionResult GetTelephoneNumbersForContact(int contactId, [FromQuery] TelephoneNumberParameters parameters)
		{
			var telephones = _unitOfWork.TelephoneRepository.ContactTelephoneNumbers(contactId, parameters);

			var metadata = new
			{
				telephones.TotalCount,
				telephones.PageSize,
				telephones.CurrentPage,
				telephones.TotalPages,
				telephones.HasNext,
				telephones.HasPrevious
			};

			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

			return Ok(telephones);
		}

		[HttpGet("{id}")]
		public IActionResult GetTelephoneNumberByContactId(int contactId, Guid id)
		{
			var telephoneNumber = _unitOfWork.TelephoneRepository.GetTelephoneByContact(contactId, id);

			if (telephoneNumber==null)
			{
				return NotFound();
			}

			return Ok(telephoneNumber);
		}
	}
}
