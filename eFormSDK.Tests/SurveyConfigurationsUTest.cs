using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace eFormSDK.Tests
{
    [TestFixture]
    public class SurveyConfigurationsUTest : DbTestFixture
    {
        [Test]
        public void SurveyConfigurations_Create_DoesCreate()
        {
            //Arrange
            Random rnd = new Random();
            
            survey_configurations surveyConfiguration = new survey_configurations();
            surveyConfiguration.Name = Guid.NewGuid().ToString();
            surveyConfiguration.Start = DateTime.Now;
            surveyConfiguration.Stop = DateTime.Now;
            surveyConfiguration.TimeOut = rnd.Next(1, 255);
            surveyConfiguration.TimeToLive = rnd.Next(1, 255);
            
            //Act
            
            surveyConfiguration.Create(DbContext);
            
            List<survey_configurations> surveyConfigurations = DbContext.survey_configurations.AsNoTracking().ToList();
            List<survey_configuration_versions> surveyConfigurationVersions = DbContext.survey_configuration_versions.AsNoTracking().ToList();
            
            Assert.NotNull(surveyConfigurations);                                                             
            Assert.NotNull(surveyConfigurationVersions);                                                             

            Assert.AreEqual(1,surveyConfigurations.Count());  
            Assert.AreEqual(1,surveyConfigurationVersions.Count());  
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurations[0].CreatedAt.ToString());                                  
            Assert.AreEqual(surveyConfiguration.Version, surveyConfigurations[0].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurations[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurations[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurations[0].Id);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurations[0].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurations[0].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurations[0].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurations[0].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurations[0].TimeToLive);
            
            //Versions
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurationVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(1, surveyConfigurationVersions[0].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurationVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurationVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurationVersions[0].SurveyConfigurationId);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurationVersions[0].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurationVersions[0].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurationVersions[0].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurationVersions[0].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurationVersions[0].TimeToLive);
        }

        [Test]
        public void SurveyConfigurations_Update_DoesUpdate()
        {
            //Arrange
            Random rnd = new Random();
            
            survey_configurations surveyConfiguration = new survey_configurations();
            surveyConfiguration.Name = Guid.NewGuid().ToString();
            surveyConfiguration.Start = DateTime.Now;
            surveyConfiguration.Stop = DateTime.Now;
            surveyConfiguration.TimeOut = rnd.Next(1, 255);
            surveyConfiguration.TimeToLive = rnd.Next(1, 255);
            surveyConfiguration.Create(DbContext);

            
            //Act

            DateTime? oldUpdatedAt = surveyConfiguration.UpdatedAt;
            string oldName = surveyConfiguration.Name;
            DateTime? oldStart = surveyConfiguration.Start;
            DateTime? oldStop = surveyConfiguration.Stop;
            int? oldTimeOut = surveyConfiguration.TimeOut;
            int? oldTimeToLive = surveyConfiguration.TimeToLive;
            
            surveyConfiguration.Name = Guid.NewGuid().ToString();
            surveyConfiguration.Start = DateTime.Now;
            surveyConfiguration.Stop = DateTime.Now;
            surveyConfiguration.TimeOut = rnd.Next(1, 255);
            surveyConfiguration.TimeToLive = rnd.Next(1, 255);
            surveyConfiguration.Update(DbContext);
            
            List<survey_configurations> surveyConfigurations = DbContext.survey_configurations.AsNoTracking().ToList();
            List<survey_configuration_versions> surveyConfigurationVersions = DbContext.survey_configuration_versions.AsNoTracking().ToList();
            
            Assert.NotNull(surveyConfigurations);                                                             
            Assert.NotNull(surveyConfigurationVersions);                                                             

            Assert.AreEqual(1,surveyConfigurations.Count());  
            Assert.AreEqual(2,surveyConfigurationVersions.Count());  
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurations[0].CreatedAt.ToString());                                  
            Assert.AreEqual(surveyConfiguration.Version, surveyConfigurations[0].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurations[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurations[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurations[0].Id);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurations[0].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurations[0].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurations[0].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurations[0].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurations[0].TimeToLive);
            
            //Old Version
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurationVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(1, surveyConfigurationVersions[0].Version);                                      
            Assert.AreEqual(oldUpdatedAt.ToString(), surveyConfigurationVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurationVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurationVersions[0].SurveyConfigurationId);
            Assert.AreEqual(oldName, surveyConfigurationVersions[0].Name);
            Assert.AreEqual(oldStart.ToString(), surveyConfigurationVersions[0].Start.ToString());
            Assert.AreEqual(oldStop.ToString(), surveyConfigurationVersions[0].Stop.ToString());
            Assert.AreEqual(oldTimeOut, surveyConfigurationVersions[0].TimeOut);
            Assert.AreEqual(oldTimeToLive, surveyConfigurationVersions[0].TimeToLive);
            
            //New Version
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurationVersions[1].CreatedAt.ToString());                                  
            Assert.AreEqual(2, surveyConfigurationVersions[1].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurationVersions[1].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurationVersions[1].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurationVersions[1].SurveyConfigurationId);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurationVersions[1].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurationVersions[1].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurationVersions[1].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurationVersions[1].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurationVersions[1].TimeToLive);
        }

        [Test]
        public void SurveyConfigurations_Delete_DoesSetWorkflowStateToRemoved()
        {
            //Arrange
            Random rnd = new Random();
            
            survey_configurations surveyConfiguration = new survey_configurations();
            surveyConfiguration.Name = Guid.NewGuid().ToString();
            surveyConfiguration.Start = DateTime.Now;
            surveyConfiguration.Stop = DateTime.Now;
            surveyConfiguration.TimeOut = rnd.Next(1, 255);
            surveyConfiguration.TimeToLive = rnd.Next(1, 255);
            surveyConfiguration.Create(DbContext);

            
            //Act

            DateTime? oldUpdatedAt = surveyConfiguration.UpdatedAt;
         
            surveyConfiguration.Delete(DbContext);
            
            List<survey_configurations> surveyConfigurations = DbContext.survey_configurations.AsNoTracking().ToList();
            List<survey_configuration_versions> surveyConfigurationVersions = DbContext.survey_configuration_versions.AsNoTracking().ToList();
            
            Assert.NotNull(surveyConfigurations);                                                             
            Assert.NotNull(surveyConfigurationVersions);                                                             

            Assert.AreEqual(1,surveyConfigurations.Count());  
            Assert.AreEqual(2,surveyConfigurationVersions.Count());  
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurations[0].CreatedAt.ToString());                                  
            Assert.AreEqual(surveyConfiguration.Version, surveyConfigurations[0].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurations[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurations[0].WorkflowState, Constants.WorkflowStates.Removed);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurations[0].Id);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurations[0].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurations[0].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurations[0].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurations[0].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurations[0].TimeToLive);
            
            //Old Version
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurationVersions[0].CreatedAt.ToString());                                  
            Assert.AreEqual(1, surveyConfigurationVersions[0].Version);                                      
            Assert.AreEqual(oldUpdatedAt.ToString(), surveyConfigurationVersions[0].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurationVersions[0].WorkflowState, Constants.WorkflowStates.Created);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurationVersions[0].SurveyConfigurationId);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurationVersions[0].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurationVersions[0].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurationVersions[0].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurationVersions[0].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurationVersions[0].TimeToLive);
            
            //New Version
            
            Assert.AreEqual(surveyConfiguration.CreatedAt.ToString(), surveyConfigurationVersions[1].CreatedAt.ToString());                                  
            Assert.AreEqual(2, surveyConfigurationVersions[1].Version);                                      
            Assert.AreEqual(surveyConfiguration.UpdatedAt.ToString(), surveyConfigurationVersions[1].UpdatedAt.ToString());                                  
            Assert.AreEqual(surveyConfigurationVersions[1].WorkflowState, Constants.WorkflowStates.Removed);
            Assert.AreEqual(surveyConfiguration.Id, surveyConfigurationVersions[1].SurveyConfigurationId);
            Assert.AreEqual(surveyConfiguration.Name, surveyConfigurationVersions[1].Name);
            Assert.AreEqual(surveyConfiguration.Start.ToString(), surveyConfigurationVersions[1].Start.ToString());
            Assert.AreEqual(surveyConfiguration.Stop.ToString(), surveyConfigurationVersions[1].Stop.ToString());
            Assert.AreEqual(surveyConfiguration.TimeOut, surveyConfigurationVersions[1].TimeOut);
            Assert.AreEqual(surveyConfiguration.TimeToLive, surveyConfigurationVersions[1].TimeToLive);
        }
    }
}