using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace eFormSDK.Tests
{
    [TestFixture]
    public class LanguagesUTest : DbTestFixture
    {
        [Test]
        public async Task Languages_Create_DoesCreate()
        {
            //Arrange
            
            languages language = new languages();
            language.Description = Guid.NewGuid().ToString();
            language.Name = Guid.NewGuid().ToString();
            
            //Act
            
            await language.Create(dbContext);
            
            List<languages> languages = dbContext.languages.AsNoTracking().ToList();
            List<language_versions> languageVersions = dbContext.language_versions.AsNoTracking().ToList();
            
            Assert.NotNull(languages);                                                             
            Assert.NotNull(languageVersions);                                                             

            Assert.AreEqual(1,languages.Count());  
            Assert.AreEqual(1,languageVersions.Count());  
            
            Assert.AreEqual(language.CreatedAt.ToString(), languages[0].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languages[0].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languages[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languages[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languages[0].Id);
            Assert.AreEqual(language.Description, languages[0].Description);
            Assert.AreEqual(language.Name, languages[0].Name);
            
            Assert.AreEqual(language.CreatedAt.ToString(), languageVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languageVersions[0].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languageVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languageVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languageVersions[0].Id);
            Assert.AreEqual(language.Description, languageVersions[0].Description);
            Assert.AreEqual(language.Name, languageVersions[0].Name);
        }

        [Test]
        public async Task Languages_Update_DoesUpdate()
        {
            //Arrange
            
            languages language = new languages();
            language.Description = Guid.NewGuid().ToString();
            language.Name = Guid.NewGuid().ToString();
            await language.Create(dbContext);
            
            //Act
            DateTime? oldUpdatedAt = language.UpdatedAt;
            string oldDescription = language.Description;
            string oldName = language.Name;
            
            language.Description = Guid.NewGuid().ToString();
            language.Name = Guid.NewGuid().ToString();
            await language.Update(dbContext);
            
            
            List<languages> languages = dbContext.languages.AsNoTracking().ToList();
            List<language_versions> languageVersions = dbContext.language_versions.AsNoTracking().ToList();
            
            Assert.NotNull(languages);                                                             
            Assert.NotNull(languageVersions);                                                             

            Assert.AreEqual(1,languages.Count());  
            Assert.AreEqual(2,languageVersions.Count());  
            
            Assert.AreEqual(language.CreatedAt.ToString(), languages[0].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languages[0].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languages[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languages[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languages[0].Id);
            Assert.AreEqual(language.Description, languages[0].Description);
            Assert.AreEqual(language.Name, languages[0].Name);
            
            //Old Version
            Assert.AreEqual(language.CreatedAt.ToString(), languageVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(1, languageVersions[0].Version);                                      
//            Assert.AreEqual(oldUpdatedAt.ToString(), languageVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languageVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languageVersions[0].LanguageId);
            Assert.AreEqual(oldDescription, languageVersions[0].Description);
            Assert.AreEqual(oldName, languageVersions[0].Name);
            
            //New Version
            Assert.AreEqual(language.CreatedAt.ToString(), languageVersions[1].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languageVersions[1].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languageVersions[1].UpdatedAt.ToString());                                  
            Assert.AreEqual(languageVersions[1].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languageVersions[1].LanguageId);
            Assert.AreEqual(language.Description, languageVersions[1].Description);
            Assert.AreEqual(language.Name, languageVersions[1].Name);
        }
        [Test]
        public async Task Languages_Delete_DoesSetWorkflowStateToRemoved()
        {
            //Arrange
            
            languages language = new languages();
            language.Description = Guid.NewGuid().ToString();
            language.Name = Guid.NewGuid().ToString();
            await language.Create(dbContext);
            
            //Act
            DateTime? oldUpdatedAt = language.UpdatedAt;
            
            await language.Delete(dbContext);
            
            
            List<languages> languages = dbContext.languages.AsNoTracking().ToList();
            List<language_versions> languageVersions = dbContext.language_versions.AsNoTracking().ToList();
            
            Assert.NotNull(languages);                                                             
            Assert.NotNull(languageVersions);                                                             

            Assert.AreEqual(1,languages.Count());  
            Assert.AreEqual(2,languageVersions.Count());  
            
            Assert.AreEqual(language.CreatedAt.ToString(), languages[0].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languages[0].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languages[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languages[0].WorkflowState, Constants.WorkflowStates.Removed);
            Assert.AreEqual(language.Id, languages[0].Id);
            Assert.AreEqual(language.Description, languages[0].Description);
            Assert.AreEqual(language.Name, languages[0].Name);
            
            //Old Version
            Assert.AreEqual(language.CreatedAt.ToString(), languageVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(1, languageVersions[0].Version);                                      
//            Assert.AreEqual(oldUpdatedAt.ToString(), languageVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(languageVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(language.Id, languageVersions[0].LanguageId);
            Assert.AreEqual(language.Description, languageVersions[0].Description);
            Assert.AreEqual(language.Name, languageVersions[0].Name);
            
            //New Version
            Assert.AreEqual(language.CreatedAt.ToString(), languageVersions[1].CreatedAt.ToString());                                  
            Assert.AreEqual(language.Version, languageVersions[1].Version);                                      
//            Assert.AreEqual(language.UpdatedAt.ToString(), languageVersions[1].UpdatedAt.ToString());                                  
            Assert.AreEqual(languageVersions[1].WorkflowState, Constants.WorkflowStates.Removed);
            Assert.AreEqual(language.Id, languageVersions[1].LanguageId);
            Assert.AreEqual(language.Description, languageVersions[1].Description);
            Assert.AreEqual(language.Name, languageVersions[1].Name);
        }
    }
}