using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefTag;

namespace RefTagTests
{
    [TestClass]
    public class RefTagTests
    {
        [TestMethod]
        public void Create_New_Tag()
        {
            // Arrange
            string tagName = "New tag";
            string expectedTagName = "New tag";

            //Act
            Tag tag = new Tag(tagName);

            //Assert
            Assert.AreEqual(expectedTagName, tag.TagName, "Tags wasn't assigned a correct Tag Name");
        }
        
        [TestMethod]
        public void Add_Tag_To_Current_Session()
        {
            // Arrange
            CurrentSession currentSession = new CurrentSession();
            string tagName = "New tag";

            // Act
            currentSession.AddTag(new Tag(tagName));

            //Assert
            Assert.AreEqual(tagName, currentSession.Tags[0].TagName, "Tag wasn't added to Tag list correctly");
        }

        [TestMethod]
        public void Recognize_Tag_As_Folder()
        {
            // Arrange
            string tagName = "New tag";
            string tagPath = "New Path";
            bool expectedValue = true;

            //Act
            Tag tag = new Tag(tagName, tagPath);

            //Assert
            Assert.AreEqual(expectedValue, tag.IsFolder, "Tag wasn't recognized as folder");
        }
    }
}
