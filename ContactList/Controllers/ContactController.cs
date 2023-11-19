namespace ContactList.Controllers;
using Microsoft.AspNetCore.Mvc;
using ContactList.Services;
using ContactList.Models;

[ApiController]
[Route("person")]
public class ContactController : ControllerBase
{
    protected IContactService _service;
    public ContactController(IContactService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult Create(Person person)
    {
        try
        {
            Person createdPerson = _service.addPerson(person);
            return Created("", createdPerson);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message.ToString() });
        }
    }

    [HttpGet]
    public ActionResult List()
    {
        try
        {
            Person[] _personList = _service.getPersonList();
            return Ok(_personList);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message.ToString() });
        }
    }

    [HttpPut("{PersonId}")]
    public ActionResult Put(int PersonId, Person person)
    {
        try
        {
            Person updatedPerson = _service.updatePerson(PersonId, person);
            return Ok(updatedPerson);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message.ToString() });
        }
    }

    [HttpDelete("{PersonId}")]
    public ActionResult Delete(int PersonId)
    {
        try
        {
            _service.deletePerson(PersonId);
            return NoContent();
            }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message.ToString() });
        }
    }



}