using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCoreArango.Data;
using DotNetCoreArango.Data.Entities;
using DotNetCoreArango.Filters;
using DotNetCoreArango.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetCoreArango.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")] 
    [ValidateModel]
    public class ContactsController : BaseController
    {
        private readonly IContactStore _contactStore;
        private readonly ILogger<ContactsController> _logger;
        private readonly IMapper _mapper;

        public ContactsController(IContactStore contactStore, 
            ILogger<ContactsController> logger,
            IMapper mapper)
        {
            _contactStore = contactStore;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("get all contacts");

            var contacts = await _contactStore.GetAllAsync<Contact>();

            return Ok(_mapper.Map<IEnumerable<ContactModel>>(contacts));
        }

        // GET: api/Contacts/5
        [HttpGet("{key}", Name = "ContactGet")]
        public IActionResult Get(string key)
        {
            return StatusCode(501); // not implemented
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ContactModel model) // FromBody allows us to map request bodies to .NET Objects
        {
            try
            {
                _logger.LogInformation("Creating a contact");

                var contact = _mapper.Map<Contact>(model);// map back to contact enitity for DB; have to do ReverseMap in mappings profile

                var result = await _contactStore.InsertAsync(contact);

                // don't assume the insert worked...
                if (result != null
                    && result.New != null
                    && result.New.Name == model.Name
                    && !string.IsNullOrWhiteSpace(result.Key))
                {
                    var contactModel = _mapper.Map<ContactModel>(result.New); // back to ContactModel for the end user
                    var newUri = Url.Link("ContactGet", new { key = result.New.Key });

                    return Created(newUri, contactModel);
                }

                _logger.LogWarning("Could not save contact to the database");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving contact: {ex}");
            }

            return BadRequest();
        }

        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
