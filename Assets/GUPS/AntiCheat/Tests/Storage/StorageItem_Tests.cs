// System
using System;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Core.Binary;
using GUPS.AntiCheat.Core.Storage;

namespace GUPS.AntiCheat.Tests
{
    /// <summary>
    /// Test fixture for testing storage item operations.
    /// </summary>
    /// <remarks>
    /// Contains tests for various data types including primitives and Unity-specific structures.
    /// </remarks>
    [TestFixture]
    public class StorageItem_Tests
    {
        [Test]
        public void PrimitiveTypes_Test()
        {
            // Test byte
            StorageItem item = new StorageItem((byte)1);
            Assert.AreEqual(EStorageType.Byte, item.Type);
            Assert.AreEqual((byte)1, item.Value);

            // Test byte array.
            byte[] array = new byte[] { 1, 2, 3, 4, 5 };
            item = new StorageItem(array);
            Assert.AreEqual(EStorageType.ByteArray, item.Type);
            CollectionAssert.AreEqual(array, (byte[])item.Value);

            // Test bool
            item = new StorageItem(true);
            Assert.AreEqual(EStorageType.Boolean, item.Type);
            Assert.AreEqual(true, item.Value);

            // Test short
            item = new StorageItem((short)1);
            Assert.AreEqual(EStorageType.Int16, item.Type);
            Assert.AreEqual((short)1, item.Value);

            // Test int
            item = new StorageItem(1);
            Assert.AreEqual(EStorageType.Int32, item.Type);
            Assert.AreEqual(1, item.Value);

            // Test long
            item = new StorageItem((long)1);
            Assert.AreEqual(EStorageType.Int64, item.Type);
            Assert.AreEqual((long)1, item.Value);

            // Test ushort
            item = new StorageItem((ushort)1);
            Assert.AreEqual(EStorageType.UInt16, item.Type);
            Assert.AreEqual((ushort)1, item.Value);

            // Test uint
            item = new StorageItem((uint)1);
            Assert.AreEqual(EStorageType.UInt32, item.Type);
            Assert.AreEqual((uint)1, item.Value);

            // Test ulong
            item = new StorageItem((ulong)1);
            Assert.AreEqual(EStorageType.UInt64, item.Type);
            Assert.AreEqual((ulong)1, item.Value);

            // Test float
            item = new StorageItem(1.0f);
            Assert.AreEqual(EStorageType.Single, item.Type);
            Assert.AreEqual(1.0f, item.Value);

            // Test double
            item = new StorageItem(1.0);
            Assert.AreEqual(EStorageType.Double, item.Type);
            Assert.AreEqual(1.0, item.Value);

            // Test decimal
            item = new StorageItem((decimal)1.0);
            Assert.AreEqual(EStorageType.Decimal, item.Type);
            Assert.AreEqual((decimal)1.0, item.Value);

            // Test char
            item = new StorageItem('a');
            Assert.AreEqual(EStorageType.Char, item.Type);
            Assert.AreEqual('a', item.Value);
        }

        [Test]
        public void UnityTypes_Test()
        {
            // Test Color
            StorageItem item = new StorageItem(Color.red);
            Assert.AreEqual(EStorageType.Color, item.Type);
            Assert.AreEqual(Color.red, item.Value);

            // Test Color32
            item = new StorageItem(new Color32(255, 0, 0, 255));
            Assert.AreEqual(EStorageType.Color32, item.Type);
            Assert.AreEqual(new Color32(255, 0, 0, 255), item.Value);

            // Test Vector2
            item = new StorageItem(Vector2.zero);
            Assert.AreEqual(EStorageType.Vector2, item.Type);
            Assert.AreEqual(Vector2.zero, item.Value);

            // Test Vector2Int
            item = new StorageItem(Vector2Int.zero);
            Assert.AreEqual(EStorageType.Vector2Int, item.Type);
            Assert.AreEqual(Vector2Int.zero, item.Value);

            // Test Vector3
            item = new StorageItem(Vector3.zero);
            Assert.AreEqual(EStorageType.Vector3, item.Type);
            Assert.AreEqual(Vector3.zero, item.Value);

            // Test Vector3Int
            item = new StorageItem(Vector3Int.zero);
            Assert.AreEqual(EStorageType.Vector3Int, item.Type);
            Assert.AreEqual(Vector3Int.zero, item.Value);

            // Test Vector4
            item = new StorageItem(Vector4.zero);
            Assert.AreEqual(EStorageType.Vector4, item.Type);
            Assert.AreEqual(Vector4.zero, item.Value);

            // Test Quaternion
            item = new StorageItem(Quaternion.identity);
            Assert.AreEqual(EStorageType.Quaternion, item.Type);
            Assert.AreEqual(Quaternion.identity, item.Value);

            // Test Rect
            item = new StorageItem(Rect.zero);
            Assert.AreEqual(EStorageType.Rect, item.Type);
            Assert.AreEqual(Rect.zero, item.Value);

            // Test Plane
            item = new StorageItem(new Plane(Vector3.up, Vector3.up, Vector3.up));
            Assert.AreEqual(EStorageType.Plane, item.Type);
            Assert.AreEqual(new Plane(Vector3.up, Vector3.up, Vector3.up), item.Value);

            // Test Ray
            item = new StorageItem(new Ray(Vector3.zero, Vector3.forward));
            Assert.AreEqual(EStorageType.Ray, item.Type);
            Assert.AreEqual(new Ray(Vector3.zero, Vector3.forward), item.Value);

            // Test Matrix4x4
            item = new StorageItem(Matrix4x4.identity);
            Assert.AreEqual(EStorageType.Matrix4x4, item.Type);
            Assert.AreEqual(Matrix4x4.identity, item.Value);
        }

