using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreArango.Data.Entities;
using DotNetCoreArango.Models;
using DotNetCoreArango.Controllers;

namespace DotNetCoreArango.Models
{
    // this is a 'values resolver' that knows how to resolve a Url for a contact
    // trick is to have use IValueResolver
    public class ContactUrlResolver : IValueResolver<Contact, ContactModel, string> // Contact (source), ContactModel (Destination), 
                                                                                    // string (type of value we want returned)
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactUrlResolver(IHttpContextAccessor httpContextAccessor) // this is injected in so we can get access to the items collection
                                                                            // of HttpContext
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // implementing the interface provides a callback called 'Resolve' ... 
        public string Resolve(Contact source, ContactModel destination, string destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.URLHELPER];
            return url.Link("ContactGet", new { key = source.Key }); // ContactGet is the name we gave ContactsController Get a contact route
        }
    }
}