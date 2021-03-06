﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FootlooseFS.Models;
using Microsoft.Extensions.Options;

namespace FootlooseFS.Service.Tests
{
    [TestClass]
    public class FootlooseFSServiceUnitTests
    {
        private IOptions<FootlooseFSConfiguration> options;

        [TestInitialize]
        public void Setup()
        {
            options = Options.Create(new FootlooseFSConfiguration());
        }

        [TestMethod]
        public void TestUpdatePerson()
        {
            var testFactory = new FootlooseFSTestUnitOfWorkFactory();

            var service = new FootlooseFSService(testFactory, options);

            var person = new Person
            {
                PersonID = 1,
                FirstName = "Pam",
                LastName = "Scicchitano",
                EmailAddress = "pam@gmail.com", // Updated email address
                Phones = new List<Phone>
                {
                    new Phone
                    {
                        PersonID = 1,
                        Number = "336-418-5555",    // Update home phone
                        PhoneType = new PhoneType { Name = "Home", PhoneTypeID = 1 },
                        PhoneTypeID = 1
                    },                    
                    new Phone
                    {
                        PersonID = 1,
                        Number = "336-418-7777",    // Updated work phone
                        PhoneType = new PhoneType { Name = "Work", PhoneTypeID = 2 },
                        PhoneTypeID = 2
                    },
                    new Phone
                    {
                        PersonID = 1,
                        Number = "336-418-4444",    // Updated cell phone
                        PhoneType = new PhoneType { Name = "Cell", PhoneTypeID = 3 },
                        PhoneTypeID = 3
                    }
                },
                Addresses = new List<PersonAddressAssn>
                {
                    new PersonAddressAssn
                    {
                        Address = new Address
                        {
                            StreetAddress = "631 Glebe Road",   // Updated address
                            State = "VA",
                            Zip = "20178",
                            AddressID = 1,
                            City = "Arlington"
                        },
                        AddressID = 1,
                        AddressType = new AddressType { Name = "Home", AddressTypeID = 1 },
                        AddressTypeID = 1,
                        PersonID = 1
                    }
                },
            };

            var opStatus = service.UpdatePerson(person);

            var updatedPerson = (Person)opStatus.Data;

            // Get the updated person object from the UoW
            var updatedPersonFromUoW = service.GetPersonById(updatedPerson.PersonID, new PersonIncludes { Phones = true, Addressses = true });

            // Verify that email address was updated
            Assert.AreEqual(updatedPersonFromUoW.EmailAddress, "pam@gmail.com");
            
            // Verify that the home phone number was updated
            var homePhone = updatedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 1).FirstOrDefault();
            Assert.AreEqual(homePhone.Number, "336-418-5555");

            // Verify that the work number was updated
            var workPhone = updatedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 2).FirstOrDefault();
            Assert.AreEqual(workPhone.Number, "336-418-7777");

            // Verify that the cell number was updated
            var cellPhone = updatedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 3).FirstOrDefault();
            Assert.AreEqual(cellPhone.Number, "336-418-4444");

            // Verify that the address was updated
            var address = updatedPersonFromUoW.Addresses.Where(a => a.AddressTypeID == 1).FirstOrDefault().Address;

            Assert.AreEqual(address.StreetAddress, "631 Glebe Road");
            Assert.AreEqual(address.City, "Arlington");
            Assert.AreEqual(address.Zip, "20178");
            Assert.AreEqual(address.State, "VA");
        }

        [TestMethod]
        public void TestInsertPerson()
        {
            var testFactory = new FootlooseFSTestUnitOfWorkFactory();

            var service = new FootlooseFSService(testFactory, options);

            var person = new Person
            {
                PersonID = 5,
                FirstName = "John",
                LastName = "Dorman",
                EmailAddress = "john@dorman.com", // Updated email address
                Phones = new List<Phone>
                {
                    new Phone
                    {
                        PersonID = 5,
                        Number = "813-657-2222",    // Update home phone
                        PhoneType = new PhoneType { Name = "Home", PhoneTypeID = 1 },
                        PhoneTypeID = 1
                    }
                },
                Addresses = new List<PersonAddressAssn>
                {
                    new PersonAddressAssn
                    {
                        Address = new Address
                        {
                            StreetAddress = "823 Newton Drive",   // Updated address
                            State = "FL",
                            Zip = "33782",
                            AddressID = 5,
                            City = "Pinellas Park"
                        },
                        AddressID = 5,
                        AddressType = new AddressType { Name = "Home", AddressTypeID = 1 },
                        AddressTypeID = 1,
                        PersonID = 5
                    }
                },
            };

            var opStatus = service.InsertPerson(person);

            var insertedPerson = (Person)opStatus.Data;

            // Get the inserted person object from the UoW
            var insertedPersonFromUoW = service.GetPersonById(5, new PersonIncludes { Phones = true, Addressses = true });

            // Verify that email address was applied
            Assert.AreEqual(insertedPersonFromUoW.EmailAddress, "john@dorman.com");

            // Verify that the home phone number was updated
            var homePhone = insertedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 1).FirstOrDefault();
            Assert.AreEqual(homePhone.Number, "813-657-2222");

            // Verify that there is no work number
            var workPhone = insertedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 2 && !string.IsNullOrEmpty(p.Number));
            Assert.AreEqual(workPhone.Count(), 0);

            // Verify that there is no cell number
            var cellPhone = insertedPersonFromUoW.Phones.Where(p => p.PhoneTypeID == 3 && !string.IsNullOrEmpty(p.Number));
            Assert.AreEqual(cellPhone.Count(), 0);

            // Verify that the address was updated
            var address = insertedPersonFromUoW.Addresses.Where(a => a.AddressTypeID == 1).FirstOrDefault().Address;

            Assert.AreEqual(address.StreetAddress, "823 Newton Drive");
            Assert.AreEqual(address.City, "Pinellas Park");
            Assert.AreEqual(address.Zip, "33782");
            Assert.AreEqual(address.State, "FL");
        }
    }
}
