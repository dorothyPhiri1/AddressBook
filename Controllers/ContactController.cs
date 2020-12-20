using AutoMapper;
using BusinessLayer.DataTransferObjects;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        //private ILoggerManager _logger;
        private IUnitOfWork _unitofwork;
        private IMapper _mapper;

        public ContactController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                var contacts = await _unitofwork.ContactRepository.GetAllContactsAsyc();
                //_logger.LogInfo($"Returned all contacts from database.");

                var contactsResults = _mapper.Map<IEnumerable<ContactDto>>(contacts);
                return Ok(contactsResults);
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Something went wrong inside GetAllContacts action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "ContactById")]
        public async Task<IActionResult> GetContactById(int id)
        {
            try
            {
                var contact = await _unitofwork.ContactRepository.GetContactByIdAsync(id);
                if (contact == null)
                {
                    //_logger.LogError($"Contact with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                   // _logger.LogInfo($"Returned Contact with id: {id}");

                    var contactResult = _mapper.Map<ContactDto>(contact);
                    return Ok(contactResult);
                }
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Something went wrong inside GetContactById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/contactDetails")]
        public async Task<IActionResult> GetContactWithDetails(int id)
        {
            try
            {
                var contactDetailed = await _unitofwork.ContactRepository.GetContactWithDetailsAsync(id);
                if (contactDetailed == null)
                {
                   //_logger.LogError($"Contact with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    //_logger.LogInfo($"Returned Contact with details for id: {id}");

                    var ownerResult = _mapper.Map<ContactDto>(contactDetailed);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside GetContactWithDetails action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] ContactCreateDto contact)
        {
            try
            {
                if (contact == null)
                {
                    //_logger.LogError("Contact object sent from client is null.");
                    return BadRequest("Contact object is null");
                }

                if (!ModelState.IsValid)
                {
                    //_logger.LogError("Invalid contact object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var contactEntity = _mapper.Map<Contact>(contact);

                _unitofwork.ContactRepository.CreateContact(contactEntity);
                await _unitofwork.SaveAsync();

                var createdContact = _mapper.Map<ContactDto>(contactEntity);

                return CreatedAtRoute("ContactById", new { id = createdContact.ContactId }, createdContact);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside CreateContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactDto updateContact)
        {
            try
            {
                if (updateContact == null)
                {
                    //_logger.LogError("Contact object sent from client is null.");
                    return BadRequest("Contact object is null");
                }

                if (!ModelState.IsValid)
                {
                    //_logger.LogError("Invalid contact object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var contactEntity = await _unitofwork.ContactRepository.GetContactByIdAsync(id);
                if (contactEntity == null)
                {
                   // _logger.LogError($"Contact with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _mapper.Map(updateContact, contactEntity);

                _unitofwork.ContactRepository.UpdateContact(contactEntity);
                await _unitofwork.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside UpdateContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var contact = await _unitofwork.ContactRepository.GetContactByIdAsync(id);
                if (contact == null)
                {
                    //_logger.LogError($"Contact with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if (_unitofwork.TelephoneRepository.ContactTelephoneNumbers(id).Any())
                {
                    //_logger.LogError($"Cannot delete contact with id: {id}. It has related telephone numbers. Delete those numbers first");
                    return BadRequest("Cannot delete contact. It has related telephone number. Delete those numbers first");
                }

                _unitofwork.ContactRepository.DeleteContact(contact);
                await _unitofwork.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside DeleteContact action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
   
