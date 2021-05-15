using ContactDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace ContactSavingWebAPI.Controllers
{
    public class ContactsController : ApiController
    {
        public IHttpActionResult Index()
        {
            ContactDBEntities entities = new ContactDBEntities();
            List<Contact> contact = entities.Contacts.ToList();

            //Add a Dummy Row.
            contact.Insert(0, new Contact());
            return Ok(contact);
        }
        public IHttpActionResult Get(string firstName = "All", string lastName = "All", string phoneNumber = "All")
        {
            //var fullName = firstName + " " + lastName;
            string username = Thread.CurrentPrincipal.Identity.Name;

            using (ContactDBEntities entities = new ContactDBEntities())
             {
                switch (firstName.ToLower())
                {
                    case "all":
                        return Ok(entities.Contacts.ToList());
                    case "firstName":
                        return Ok(entities.Contacts.Where(e => e.FirstName.ToLower() == firstName.ToLower() && e.LastName.ToLower() == lastName.ToLower()).ToList());
                    case "lastName":
                        return Ok(entities.Contacts.Where(e => e.FirstName.ToLower() == firstName.ToLower() && e.LastName.ToLower() == lastName.ToLower()).ToList());
                    case "phoneNumber":
                        return Ok(entities.Contacts.Where(e => e.FirstName.ToLower() == firstName.ToLower() && e.LastName.ToLower() == lastName.ToLower()).ToList());
                    default:
                        return BadRequest();
                }
            }
        }

        public HttpResponseMessage Get(int id)
        {
            using (ContactDBEntities entities = new ContactDBEntities())
            {
                var entity = entities.Contacts.FirstOrDefault(e => e.ID == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Contact with Id = " + id.ToString() + " is not found");
                }
            }
        }

        public HttpResponseMessage Post([FromBody] Contact contact)
        {
            try
            {
                using (ContactDBEntities entities = new ContactDBEntities())
                {
                    entities.Contacts.Add(contact);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, contact);
                    message.Headers.Location = new Uri(Request.RequestUri + contact.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (ContactDBEntities entities = new ContactDBEntities())
                {
                    var entity = entities.Contacts.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Contact with Id = " + id.ToString() + " is not found to delete");
                    }
                    else
                    {
                        entities.Contacts.Remove(entity);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        public HttpResponseMessage PUT([FromBody]int id, Contact contact)
        {
            try
            {
                using (ContactDBEntities entities = new ContactDBEntities())
                {
                    var entity = entities.Contacts.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Contact with Id = " + id.ToString() + " is not found to update");
                    }
                    else
                    {
                        entity.FirstName = contact.FirstName;
                        entity.LastName = contact.LastName;
                        entity.Gender = contact.Gender;
                        entity.PhoneNumber = contact.PhoneNumber;

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
