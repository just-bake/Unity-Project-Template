// System
using System;
using System.Collections.Generic;
using System.IO;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Core.Storage;

namespace GUPS.AntiCheat.Tests
{
    /// <summary>
    /// Test fixture for testing storage container operations.
    /// </summary>
    [TestFixture]
    public class StorageContainer_Tests
    {
        private StorageContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new StorageContainer();
        }

        [Test]
        public void Add_Item_ShouldAddItemToContainer()
        {
            // Arrange
            string key = "Item1";
            object value = 123;

            // Act
            _container.Add(key, value);

            // Assert
            Assert.AreEqual(value, _container.Get(key));
        }

        [Test]
        public void Set_Item_ShouldUpdateExistingItemInContainer()
        {
            // Arrange
            string key = "Item1";
            object initialValue = 123;
            object newValue = 456;
            _container.Add(key, initialValue);

            // Act
            _container.Set(key, newValue);

            // Assert
            Assert.AreEqual(newValue, _container.Get(key));
        }

        [Test]
        public void Remove_Item_ShouldRemoveItemFromContainer()
        {
            // Arrange
            string key = "Item1";
            object value = 123;
            _container.Add(key, value);

            // Act
            _container.Remove(key);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => _container.Get(key));
        }

        [Test]
        public void Get_Item_ShouldReturnCorrectValue()
        {
            // Arrange
            string key = "Item1";
            object value = 123;
            _container.Add(key, value);

            // Act
            var retrievedValue = _container.Get(key);

            // Assert
            Assert.AreEqual(value, retrievedValue);
        }

        [Test]
        public void Get_GenericItem_ShouldReturnCorrectTypedValue()
        {
            // Arrange
            string key = "Item1";
            int value = 123;
            _container.Add(key, value);

            // Act
            int retrievedValue = _container.Get<int>(key);

            // Assert
            Assert.AreEqual(value, retrievedValue);
        }

        [Test]
        public void Owner_SetAndGet_ShouldReturnCorrectOwner()
        {
            // Arrange
            string owner = "TestOwner";

            // Act
            _container.Owner = owner;

            // Assert
            Assert.AreEqual(owner, _container.Owner);
        }

        [Test]
        public void Write_And_Read_ShouldMaintainDataIntegrity()
        {
            // Arrange
            string key = "Item1";
            object value = 123;
            _container.Add(key, value);
            _container.Owner = "TestOwner";

            using (MemoryStream stream = new MemoryStream())
            {
                // Act
                _container.Write(stream);
                stream.Position = 0;  // Reset stream position for reading

                var newContainer = new StorageContainer();
                newContainer.Read(stream);

                // Assert
                Assert.AreEqual(_container.Get(key), newContainer.Get(key));
                Assert.AreEqual(_container.Owner, newContainer.Owner);
            }
        }

        [Test]
        public void Write_ShouldGenerateSignature()
        {
            // Arrange
            string key = "Item1";
            object value = 123;
            _container.Add(key, value);
            _container.Owner = "TestOwner";

            using (MemoryStream stream = new MemoryStream())
            {
                // Act
                _container.Write(stream);

                // Assert
                Assert.IsNotNull(_container.Signature);
                Assert.IsNotEmpty(_container.Signature);
            }
        }

        [Test]
        public void Read_InvalidSignature_ShouldThrowException()
        {
            // Arrange
            string key = "Item1";
            object value = 123;
            _container.Add(key, value);
            _container.Owner = "TestOwner";

            using (MemoryStream stream = new MemoryStream())
            {
                _container.Write(stream);
                stream.Position = 0;

                // Modify the stream to simulate an invalid signature
                byte[] data = stream.ToArray();
                data[4] = 0x01;  // Corrupt the first data byte of the signature
                data[4 + 1] = 0x02;  // Corrupt the second data byte of the signature
                data[4 + 2] = 0x03;  // Corrupt the third data byte of the signature
                MemoryStream corruptedStream = new MemoryStream(data);

                var newContainer = new StorageContainer();

                // Act & Assert
                Assert.Throws<Exception>(() => newContainer.Read(corruptedStream), StorageContainer.ERROR_SIGNATURE);
            }
        }

        [Test]
        public void Add_DuplicateKey_ShouldThrowException()
        {
            // Arrange
            string key = "DuplicateKey";
            object value = 123;
            _container.Add(key, value);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _container.Add(key, 456), StorageContainer.ERROR_DUPLICATE);
        }

        [Test]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            string key = "NonExistentKey";

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _container.Get(key));
        }

        [Test]
        public void Remove_NonExistentKey_ShouldNotThrowException()
        {
            // Arrange
            string key = "NonExistentKey";

            // Act & Assert
            Assert.DoesNotThrow(() => _container.Remove(key));
        }
    }
}
