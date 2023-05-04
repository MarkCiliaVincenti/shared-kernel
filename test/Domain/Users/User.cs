﻿using SharedKernel.Domain.Aggregates;
using System;
using System.Collections.Generic;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace SharedKernel.Domain.Tests.Users
{
    internal class User : AggregateRoot<Guid>
    {
        private List<string> _emails;
        private List<Address> _addresses;

        protected User()
        {
            _emails = new List<string>();
            _addresses = new List<Address>();
        }

        public static User Create(Guid id, string name)
        {
            var user = new User
            {
                Id = id,
                Name = name
            };

            user.Record(new UserCreated(id, name, id.ToString()));

            return user;
        }

        public string Name { get; private set; }

        public IEnumerable<string> Emails => _emails;

        public IEnumerable<Address> Addresses => _addresses;

        public User ChangeName(string name)
        {
            Name = name;
            return this;
        }

        public User AddEmail(string email)
        {
            _emails.Add(email);
            return this;
        }

        public User AddAddress(Address address)
        {
            _addresses.Add(address);
            return this;
        }
    }
}
