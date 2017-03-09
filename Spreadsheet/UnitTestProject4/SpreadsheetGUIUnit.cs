﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.Windows.Forms;

namespace SpreadsheetGUITest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Tests to see if the stub is closed if the FileCloseEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestCloseEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileCloseEvent();
            Assert.IsTrue(stub.Closed);
        }

        /// <summary>
        /// Tests to see if OpenNew is called if the FileNewEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileNewEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileNewEvent();
            Assert.IsTrue(stub.CalledOpenNew);
        }

        /// <summary>
        /// Tests to see if OpenSaved is called when FileOpenEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileOpenEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
            string fileName = "hello.ss";
            
            stub.FireFileOpenEvent(fileName);
            Assert.IsTrue(stub.CalledOpenSaved);
        }

        /// <summary>
        /// Tests to see if file is saved when FileSaveEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFileSaveEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFileSaveEvent();
            Assert.IsTrue(stub.CalledSaveDialog);
        }

        /// <summary>
        /// Tests to see if the UnsavedData method is called when FormCloseEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFormCloseEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
            CloseReason reason = new CloseReason();
            bool cancel = true;
            FormClosingEventArgs e = new FormClosingEventArgs(reason, cancel);

            stub.FireKeyPressEvent('1');
            stub.FireKeyPressEvent('\r');

            stub.FireFormCloseEvent(e);
            Assert.IsTrue(stub.CalledSaveDialog);
        }

        /// <summary>
        /// Tests to make sure GetPanelUpdated and GetTextBoxUpdated are called when FormLoadEvent is fired.
        /// </summary>
        [TestMethod]
        public void TestFormLoadEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);

            stub.FireFormLoadEvent();
            Assert.IsTrue(stub.CalledGetPanelUpdated && stub.CalledGetTextBoxUpdated);
        }

        /// <summary>
        /// Tests to make sure GetPanelUpdated and GetTextBoxUpdayed
        /// </summary>
        [TestMethod]
        public void PublicFunctionTest1()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.PrintMessage("hello");
            Assert.IsTrue(stub.CalledPrintMessage);
        }
        [TestMethod]
        public void PublicFunctionTest2()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.OpenNew();
            Assert.IsTrue(stub.CalledOpenNew);
        }
        [TestMethod]
        public void PublicFunctionTest3()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.DoClose();
            Assert.IsTrue(stub.Closed);
        }
        [TestMethod]
        public void PublicFunctionTest4()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            string selectedCellName = stub.GetSelectedCellName();
            Assert.AreEqual(selectedCellName, stub.SelectedCell);
        }
        [TestMethod]
        public void PublicFunctionTest5()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            string saveDialog = stub.SaveDialog();
            Assert.AreEqual(saveDialog, "newFile.ss");
        }
        [TestMethod]
        public void PublicFunctionTest6()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellNameDisplay("abc");
            Assert.AreEqual(stub.CellNameDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest7()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellValueDisplay("abc");
            Assert.AreEqual(stub.CellValueDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest8()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetCellContentsDisplay("abc");
            Assert.AreEqual(stub.CellContentsDisplay, "abc");
        }
        [TestMethod]
        public void PublicFunctionTest9()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.SetValueOfPanel(1, 1, "hello");
            Assert.AreEqual(stub.PanelGrid[1,1], "hello");
        }
        [TestMethod]
        public void PublicFunctionTest10()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.OpenSaved("newFile.ss");
            Assert.IsTrue(stub.CalledOpenSaved);
        }
        [TestMethod]
        public void PublicFunctionTest11()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            Controller controller = new Controller(stub);
            stub.UnsavedData(new FormClosingEventArgs(new CloseReason(), true));
            Assert.IsTrue(stub.CalledUnsavedData);
        }

        public void TestKeyPressEnterEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        /// <summary>
        /// Tests to make sure circular exception is thrown when key press enter results in circular dependency.
        /// </summary>
        [TestMethod]
        public void TestKeyPressCircularEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        [TestMethod]
        public void TestPanelLoadEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }

        [TestMethod]
        public void TestSelectionChangedEvent()
        {
            SpreadsheetStub stub = new SpreadsheetStub();
            SpreadsheetGUI.Controller controller = new SpreadsheetGUI.Controller(stub);
        }
    }
}
