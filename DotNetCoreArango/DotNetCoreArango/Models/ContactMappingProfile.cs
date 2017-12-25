using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCoreArango.Data.Entities;

namespace DotNetCoreArango.Models
{
    // create maps between the model and the entity
    public class ContactMappingProfile : Profile // derive it from Profile which is in AutoMapper namespace
    {
        public ContactMappingProfile()
        {
            CreateMap<Contact, ContactModel>() // ...create map from Contact entity to ContactModel; it will try to match the properties
                .ForMember(model => model.Id,
                    opt => opt.MapFrom(contact => contact.Key))
                .ForMember(c => c.Url,
                    opt => opt.ResolveUsing<ContactUrlResolver>())
                // BELOW IS WHEN WE NEED TO REVERSE THE DIRECTION -> ContactModel to Contact 
                .ReverseMap() // 
                    .ForMember(contact => contact.Key, 
                        opt => opt.MapFrom(model => model.Id));
        }
    }
}
