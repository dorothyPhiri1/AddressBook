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
            CreateMap<Contact, ContactDto>().ForMember(des => des.TelephoneNumbersDto, t => t.MapFrom(src => src.TelephoneNumbers));

            CreateMap<TelephoneNumber, TelephoneNumberDto>();

            CreateMap<List<TelephoneNumber>, List<TelephoneNumberDto>>();

            CreateMap<ContactCreateDto, Contact>();

            CreateMap<UpdateContactDto, Contact>();
        }
           
    }
}
