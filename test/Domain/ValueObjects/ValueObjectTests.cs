﻿using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace SharedKernel.Domain.Tests.ValueObjects
{
    public class ValueObjectTests
    {
        [Fact]
        public void IdenticalDataEqualsIsTrueTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine1", "streetLine2", "city", "zipcode");

            //Act
            var resultEquals = address1.Equals(address2);
            var resultEqualsSimetric = address2.Equals(address1);
            var resultEqualsOnThis = address1.Equals(address1);

            //Assert
            Assert.True(resultEquals);
            Assert.True(resultEqualsSimetric);
            Assert.True(resultEqualsOnThis);
        }

        [Fact]
        public void IdenticalDataEqualOperatorIsTrueTest()
        {
            //Arraneg
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine1", "streetLine2", "city", "zipcode");

            //Act
            var resultEquals = address1 == address2;
            var resultEqualsSimetric = address2 == address1;
            // ReSharper disable once EqualExpressionComparison
            //var resultEqualsOnThis = address1 == address1;

            //Assert
            Assert.True(resultEquals);
            Assert.True(resultEqualsSimetric);
            //Assert.True(resultEqualsOnThis);
        }

        [Fact]
        public void IdenticalDataIsNotEqualOperatorIsFalseTest()
        {
            //Arraneg
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine1", "streetLine2", "city", "zipcode");

            //Act
            var resultEquals = address1 != address2;
            var resultEqualsSimetric = address2 != address1;
            // ReSharper disable once EqualExpressionComparison
            //var resultEqualsOnThis = address1 != address1;

            //Assert
            Assert.False(resultEquals);
            Assert.False(resultEqualsSimetric);
            //Assert.False(resultEqualsOnThis);
        }

        [Fact]
        public void DiferentDataEqualsIsFalseTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine2", "streetLine1", "city", "zipcode");

            //Act
            var result = address1.Equals(address2);
            var resultSimetric = address2.Equals(address1);

            //Assert
            Assert.False(result);
            Assert.False(resultSimetric);
        }

        [Fact]
        public void DiferentDataIsNotEqualOperatorIsTrueTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine2", "streetLine1", "city", "zipcode");

            //Act
            var result = address1 != address2;
            var resultSimetric = address2 != address1;

            //Assert
            Assert.True(result);
            Assert.True(resultSimetric);
        }

        [Fact]
        public void DiferentDataEqualOperatorIsFalseTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", "city", "zipcode");
            var address2 = new Address("streetLine2", "streetLine1", "city", "zipcode");

            //Act
            var result = address1 == address2;
            var resultSimetric = address2 == address1;

            //Assert
            Assert.False(result);
            Assert.False(resultSimetric);
        }

        [Fact]
        public void SameDataInDiferentPropertiesIsEqualsFalseTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", null, null);
            var address2 = new Address("streetLine2", "streetLine1", null, null);

            //Act
            var result = address1.Equals(address2);


            //Assert
            Assert.False(result);
        }

        [Fact]
        public void SameDataInDiferentPropertiesEqualOperatorFalseTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", null, null);
            var address2 = new Address("streetLine2", "streetLine1", null, null);

            //Act
            var result = address1 == address2;


            //Assert
            Assert.False(result);
        }

        [Fact]
        public void NullableValueObjectEqualTrueTest()
        {
            //Arrange
            Address address1 = null;
            Address address2 = null;

            //Act
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var result = address1 == address2;


            //Assert
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            result.Should().BeTrue();
        }

        [Fact]
        public void NullablePropertiesValueObjectEqualTrueTest()
        {
            //Arrange
            var address1 = new Address(null, null, null, null);
            var address2 = new Address(null, null, null, null);

            //Act
            var result = address1 == address2;


            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void DiferentDataInDiferentPropertiesProduceDiferentHashCodeTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", null, null);
            var address2 = new Address("streetLine2", "streetLine1", null, null);

            //Act
            var address1HashCode = address1.GetHashCode();
            var address2HashCode = address2.GetHashCode();


            //Assert
            Assert.NotEqual(address1HashCode, address2HashCode);
        }

        [Fact]
        public void SameDataInDiferentPropertiesProduceDiferentHashCodeTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", null, null, "streetLine1");
            var address2 = new Address(null, "streetLine1", "streetLine1", null);

            //Act
            var address1HashCode = address1.GetHashCode();
            var address2HashCode = address2.GetHashCode();


            //Assert
            Assert.NotEqual(address1HashCode, address2HashCode);
        }

        [Fact]
        public void SameReferenceEqualsTrueTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", null, null, "streetLine1");
            var address2 = address1;


            //Act
            if (!address1.Equals(address2))
                Assert.True(false, "error");

            if (!(address1 == address2))
                Assert.True(false, "error");

        }

        [Fact]
        public void SameDataSameHashCodeTest()
        {
            //Arrange
            var address1 = new Address("streetLine1", "streetLine2", null, null);
            var address2 = new Address("streetLine1", "streetLine2", null, null);

            //Act
            var address1HashCode = address1.GetHashCode();
            var address2HashCode = address2.GetHashCode();


            //Assert
            Assert.Equal(address1HashCode, address2HashCode);
        }

        [Fact]
        public void SelfReferenceNotProduceInfiniteLoop()
        {
            //Arrange
            var aReference = new SelfReference();
            var bReference = new SelfReference();

            //Act
            aReference.Value = bReference;
            bReference.Value = aReference;

            //Assert

            Assert.NotEqual(aReference, bReference);
        }

        [Fact]
        public void AllNullPropertiesEquals()
        {
            //Arrange
            var address1 = new Address(null, null, null, null);
            var address2 = new Address(null, null, null, null);

            //Act

            //Assert
            Assert.Equal(address1, address2);
        }

        [Fact]
        public void SameListAreEquals()
        {
            //Arrange
            var integers1 = new Integers
            {
                Ints = new List<int> { 1, 3 }
            };
            var integers2 = new Integers
            {
                Ints = new List<int> { 1, 3 }
            };

            //Act

            //Assert
            Assert.True(integers1.Equals(integers2));
            Assert.Equal(integers1, integers2);
        }


        [Fact]
        public void SameListDistinctOrderAreEquals()
        {
            //Arrange
            var integers1 = new Integers
            {
                Ints = new List<int> { 3, 1 }
            };
            var integers2 = new Integers
            {
                Ints = new List<int> { 1, 3 }
            };

            //Act

            //Assert
            Assert.True(integers1.Equals(integers2));
            Assert.Equal(integers1, integers2);
        }

        [Fact]
        public void IdenticalDataIsNotEqualOperatorIsFalseTestWithList()
        {
            //Arraneg
            var integers1 = new Integers
            {
                Ints = new List<int> { 3, 1 }
            };
            var integers2 = new Integers
            {
                Ints = new List<int> { 1, 3 }
            };

            //Act
            var resultEquals = integers1 != integers2;
            var resultEqualsSimetric = integers2 != integers1;
            // ReSharper disable once EqualExpressionComparison
            //var resultEqualsOnThis = address1 != address1;

            //Assert
            Assert.False(resultEquals);
            Assert.False(resultEqualsSimetric);
            //Assert.False(resultEqualsOnThis);
        }
    }
}
