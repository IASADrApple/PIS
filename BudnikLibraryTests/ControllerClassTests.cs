using Microsoft.VisualStudio.TestTools.UnitTesting;
using BudnikLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudnikLibrary.Tests
{
    [TestClass()]
    public class ControllerClassTests
    {
        [TestMethod()]
        public void AddRemoveAuthorTest()
        {
            try
            {
                ControllerClass controller = new ControllerClass();
                controller.AddAuthor("Allah", "Jahid");

                var sAuthor = controller.FindAuthors("Jahid");
                Assert.AreEqual(sAuthor.Count(), 1);

                Assert.AreEqual("Allah", sAuthor[0].Name);
                Assert.AreEqual("Jahid", sAuthor[0].Sorname);

                controller.RemoveAuthor(sAuthor[0].ItemId);

                sAuthor = controller.FindAuthors("Jahid");
                Assert.AreEqual(sAuthor.Count(), 0);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod()]
        public void BookTest()
        {
            try
            {
                ControllerClass controller = new ControllerClass();

                //Author
                controller.AddAuthor("Allah", "Jahid");
                var sAuthor = controller.FindAuthors("Jahid");
                Assert.AreEqual(sAuthor.Count(), 1, "Auhtor not found");

                Assert.AreEqual("Allah", sAuthor[0].Name);
                Assert.AreEqual("Jahid", sAuthor[0].Sorname);

                //Book
                controller.AddBook(sAuthor[0].ItemId, "TestBook", 1995);
                var sBook = controller.FindBooks("Jahid");
                Assert.AreEqual(sBook.Count(), 1, "Book not found");

                Assert.AreEqual("TestBook", sBook[0].Name);
                Assert.AreEqual(1995, sBook[0].Year);

                //Book Copy
                controller.AddCopy(sBook[0].ItemId, "TestBookCopy1");
                var sBookCopy = controller.FindCopies("TestBookCopy1");
                Assert.AreEqual(sBookCopy.Count(), 1, "Book Copy not found");

                Assert.AreEqual(sBook[0].ItemId, sBookCopy[0].Book);
                Assert.AreEqual("TestBookCopy1", sBookCopy[0].ID);

                //Reader 
                controller.AddReader("John", "Smith", "Japan, Tokyo 12, 5", 0960644445);
                var sReader = controller.FindReaders("John");
                Assert.AreEqual(sReader.Count(), 1, "Reader not found");

                Assert.AreEqual(sReader[0].Name, "John");
                Assert.AreEqual(sReader[0].Sorname, "Smith");
                Assert.AreEqual(sReader[0].Adress, "Japan, Tokyo 12, 5");
                Assert.AreEqual(sReader[0].TelNumber, 0960644445);

                //Renting book
                controller.RentBook(sBookCopy[0].ItemId, sReader[0].ItemId);
                Assert.AreEqual(sBookCopy[0].OnHand, sReader[0].ItemId);

                //Return book
                controller.ReturnBook(sBookCopy[0].ItemId);
                Assert.AreEqual(sBookCopy[0].OnHand, 0);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}