        [Test]
        public void ReadWrite_Test()
        {
            // Create a storage item
            StorageItem item = new StorageItem(1);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void ByteArray_Test()
        {
            // Create a byte array
            byte[] array = new byte[] { 1, 2, 3, 4, 5 };

            // Create a storage item from the byte array
            StorageItem item = new StorageItem(array);

            // Check that the item's type is ByteArray
            Assert.AreEqual(EStorageType.ByteArray, item.Type);

            // Check that the item's value is the byte array
            Assert.AreEqual(array, item.Value);

            // Write the item to a binary writer
            GUPS.AntiCheat.Core.Binary.BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void String_Test()
        {
            // Create a String
            String str = "Hello, World!";

            // Create a storage item from the String
            StorageItem item = new StorageItem(str);

            // Check that the item's type is String
            Assert.AreEqual(EStorageType.String, item.Type);

            // Check that the item's value is the String
            Assert.AreEqual(str, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Boolean_Test()
        {
            // Create a boolean
            bool value = true;

            // Create a storage item from the boolean
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Boolean
            Assert.AreEqual(EStorageType.Boolean, item.Type);

            // Check that the item's value is the boolean
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Int16_Test()
        {
            // Create a short
            short value = 1;

            // Create a storage item from the short
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Int16
            Assert.AreEqual(EStorageType.Int16, item.Type);

            // Check that the item's value is the short
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Int32_Test()
        {
            // Create an integer
            int value = 1;

            // Create a storage item from the integer
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Int32
            Assert.AreEqual(EStorageType.Int32, item.Type);

            // Check that the item's value is the integer
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Int64_Test()
        {
            // Create a long
            long value = 1;

            // Create a storage item from the long
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Int64
            Assert.AreEqual(EStorageType.Int64, item.Type);

            // Check that the item's value is the long
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void UInt16_Test()
        {
            // Create an unsigned short
            ushort value = 1;

            // Create a storage item from the unsigned short
            StorageItem item = new StorageItem(value);

            // Check that the item's type is UInt16
            Assert.AreEqual(EStorageType.UInt16, item.Type);

            // Check that the item's value is the unsigned short
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void UInt32_Test()
        {
            // Create an unsigned integer
            uint value = 1;

            // Create a storage item from the unsigned integer
            StorageItem item = new StorageItem(value);

            // Check that the item's type is UInt32
            Assert.AreEqual(EStorageType.UInt32, item.Type);

            // Check that the item's value is the unsigned integer
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void UInt64_Test()
        {
            // Create an unsigned long
            ulong value = 1;

            // Create a storage item from the unsigned long
            StorageItem item = new StorageItem(value);

            // Check that the item's type is UInt64
            Assert.AreEqual(EStorageType.UInt64, item.Type);

            // Check that the item's value is the unsigned long
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Single_Test()
        {
            // Create a float
            float value = 1.0f;

            // Create a storage item from the float
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Single
            Assert.AreEqual(EStorageType.Single, item.Type);

            // Check that the item's value is the float
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Double_Test()
        {
            // Create a double
            double value = 1.0;

            // Create a storage item from the double
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Double
            Assert.AreEqual(EStorageType.Double, item.Type);

            // Check that the item's value is the double
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Decimal_Test()
        {
            // Create a decimal
            decimal value = 1.0m;

            // Create a storage item from the decimal
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Decimal
            Assert.AreEqual(EStorageType.Decimal, item.Type);

            // Check that the item's value is the decimal
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Char_Test()
        {
            // Create a char
            char value = 'a';

            // Create a storage item from the char
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Char
            Assert.AreEqual(EStorageType.Char, item.Type);

            // Check that the item's value is the char
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Color_Test()
        {
            // Create a color
            Color value = Color.red;

            // Create a storage item from the color
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Color
            Assert.AreEqual(EStorageType.Color, item.Type);

            // Check that the item's value is the color
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Color32_Test()
        {
            // Create a color32
            Color32 value = new Color32(255, 0, 0, 255);

            // Create a storage item from the color32
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Color32
            Assert.AreEqual(EStorageType.Color32, item.Type);

            // Check that the item's value is the color32
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Vector2_Test()
        {
            // Create a vector2
            Vector2 value = new Vector2(1, 2);

            // Create a storage item from the vector2
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Vector2
            Assert.AreEqual(EStorageType.Vector2, item.Type);

            // Check that the item's value is the vector2
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Vector2Int_Test()
        {
            // Create a vector2int
            Vector2Int value = new Vector2Int(1, 2);

            // Create a storage item from the vector2int
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Vector2Int
            Assert.AreEqual(EStorageType.Vector2Int, item.Type);

            // Check that the item's value is the vector2int
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Vector3_Test()
        {
            // Create a vector3
            Vector3 value = new Vector3(1, 2, 3);

            // Create a storage item from the vector3
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Vector3
            Assert.AreEqual(EStorageType.Vector3, item.Type);

            // Check that the item's value is the vector3
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Vector3Int_Test()
        {
            // Create a vector3int
            Vector3Int value = new Vector3Int(1, 2, 3);

            // Create a storage item from the vector3int
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Vector3Int
            Assert.AreEqual(EStorageType.Vector3Int, item.Type);

            // Check that the item's value is the vector3int
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Vector4_Test()
        {
            // Create a vector4
            Vector4 value = new Vector4(1, 2, 3, 4);

            // Create a storage item from the vector4
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Vector4
            Assert.AreEqual(EStorageType.Vector4, item.Type);

            // Check that the item's value is the vector4
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Quaternion_Test()
        {
            // Create a quaternion
            Quaternion value = new Quaternion(1, 2, 3, 4);

            // Create a storage item from the quaternion
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Quaternion
            Assert.AreEqual(EStorageType.Quaternion, item.Type);

            // Check that the item's value is the quaternion
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Rect_Test()
        {
            // Create a rect
            Rect value = new Rect(1, 2, 3, 4);

            // Create a storage item from the rect
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Rect
            Assert.AreEqual(EStorageType.Rect, item.Type);

            // Check that the item's value is the rect
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Plane_Test()
        {
            // Create a plane
            Plane value = new Plane(Vector3.up, Vector3.up, Vector3.up);

            // Create a storage item from the plane
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Plane
            Assert.AreEqual(EStorageType.Plane, item.Type);

            // Check that the item's value is the plane
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Ray_Test()
        {
            // Create a ray
            Ray value = new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6));

            // Create a storage item from the ray
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Ray
            Assert.AreEqual(EStorageType.Ray, item.Type);

            // Check that the item's value is the ray
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }

        [Test]
        public void Matrix4x4_Test()
        {
            // Create a matrix4x4
            Matrix4x4 value = new Matrix4x4();

            // Create a storage item from the matrix4x4
            StorageItem item = new StorageItem(value);

            // Check that the item's type is Matrix4x4
            Assert.AreEqual(EStorageType.Matrix4x4, item.Type);

            // Check that the item's value is the matrix4x4
            Assert.AreEqual(value, item.Value);

            // Write the item to a binary writer
            BinaryWriter writer = new BinaryWriter();
            item.Write(writer);

            // Get the binary data from the writer
            byte[] data = writer.ToArray();

            // Read the item from the binary data
            BinaryReader reader = new BinaryReader(data);
            StorageItem readItem = new StorageItem();
            readItem.Read(reader);

            // Check that the read item is equal to the original item
            Assert.AreEqual(item.Type, readItem.Type);
            Assert.AreEqual(item.Value, readItem.Value);
        }
    }
}
