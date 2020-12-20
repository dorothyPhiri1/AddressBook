using AutoMapper;
using BusinessLayer.DataTransferObjects;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBook
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Contact, ContactDto>();

            CreateMap<TelephoneNumber, TelephoneNumberDto>();

            CreateMap<ContactCreateDto, Contact>();

            CreateMap<UpdateContactDto, Contact>();

            //CreateMap<OwnerForUpdateDto, Owner>();
        }
           
    }
}
