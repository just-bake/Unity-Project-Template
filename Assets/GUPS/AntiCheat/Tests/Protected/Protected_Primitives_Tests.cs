// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;

namespace GUPS.AntiCheat.Tests
{
    public class Protected_Primitives_Tests
    {

#if UNITY_EDITOR

        [SetUp]
        public void Setup_Global_Settings()
        {
            GUPS.AntiCheat.Settings.GlobalSettings.LoadOrCreateAsset();
        }

#endif

        [Test]
        public void Protected_Int16_Test()
        {
            // Arrange
            ProtectedInt16 protectedInt16 = new ProtectedInt16(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedInt16.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedInt16.Value);

            // Assert - Greater Than
            Assert.Greater(protectedInt16.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedInt16.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedInt16.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedInt16.Value, 10);

            // Arrange - Set Value
            protectedInt16.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedInt16.Value);
        }

        [Test]
        public void Protected_Int32_Test()
        {
            // Arrange
            ProtectedInt32 protectedInt32 = new ProtectedInt32(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedInt32.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedInt32.Value);

            // Assert - Greater Than
            Assert.Greater(protectedInt32.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedInt32.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedInt32.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedInt32.Value, 10);

            // Arrange - Set Value
            protectedInt32.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedInt32.Value);
        }

        [Test]
        public void Protected_Int64_Test()
        {
            // Arrange
            ProtectedInt64 protectedInt64 = new ProtectedInt64(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedInt64.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedInt64.Value);

            // Assert - Greater Than
            Assert.Greater(protectedInt64.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedInt64.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedInt64.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedInt64.Value, 10);

            // Arrange - Set Value
            protectedInt64.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedInt64.Value);
        }

        [Test]
        public void Protected_UInt16_Test()
        {
            // Arrange
            ProtectedUInt16 protectedUInt16 = new ProtectedUInt16(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedUInt16.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedUInt16.Value);

            // Assert - Greater Than
            Assert.Greater(protectedUInt16.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedUInt16.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedUInt16.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedUInt16.Value, 10);

            // Arrange - Set Value
            protectedUInt16.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedUInt16.Value);
        }

        [Test]
        public void Protected_UInt32_Test()
        {
            // Arrange
            ProtectedUInt32 protectedUInt32 = new ProtectedUInt32(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedUInt32.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedUInt32.Value);

            // Assert - Greater Than
            Assert.Greater(protectedUInt32.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedUInt32.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedUInt32.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedUInt32.Value, 10);

            // Arrange - Set Value
            protectedUInt32.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedUInt32.Value);
        }

        [Test]
        public void Protected_UInt64_Test()
        {
            // Arrange
            ProtectedUInt64 protectedUInt64 = new ProtectedUInt64(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedUInt64.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedUInt64.Value);

            // Assert - Greater Than
            Assert.Greater(protectedUInt64.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedUInt64.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedUInt64.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedUInt64.Value, 10);

            // Arrange - Set Value
            protectedUInt64.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedUInt64.Value);
        }

        [Test]
        public void Protected_Float_Test()
        {
            // Arrange
            ProtectedFloat protectedFloat = new ProtectedFloat(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedFloat.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedFloat.Value);

            // Assert - Greater Than
            Assert.Greater(protectedFloat.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedFloat.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedFloat.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedFloat.Value, 10);

            // Arrange - Set Value
            protectedFloat.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedFloat.Value);
        }

        [Test]
        public void Protected_Double_Test()
        {
            // Arrange
            ProtectedDouble protectedDouble = new ProtectedDouble(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedDouble.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedDouble.Value);

            // Assert - Greater Than
            Assert.Greater(protectedDouble.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedDouble.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedDouble.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedDouble.Value, 10);

            // Arrange - Set Value
            protectedDouble.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedDouble.Value);
        }

        [Test]
        public void Protected_Decimal_Test()
        {
            // Arrange
            ProtectedDecimal protectedDecimal = new ProtectedDecimal(10);

            // Assert - Equals
            Assert.AreEqual(10, protectedDecimal.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(20, protectedDecimal.Value);

            // Assert - Greater Than
            Assert.Greater(protectedDecimal.Value, 5);

            // Assert - Greater Than Or Equal
            Assert.GreaterOrEqual(protectedDecimal.Value, 10);

            // Assert - Less Than
            Assert.Less(protectedDecimal.Value, 20);

            // Assert - Less Than Or Equal
            Assert.LessOrEqual(protectedDecimal.Value, 10);

            // Arrange - Set Value
            protectedDecimal.Value = 20;

            // Assert - Equals
            Assert.AreEqual(20, protectedDecimal.Value);
        }

        [Test]
        public void Protected_Bool_Test()
        {
            // Arrange
            ProtectedBool protectedBool = new ProtectedBool(true);

            // Assert - Equals
            Assert.AreEqual(true, protectedBool.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(false, protectedBool.Value);

            // Arrange - Set Value
            protectedBool.Value = false;

            // Assert - Equals
            Assert.AreEqual(false, protectedBool.Value);
        }

        [Test]
        public void Protected_String_Test()
        {
            // Arrange
            ProtectedString protectedString = new ProtectedString("Hello World");

            // Assert - Equals
            Assert.AreEqual("Hello World", protectedString.Value);

            // Assert - Not Equals
            Assert.AreNotEqual("Hello World 2", protectedString.Value);

            // Arrange - Set Value
            protectedString.Value = "Hello World 2";

            // Assert - Equals
            Assert.AreEqual("Hello World 2", protectedString.Value);
        }

        [Test]
        public void Protected_Vector2_Test()
        {
            // Arrange
            ProtectedVector2 protectedVector2 = new ProtectedVector2(new Vector2(10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector2(10, 10), protectedVector2.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector2(20, 20), protectedVector2.Value);

            // Arrange - Set Value
            protectedVector2.Value = new Vector2(20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector2(20, 20), protectedVector2.Value);
        }

        [Test]
        public void Protected_Vector2Int_Test()
        {
            // Arrange
            ProtectedVector2Int protectedVector2Int = new ProtectedVector2Int(new Vector2Int(10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector2Int(10, 10), protectedVector2Int.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector2Int(20, 20), protectedVector2Int.Value);

            // Arrange - Set Value
            protectedVector2Int.Value = new Vector2Int(20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector2Int(20, 20), protectedVector2Int.Value);
        }

        [Test]
        public void Protected_Vector3_Test()
        {
            // Arrange
            ProtectedVector3 protectedVector3 = new ProtectedVector3(new Vector3(10, 10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector3(10, 10, 10), protectedVector3.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector3(20, 20, 20), protectedVector3.Value);

            // Arrange - Set Value
            protectedVector3.Value = new Vector3(20, 20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector3(20, 20, 20), protectedVector3.Value);
        }

        [Test]
        public void Protected_Vector3Int_Test()
        {
            // Arrange
            ProtectedVector3Int protectedVector3Int = new ProtectedVector3Int(new Vector3Int(10, 10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector3Int(10, 10, 10), protectedVector3Int.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector3Int(20, 20, 20), protectedVector3Int.Value);

            // Arrange - Set Value
            protectedVector3Int.Value = new Vector3Int(20, 20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector3Int(20, 20, 20), protectedVector3Int.Value);
        }

        [Test]
        public void Protected_Vector4_Test()
        {
            // Arrange
            ProtectedVector4 protectedVector4 = new ProtectedVector4(new Vector4(10, 10, 10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector4(10, 10, 10, 10), protectedVector4.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector4(20, 20, 20, 20), protectedVector4.Value);

            // Arrange - Set Value
            protectedVector4.Value = new Vector4(20, 20, 20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector4(20, 20, 20, 20), protectedVector4.Value);
        }

        [Test]
        public void Protected_Vector4Int_Test()
        {
            // Arrange
            ProtectedVector4Int protectedVector4 = new ProtectedVector4Int(new Vector4(10, 10, 10, 10));

            // Assert - Equals
            Assert.AreEqual(new Vector4(10, 10, 10, 10), protectedVector4.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Vector4(20, 20, 20, 20), protectedVector4.Value);

            // Arrange - Set Value
            protectedVector4.Value = new Vector4(20, 20, 20, 20);

            // Assert - Equals
            Assert.AreEqual(new Vector4(20, 20, 20, 20), protectedVector4.Value);
        }

        [Test]
        public void Protected_Quaternion_Test()
        {
            // Arrange
            ProtectedQuaternion protectedQuaternion = new ProtectedQuaternion(new Quaternion(10, 10, 10, 10));

            // Assert - Equals
            Assert.AreEqual(new Quaternion(10, 10, 10, 10), protectedQuaternion.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new Quaternion(20, 20, 20, 20), protectedQuaternion.Value);

            // Arrange - Set Value
            protectedQuaternion.Value = new Quaternion(20, 20, 20, 20);

            // Assert - Equals
            Assert.AreEqual(new Quaternion(20, 20, 20, 20), protectedQuaternion.Value);
        }

        [Test]
        public void Protected_DateTime_Test()
        {
            // Arrange
            ProtectedDateTime protectedDateTime = new ProtectedDateTime(new DateTime(2020, 1, 1));

            // Assert - Equals
            Assert.AreEqual(new DateTime(2020, 1, 1), protectedDateTime.Value);

            // Assert - Not Equals
            Assert.AreNotEqual(new DateTime(2020, 1, 2), protectedDateTime.Value);

            // Arrange - Set Value
            protectedDateTime.Value = new DateTime(2020, 1, 2);

            // Assert - Equals
            Assert.AreEqual(new DateTime(2020, 1, 2), protectedDateTime.Value);
        }
    }
}
