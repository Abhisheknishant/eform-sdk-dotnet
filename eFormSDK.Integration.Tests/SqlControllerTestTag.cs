﻿using eFormCore;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microting.eForm;
using Microting.eForm.Dto;
using Microting.eForm.Helpers;
using Microting.eForm.Infrastructure;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;

namespace eFormSDK.Integration.Tests
{
    [TestFixture]
    public class SqlControllerTestTag : DbTestFixture
    {
        private SqlController sut;
        private TestHelpers testHelpers;
        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:", "");

        public override async Task DoSetup()
        {
            if (sut == null)
            {
                sut = new SqlController(ConnectionString);
                await sut.StartLog(new CoreBase());
            }
            testHelpers = new TestHelpers();
            await sut.SettingUpdate(Settings.fileLocationPicture, Path.Combine(path, "output", "dataFolder", "picture"));
            await sut.SettingUpdate(Settings.fileLocationPdf, Path.Combine(path, "output", "dataFolder", "pdf"));
            await sut.SettingUpdate(Settings.fileLocationJasper, Path.Combine(path, "output", "dataFolder", "reports"));
        }


        #region tag
        [Test]
        public async Task SQL_Tags_CreateTag_DoesCreateNewTag()
        {
            // Arrance
            string tagName = "Tag1";

            // Act
            await sut.TagCreate(tagName);

            // Assert
            var tag = dbContext.tags.ToList();

            Assert.AreEqual(tag[0].Name, tagName);
            Assert.AreEqual(1, tag.Count());
        }

        [Test]
        public async Task SQL_Tags_DeleteTag_DoesMarkTagAsRemoved()
        {
            // Arrance
            string tagName = "Tag1";
            tags tag = new tags();
            tag.Name = tagName;
            tag.WorkflowState = Constants.WorkflowStates.Created;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();

            // Act
            await sut.TagDelete(tag.Id);

            // Assert
            var result = dbContext.tags.AsNoTracking().ToList();

            Assert.AreEqual(result[0].Name, tagName);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Constants.WorkflowStates.Removed, result[0].WorkflowState);
        }

        [Test]
        public async Task SQL_Tags_CreateTag_DoesRecreateRemovedTag()
        {
            // Arrance
            string tagName = "Tag1";
            tags tag = new tags();
            tag.Name = tagName;
            tag.WorkflowState = Constants.WorkflowStates.Removed;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();

            // Act
            await sut.TagCreate(tagName);

            // Assert
            var result = dbContext.tags.AsNoTracking().ToList();

            Assert.AreEqual(result[0].Name, tagName);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Constants.WorkflowStates.Created, result[0].WorkflowState);
        }

        [Test]
        public async Task SQL_Tags_GetAllTags_DoesReturnAllTags()
        {
            // Arrance
            string tagName1 = "Tag1";
            tags tag = new tags();
            tag.Name = tagName1;
            tag.WorkflowState = Constants.WorkflowStates.Removed;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();

            string tagName2 = "Tag2";
            tag = new tags();

            tag.Name = tagName2;
            tag.WorkflowState = Constants.WorkflowStates.Removed;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();
            string tagName3 = "Tag3";
            tag = new tags();

            tag.Name = tagName3;
            tag.WorkflowState = Constants.WorkflowStates.Removed;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();
            //int tagId3 = await sut.TagCreate(tagName3);

            // Act
            var tags = await sut.GetAllTags(true);

            // Assert
            Assert.True(true);
            Assert.AreEqual(3, tags.Count());
            Assert.AreEqual(tagName1, tags[0].Name);
            Assert.AreEqual(0, tags[0].TaggingCount);
            Assert.AreEqual(tagName2, tags[1].Name);
            Assert.AreEqual(0, tags[1].TaggingCount);
            Assert.AreEqual(tagName3, tags[2].Name);
            Assert.AreEqual(0, tags[2].TaggingCount);
        }

        [Test]
        public async Task SQL_Tags_TemplateSetTags_DoesAssignTagToTemplate()
        {
            // Arrance
            check_lists cl1 = new check_lists();
            cl1.CreatedAt = DateTime.Now;
            cl1.UpdatedAt = DateTime.Now;
            cl1.Label = "A";
            cl1.Description = "D";
            cl1.WorkflowState = Constants.WorkflowStates.Created;
            cl1.CaseType = "CheckList";
            cl1.FolderName = "Template1FolderName";
            cl1.DisplayIndex = 1;
            cl1.Repeated = 1;

            dbContext.check_lists.Add(cl1);
            await dbContext.SaveChangesAsync();

            string tagName1 = "Tag1";
            tags tag = new tags();
            tag.Name = tagName1;
            tag.WorkflowState = Constants.WorkflowStates.Created;

            dbContext.tags.Add(tag);
            await dbContext.SaveChangesAsync();

            // Act
            List<int> tags = new List<int>();
            tags.Add(tag.Id);
            await sut.TemplateSetTags(cl1.Id, tags);


            // Assert
            List<taggings> result = dbContext.taggings.AsNoTracking().ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(tag.Id, result[0].TagId);
            Assert.AreEqual(cl1.Id, result[0].CheckListId);
            Assert.True(true);
        }
        #endregion

        
        #region eventhandlers
#pragma warning disable 1998
        public async Task EventCaseCreated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public async Task EventCaseRetrived(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public async Task EventCaseCompleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public async Task EventCaseDeleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public async Task EventFileDownloaded(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public async Task EventSiteActivated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }
#pragma warning restore 1998
        #endregion
    }

}