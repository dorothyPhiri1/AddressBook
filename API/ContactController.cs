using AutoMapper;
using BusinessLayer.DataTransferObjects;
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
    [Route("api/contacts")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IUnitOfWork _unitofwork;
        private IMapper _mapper;

        public ContactController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;

        }

        [HttpGet]
        public IActionResult GetAllContacts([FromQuery] ContactQueryParameters contactQueryParameters)
        {
            try
            {
                var contacts = _unitofwork.ContactRepository.GetAllContactsAsyc(contactQueryParameters);
                var metadata = new
                {
                    contacts.TotalCount,
                    contacts.PageSize,
                    contacts.CurrentPage,
                    contacts.TotalPages,
                    contacts.HasNext,
                    contacts.HasPrevious
                };

                var contactsResults = _mapper.Map<IEnumerable<ContactDto>>(contacts);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                return Ok(contactsResults);
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }
                else
                {
                    var contactResult = _mapper.Map<ContactDto>(contact);
                    return Ok(contactResult);
                }
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }
                else
                {
                    var ownerResult = _mapper.Map<ContactDto>(contactDetailed);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
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
                    return BadRequest("Contact object is null");
                }

                if (!ModelState.IsValid)
                {
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
                    return BadRequest("Contact object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }

                var contactEntity = await _unitofwork.ContactRepository.GetContactByIdAsync(id);
                if (contactEntity == null)
                {
                    return NotFound();
                }

                _mapper.Map(updateContact, contactEntity);

                _unitofwork.ContactRepository.UpdateContact(contactEntity);
                await _unitofwork.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }

                //if (_unitofwork.TelephoneRepository.ContactTelephoneNumbers(id).Any())
                //{
                //    return BadRequest("Cannot delete contact. It has related telephone number. Delete those numbers first");
                //}


                _unitofwork.ContactRepository.DeleteContact(contact);
                await _unitofwork.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

