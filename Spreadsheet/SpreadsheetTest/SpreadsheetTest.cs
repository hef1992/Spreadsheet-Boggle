﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;

namespace SpreadsheetTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("aa", null);
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, 1.0);
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "dd");
        }
        /// <summary>
        /// test argument null
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NullArgument5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, new Formula("1"));
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("A");
        }

        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("AB4a");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("1");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("7d");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("AAA");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A", 1.0);
        }

        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("AB4a", "d");
        }
        /// <summary>
        /// test name is not valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName8()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("1", new Formula("2"));
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", 1.0);
            Assert.AreEqual(1.0, s.GetCellContents("Z7"));
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }
        /// <summary>
        /// test get cell contents
        /// </summary>
        [TestMethod()]
        public void GetCellContent3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("Z7", new Formula("1.0"));
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(1.0, f.Evaluate(v => 1.0));
        }

        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("B1", "A2");
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void nullEmptyCell3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("B1", "A2");
            s.SetCellContents("C3", "Hello");

            List<string> names = new List<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual("B1", names[0]);
            Assert.AreEqual("C3", names[1]);
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void MoreContents1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("C1+C2"));
            s.SetCellContents("B1", new Formula("D1/D2"));
            s.SetCellContents("C2", new Formula("B3*B4"));
        }
        /// <summary>
        /// test get all null empty cell
        /// </summary>
        [TestMethod()]
        public void MoreContents2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("C1+C2"));
            s.SetCellContents("B1", new Formula("D1/D2"));
            s.SetCellContents("C2", new Formula("B3*B4"));
            Assert.AreEqual(s.GetCellContents("R3"), "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestProtectedMethods1()
        {
            PrivateObject sampleAccessor = new PrivateObject(new Spreadsheet());
            object[] parameters = { null };
            sampleAccessor.Invoke("GetDirectDependents", parameters);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestProtectedMethods2()
        {
            PrivateObject sampleAccessor = new PrivateObject(new Spreadsheet());
            object[] parameters = { "AAA" };
            sampleAccessor.Invoke("GetDirectDependents", parameters);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestProtectedMethods3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("C1+C2"));
            s.SetCellContents("B1", new Formula("D1/D2"));
            s.SetCellContents("C2", new Formula("A1*B4"));
        }
    }

}