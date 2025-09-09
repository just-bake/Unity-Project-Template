// System
using System;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Core.Binary;

namespace GUPS.AntiCheat.Tests
{
    /// <summary>
    /// Test fixture for testing binary reading and writing operations.
    /// </summary>
    /// <remarks>
    /// Contains tests for various data types including primitives and Unity-specific structures.
    /// </remarks>
    [TestFixture]
    public class ReaderWriter_Test
    {
        /// <summary>
        /// Binary writer instance used for testing.
        /// </summary>
        private BinaryWriter _Writer;

        /// <summary>
        /// Binary reader instance used for testing.
        /// </summary>
        private BinaryReader _Reader;

        /// <summary>
        /// Initializes the test environment.
        /// </summary>
        /// <remarks>
        /// Creates new instances of binary reader and writer before each test.
        /// </remarks>
        [SetUp]
        public void Setup()
        {
            _Writer = new BinaryWriter();
        }

        /// <summary>
        /// Tests reading and writing of primitive data types.
        /// </summary>
        /// <typeparam name="T">The type of primitive value to test.</typeparam>
        /// <param name="_Value">The test value to write and read.</param>
        /// <remarks>
        /// Covers byte, bool, short, int, long, ushort, uint, ulong, float, double, char, and string types..
        /// </remarks>
        [TestCase((byte)123)]
        [TestCase((bool)true)]
        [TestCase((short)-12345)]
        [TestCase((int)123456)]
        [TestCase((long)9876543210)]
        [TestCase((ushort)54321)]
        [TestCase((uint)123456789)]
        [TestCase((ulong)9876543210)]
        [TestCase((float)12.34f)]
        [TestCase(56.78)]
        [TestCase('A')]
        [TestCase("Hello, World!")]
        public void PrimitiveTypes_Test<T>(T _Value)
        {
            Type type = typeof(T);
            _Writer.Write(type, _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(type);
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Color structure.
        /// </summary>
        [Test]
        public void Color_Test()
        {
            Color _Value = new Color(0.1f, 0.2f, 0.3f, 0.4f);
            _Writer.Write(typeof(Color), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Color));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Color32 structure.
        /// </summary>
        [Test]
        public void Color32_Test()
        {
            Color32 _Value = new Color32(10, 20, 30, 40);
            _Writer.Write(typeof(Color32), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Color32));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Vector2 structure.
        /// </summary>
        [Test]
        public void Vector2_Test()
        {
            Vector2 _Value = new Vector2(1.1f, 2.2f);
            _Writer.Write(typeof(Vector2), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Vector2));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Vector3 structure.
        /// </summary>
        [Test]
        public void Vector3_Test()
        {
            Vector3 _Value = new Vector3(1.1f, 2.2f, 3.3f);
            _Writer.Write(typeof(Vector3), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Vector3));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Vector4 structure.
        /// </summary>
        [Test]
        public void Vector4_Test()
        {
            Vector4 _Value = new Vector4(1.1f, 2.2f, 3.3f, 4.4f);
            _Writer.Write(typeof(Vector4), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Vector4));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Quaternion structure.
        /// </summary>
        [Test]
        public void Quaternion_Test()
        {
            Quaternion _Value = new Quaternion(0.1f, 0.2f, 0.3f, 0.4f);
            _Writer.Write(typeof(Quaternion), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Quaternion));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Rect structure.
        /// </summary>
        [Test]
        public void Rect_Test()
        {
            Rect _Value = new Rect(1.1f, 2.2f, 3.3f, 4.4f);
            _Writer.Write(typeof(Rect), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Rect));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Plane structure.
        /// </summary>
        [Test]
        public void Plane_Test()
        {
            Plane _Value = new Plane(new Vector3(0.1f, 0.2f, 0.3f), 4.5f);
            _Writer.Write(typeof(Plane), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Plane));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Ray structure.
        /// </summary>
        [Test]
        public void Ray_Test()
        {
            Ray _Value = new Ray(new Vector3(0.46f, 0.57f, 0.68f), new Vector3(0.27f, 0.53f, 0.80f));
            _Writer.Write(typeof(Ray), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Ray));
            Assert.AreEqual(_Value, result);
        }

        /// <summary>
        /// Tests reading and writing of Matrix4x4 structure.
        /// </summary>
        [Test]
        public void Matrix4x4_Test()
        {
            Matrix4x4 _Value = new Matrix4x4(
                new Vector4(1, 2, 3, 4),
                new Vector4(5, 6, 7, 8),
                new Vector4(9, 10, 11, 12),
                new Vector4(13, 14, 15, 16));
            _Writer.Write(typeof(Matrix4x4), _Value);
            _Reader = new BinaryReader(_Writer.ToArray());
            object result = _Reader.Read(typeof(Matrix4x4));
            Assert.AreEqual(_Value, result);
        }
    }
}
