namespace ContactList.Services;
using ContactList.Models;

public class ContactService : IContactService
{
    private int _nextPersonId;
    private Person[] _personList;
    public ContactService()
    {
        _nextPersonId = 1;
        _personList = new Person[100];
    }

    public Person addPerson(Person person)
    {
        if (_nextPersonId > _personList.Length) throw new Exception("More than " +_personList.Length.ToString()+ " contacts is not allowed");
        person.PersonId = _nextPersonId;
        _personList[_nextPersonId - 1] = person;
        _nextPersonId += 1;
        return person;
    }

    public Person[] getPersonList()
    {
        return _personList.Where(p => p != null).ToArray();
    }

    public Person updatePerson(int PersonId, Person person)
    {
        _personList[PersonId - 1].PersonName = person.PersonName;
        _personList[PersonId - 1].PersonEmail = person.PersonEmail;
        _personList[PersonId - 1].PersonPhone = person.PersonPhone;
        return _personList[PersonId - 1];
    }

    public void deletePerson(int PersonId)
    {
        _personList[PersonId - 1] = null!;
    }
}
