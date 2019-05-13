﻿using eFormCore;
using eFormCore.Helpers;
using eFormData;
using eFormShared;
using eFormSqlController;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace eFormSDK.Integration.Tests
{
    [TestFixture]
    public class SqlControllerTestCase : DbTestFixture
    {
        private SqlController sut;
        private TestHelpers testHelpers;
        private string path;

        public override void DoSetup()
        {
            #region Setup SettingsTableContent

            SqlController sql = new SqlController(ConnectionString);
            sql.SettingUpdate(Settings.token, "abc1234567890abc1234567890abcdef");
            sql.SettingUpdate(Settings.firstRunDone, "true");
            sql.SettingUpdate(Settings.knownSitesDone, "true");
            #endregion

            sut = new SqlController(ConnectionString);
            sut.StartLog(new CoreBase());
            testHelpers = new TestHelpers();
            sut.SettingUpdate(Settings.fileLocationPicture, path + @"\output\dataFolder\picture\");
            sut.SettingUpdate(Settings.fileLocationPdf, path + @"\output\dataFolder\pdf\");
            sut.SettingUpdate(Settings.fileLocationJasper, path + @"\output\dataFolder\reports\");
        }

        [Test]
        public void SQL_Case_CaseCreate_DoesCaseCreate()
        {
            sites site1 = testHelpers.CreateSite("MySite", 22);

            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "template1", "template_desc", "", "", 1, 1);

            string guid = Guid.NewGuid().ToString();

            //Case aCase = CreateCase("caseUID", cl1, )
            DateTime c1_ca = DateTime.Now.AddDays(-9);
            DateTime c1_da = DateTime.Now.AddDays(-8).AddHours(-12);
            DateTime c1_ua = DateTime.Now.AddDays(-8);
            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site1, worker);
            units unit = testHelpers.CreateUnit(48, 49, site1, 348);

            string microtingUId = Guid.NewGuid().ToString();
            string microtingCheckId = Guid.NewGuid().ToString();
        

            // Act
            int matches = sut.CaseCreate(cl1.Id, (int)site1.MicrotingUid, microtingUId, microtingCheckId, "", "", c1_ca);
            List<check_list_sites> checkListSiteResult1 = DbContext.check_list_sites.AsNoTracking().ToList();
            var versionedMatches1 = DbContext.check_list_site_versions.AsNoTracking().ToList();

            // Assert

            Assert.NotNull(matches);
            // Assert.AreEqual(Constants.WorkflowStates.Created, versionedMatches1[1].workflow_state);
            // Assert.AreEqual(Constants.WorkflowStates.Created, versionedMatches1[0].workflow_state);

        }

        [Test]
        public void SQL_Case_CaseReadLastCheckIdByMicrotingUId_DoesCaseReadLastIDByMicrotiingUID()
        {
            sites site1 = testHelpers.CreateSite("mySite2", 331);
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "template2", "template_desc", "", "", 1, 1);

            string guid = Guid.NewGuid().ToString();
            string guid2 = Guid.NewGuid().ToString();
            string lastCheckUid1 = Guid.NewGuid().ToString();

            
            //check_list_sites cls1 = testHelpers.CreateCheckListSite(cl1.Id, site1.Id, guid, Constants.WorkflowStates.Created, lastCheckUid1);
            check_list_sites cls1 = testHelpers.CreateCheckListSite(cl1,cl1_Ca, site1, cl1_Ua, 1, Constants.WorkflowStates.Created, lastCheckUid1);

            //cases case1 = CreateCase

            // Act
            string matches = sut.CaseReadLastCheckIdByMicrotingUId(cls1.MicrotingUid);
            List<check_list_sites> checkListSiteResult1 = DbContext.check_list_sites.AsNoTracking().ToList();
            var versionedMatches1 = DbContext.check_list_site_versions.AsNoTracking().ToList();


            // Assert
            Assert.AreEqual(cls1.LastCheckId, matches);


        }

        [Test]
        public void SQL_Case_CaseUpdateRetrived_DoesCaseGetUpdated()
        {

            // Arrance
            sites site = new sites();
            site.Name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.Label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.MicrotingUid = "microting_uid";
            aCase.MicrotingCheckUid = "microting_check_uid";
            aCase.WorkflowState = Constants.WorkflowStates.Created;
            aCase.CheckListId = cl.Id;
            aCase.SiteId = site.Id;
            aCase.Status = 66;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseUpdateRetrived(aCase.MicrotingUid);
            //Case_Dto caseResult = sut.CaseFindCustomMatchs(aCase.microting_uid);
            List<cases> caseResults = DbContext.cases.AsNoTracking().ToList();


            // Assert


            Assert.NotNull(caseResults);
            Assert.AreEqual(1, caseResults.Count);
            Assert.AreNotEqual(1, caseResults[0]);




        }

        [Test]
        public void SQL_Case_CaseUpdateCompleted_DoesCaseGetUpdated()
        {
            sites site1 = testHelpers.CreateSite("MySite", 22);
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ca, "template1", "template_desc", "", "", 1, 1);

            string guid = Guid.NewGuid().ToString();
            string lastCheckUid1 = Guid.NewGuid().ToString();

            //Case aCase = CreateCase("caseUID", cl1, )
            DateTime c1_ca = DateTime.Now.AddDays(-9);
            DateTime c1_da = DateTime.Now.AddDays(-8).AddHours(-12);
            DateTime c1_ua = DateTime.Now.AddDays(-8);
            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site1, worker);
            units unit = testHelpers.CreateUnit(48, 49, site1, 348);



            string microtingUId = Guid.NewGuid().ToString();
            string microtingCheckId = Guid.NewGuid().ToString();
            cases aCase1 = testHelpers.CreateCase("case1UId", cl1, c1_ca, "custom1",
                c1_da, worker, "microtingCheckUId1", "microtingUId1",
              site1, 66, "caseType1", unit, c1_ua, 1, worker, Constants.WorkflowStates.Created);


            // Act
            //sut.CaseUpdateCompleted(aCase1.microting_uid, aCase1.microting_check_uid, c1_ua, aCase1.Id, aCase1.Id);
            List<cases> caseResults = DbContext.cases.AsNoTracking().Where(x => x.MicrotingUid == aCase1.MicrotingUid).ToList();
            Assert.NotNull(caseResults);
            Assert.AreEqual(1, caseResults.Count());
            Assert.AreEqual(Constants.WorkflowStates.Created, caseResults[0].WorkflowState);
            Assert.AreEqual(66, caseResults[0].Status);

            sut.CaseUpdateCompleted(aCase1.MicrotingUid, aCase1.MicrotingCheckUid, c1_da, aCase1.Worker.MicrotingUid, (int)unit.MicrotingUid);
            caseResults = DbContext.cases.AsNoTracking().Where(x => x.MicrotingUid == aCase1.MicrotingUid).ToList();
            var versionedMatches1 = DbContext.check_list_site_versions.AsNoTracking().Where(x => x.MicrotingUid == aCase1.MicrotingUid).ToList();
            //DbContext.cases

            // Assert


            Assert.NotNull(caseResults);
            Assert.AreEqual(1, caseResults.Count());
            Assert.AreEqual(Constants.WorkflowStates.Created, caseResults[0].WorkflowState);
            Assert.AreEqual(100, caseResults[0].Status);
            Assert.AreEqual(c1_da.ToString(), caseResults[0].DoneAt.ToString());

        }

        [Test]
        public void SQL_Case_CaseRetract_DoesCaseGetRetracted()
        {

            // Arrance
            // Arrance
            sites site = new sites();
            site.Name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.Label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.MicrotingUid = "microting_uid";
            aCase.MicrotingCheckUid = "microting_check_uid";
            aCase.WorkflowState = Constants.WorkflowStates.Created;
            aCase.CheckListId = cl.Id;
            aCase.SiteId = site.Id;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseRetract(aCase.MicrotingUid, aCase.MicrotingCheckUid);
            //cases theCase = sut.CaseReadFull(aCase.microting_uid, aCase.microting_check_uid);
            var match = DbContext.cases.AsNoTracking().ToList();
            var versionedMatches = DbContext.case_versions.AsNoTracking().ToList();

            // Assert
            Assert.NotNull(match);
            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(1, versionedMatches.Count);
            Assert.AreEqual(Constants.WorkflowStates.Retracted, match[0].WorkflowState);
            Assert.AreEqual(Constants.WorkflowStates.Retracted, versionedMatches[0].WorkflowState);


        }

        [Test]
        public void SQL_Case_CaseDelete_DoesCaseRemoved()
        {
            // Arrance
            // Arrance
            sites site = new sites();
            site.Name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.Label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.MicrotingUid = "microting_uid";
            aCase.MicrotingCheckUid = "microting_check_uid";
            aCase.WorkflowState = Constants.WorkflowStates.Created;
            aCase.CheckListId = cl.Id;
            aCase.SiteId = site.Id;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseDelete(aCase.MicrotingUid);
            //cases theCase = sut.CaseReadFull(aCase.microting_uid, aCase.microting_check_uid);
            var match = DbContext.cases.AsNoTracking().ToList();
            var versionedMatches = DbContext.case_versions.AsNoTracking().ToList();

            // Assert
            Assert.NotNull(match);
            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(1, versionedMatches.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, match[0].WorkflowState);
            Assert.AreEqual(Constants.WorkflowStates.Removed, versionedMatches[0].WorkflowState);
        }

        [Test]
        public void SQL_Case_CaseDeleteResult_DoesMarkCaseRemoved()
        {

            // Arrance
            sites site = new sites();
            site.Name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.Label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.MicrotingUid = "microting_uid";
            aCase.MicrotingCheckUid = "microting_check_uid";
            aCase.WorkflowState = Constants.WorkflowStates.Created;
            aCase.CheckListId = cl.Id;
            aCase.SiteId = site.Id;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseDeleteResult(aCase.Id);
            //cases theCase = sut.CaseReadFull(aCase.microting_uid, aCase.microting_check_uid);
            var match = DbContext.cases.AsNoTracking().ToList();
            var versionedMatches = DbContext.case_versions.AsNoTracking().ToList();

            // Assert
            Assert.NotNull(match);
            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(1, versionedMatches.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, match[0].WorkflowState);
            Assert.AreEqual(Constants.WorkflowStates.Removed, versionedMatches[0].WorkflowState);
        }

        [Test]
        public void SQL_PostCase_CaseReadFirstId()
        {
            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region Case1

            cases aCase = testHelpers.CreateCase("caseUId", cl1, DateTime.Now, "custom", DateTime.Now,
                worker, "microtingCheckUId", "microtingUId",
               site, 100, "caseType", unit, DateTime.Now, 1, worker, Constants.WorkflowStates.Created);

            #endregion


            #endregion
            // Act
            var match = sut.CaseReadFirstId(aCase.CheckListId, Constants.WorkflowStates.NotRemoved);
            // Assert
            Assert.AreEqual(aCase.Id, match);
        }

        [Test]
        public void SQL_PostCase_CaseFindCustomMatchs()
        {


            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region cases
            #region cases created
            #region Case1

            DateTime c1_ca = DateTime.Now.AddDays(-9);
            DateTime c1_da = DateTime.Now.AddDays(-8).AddHours(-12);
            DateTime c1_ua = DateTime.Now.AddDays(-8);

            cases aCase1 = testHelpers.CreateCase("case1UId", cl1, c1_ca, "custom1",
                c1_da, worker, "microtingCheckUId1", "microtingUId1",
               site, 1, "caseType1", unit, c1_ua, 1, worker, Constants.WorkflowStates.Created);

            #endregion

            #region Case2

            DateTime c2_ca = DateTime.Now.AddDays(-7);
            DateTime c2_da = DateTime.Now.AddDays(-6).AddHours(-12);
            DateTime c2_ua = DateTime.Now.AddDays(-6);
            cases aCase2 = testHelpers.CreateCase("case2UId", cl1, c2_ca, "custom2",
             c2_da, worker, "microtingCheck2UId", "microting2UId",
               site, 10, "caseType2", unit, c2_ua, 1, worker, Constants.WorkflowStates.Created);
            #endregion

            #region Case3
            DateTime c3_ca = DateTime.Now.AddDays(-10);
            DateTime c3_da = DateTime.Now.AddDays(-9).AddHours(-12);
            DateTime c3_ua = DateTime.Now.AddDays(-9);

            cases aCase3 = testHelpers.CreateCase("case3UId", cl1, c3_ca, "custom3",
              c3_da, worker, "microtingCheck3UId", "microtin3gUId",
               site, 15, "caseType3", unit, c3_ua, 1, worker, Constants.WorkflowStates.Created);
            #endregion

            #region Case4
            DateTime c4_ca = DateTime.Now.AddDays(-8);
            DateTime c4_da = DateTime.Now.AddDays(-7).AddHours(-12);
            DateTime c4_ua = DateTime.Now.AddDays(-7);

            cases aCase4 = testHelpers.CreateCase("case4UId", cl1, c4_ca, "custom4",
                c4_da, worker, "microtingCheck4UId", "microting4UId",
               site, 100, "caseType4", unit, c4_ua, 1, worker, Constants.WorkflowStates.Created);
            #endregion
            #endregion

            #endregion

            #endregion
            // Act
            List<Case_Dto> aCase1Custom = sut.CaseFindCustomMatchs(aCase1.Custom);
            List<Case_Dto> aCase2Custom = sut.CaseFindCustomMatchs(aCase2.Custom);
            List<Case_Dto> aCase3Custom = sut.CaseFindCustomMatchs(aCase3.Custom);
            List<Case_Dto> aCase4Custom = sut.CaseFindCustomMatchs(aCase4.Custom);
            // Assert
            Assert.AreEqual(aCase1.Custom, aCase1Custom[0].Custom);
            Assert.AreEqual(aCase2.Custom, aCase2Custom[0].Custom);
            Assert.AreEqual(aCase3.Custom, aCase3Custom[0].Custom);
            Assert.AreEqual(aCase4.Custom, aCase4Custom[0].Custom);




        }

        [Test]
        public void SQL_PostCase_CaseUpdateFieldValues()
        {


            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field6

            fields f6 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                86, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field7

            fields f7 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                87, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field8

            fields f8 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                88, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field9

            fields f9 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                89, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field10

            fields f10 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                90, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #endregion

            #region Worker

            workers worker1 = testHelpers.CreateWorker(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            workers worker2 = testHelpers.CreateWorker(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            workers worker3 = testHelpers.CreateWorker(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            workers worker4 = testHelpers.CreateWorker(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), testHelpers.GetRandomInt());

            #endregion

            #region site
            sites site1 = testHelpers.CreateSite(Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            sites site2 = testHelpers.CreateSite(Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            sites site3 = testHelpers.CreateSite(Guid.NewGuid().ToString(), testHelpers.GetRandomInt());
            sites site4 = testHelpers.CreateSite(Guid.NewGuid().ToString(), testHelpers.GetRandomInt());

            #endregion

            #region units
            units unit1 = testHelpers.CreateUnit(testHelpers.GetRandomInt(), testHelpers.GetRandomInt(), site1, 348);
            units unit2 = testHelpers.CreateUnit(testHelpers.GetRandomInt(), testHelpers.GetRandomInt(), site2, 348);
            units unit3 = testHelpers.CreateUnit(testHelpers.GetRandomInt(), testHelpers.GetRandomInt(), site3, 348);
            units unit4 = testHelpers.CreateUnit(testHelpers.GetRandomInt(), testHelpers.GetRandomInt(), site4, 348);

            #endregion

            #region site_workers
            site_workers site_workers1 = testHelpers.CreateSiteWorker(testHelpers.GetRandomInt(), site1, worker1);
            site_workers site_workers2 = testHelpers.CreateSiteWorker(testHelpers.GetRandomInt(), site2, worker2);
            site_workers site_workers3 = testHelpers.CreateSiteWorker(testHelpers.GetRandomInt(), site3, worker3);
            site_workers site_workers4 = testHelpers.CreateSiteWorker(testHelpers.GetRandomInt(), site4, worker4);

            #endregion

            #region cases
            #region cases created
            #region Case1

            DateTime c1_ca = DateTime.Now.AddDays(-9);
            DateTime c1_da = DateTime.Now.AddDays(-8).AddHours(-12);
            DateTime c1_ua = DateTime.Now.AddDays(-8);

            cases aCase1 = testHelpers.CreateCase("case1UId", cl2, c1_ca, "custom1",
                c1_da, worker1, "microtingCheckUId1", "microtingUId1",
               site1, 1, "caseType1", unit1, c1_ua, 1, worker1, Constants.WorkflowStates.Created);

            #endregion

            #region Case2

            DateTime c2_ca = DateTime.Now.AddDays(-7);
            DateTime c2_da = DateTime.Now.AddDays(-6).AddHours(-12);
            DateTime c2_ua = DateTime.Now.AddDays(-6);
            cases aCase2 = testHelpers.CreateCase("case2UId", cl1, c2_ca, "custom2",
             c2_da, worker2, "microtingCheck2UId", "microting2UId",
               site2, 10, "caseType2", unit2, c2_ua, 1, worker2, Constants.WorkflowStates.Created);
            #endregion

            #region Case3
            DateTime c3_ca = DateTime.Now.AddDays(-10);
            DateTime c3_da = DateTime.Now.AddDays(-9).AddHours(-12);
            DateTime c3_ua = DateTime.Now.AddDays(-9);

            cases aCase3 = testHelpers.CreateCase("case3UId", cl1, c3_ca, "custom3",
              c3_da, worker3, "microtingCheck3UId", "microtin3gUId",
               site3, 15, "caseType3", unit3, c3_ua, 1, worker3, Constants.WorkflowStates.Created);
            #endregion

            #region Case4
            DateTime c4_ca = DateTime.Now.AddDays(-8);
            DateTime c4_da = DateTime.Now.AddDays(-7).AddHours(-12);
            DateTime c4_ua = DateTime.Now.AddDays(-7);

            cases aCase4 = testHelpers.CreateCase("case4UId", cl1, c4_ca, "custom4",
                c4_da, worker4, "microtingCheck4UId", "microting4UId",
               site4, 100, "caseType4", unit4, c4_ua, 1, worker4, Constants.WorkflowStates.Created);
            #endregion
            #endregion



            #endregion

            #region UploadedData
            #region ud1
            uploaded_data ud1 = testHelpers.CreateUploadedData("checksum1", "File1", "no", "hjgjghjhg", "File1", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud2
            uploaded_data ud2 = testHelpers.CreateUploadedData("checksum2", "File1", "no", "hjgjghjhg", "File2", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud3
            uploaded_data ud3 = testHelpers.CreateUploadedData("checksum3", "File1", "no", "hjgjghjhg", "File3", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud4
            uploaded_data ud4 = testHelpers.CreateUploadedData("checksum4", "File1", "no", "hjgjghjhg", "File4", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud5
            uploaded_data ud5 = testHelpers.CreateUploadedData("checksum5", "File1", "no", "hjgjghjhg", "File5", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud6
            uploaded_data ud6 = testHelpers.CreateUploadedData("checksum6", "File1", "no", "hjgjghjhg", "File6", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud7
            uploaded_data ud7 = testHelpers.CreateUploadedData("checksum7", "File1", "no", "hjgjghjhg", "File7", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud8
            uploaded_data ud8 = testHelpers.CreateUploadedData("checksum8", "File1", "no", "hjgjghjhg", "File8", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud9
            uploaded_data ud9 = testHelpers.CreateUploadedData("checksum9", "File1", "no", "hjgjghjhg", "File9", 1, worker1,
                "local", 55, false);
            #endregion

            #region ud10
            uploaded_data ud10 = testHelpers.CreateUploadedData("checksum10", "File1", "no", "hjgjghjhg", "File10", 1, worker1,
                "local", 55, false);
            #endregion

            #endregion

            #region Field Values
            #region fv1
            field_values field_Value1 = testHelpers.CreateFieldValue(aCase1, cl2, f1, ud1.Id, null, "tomt1", 61234, worker1);

            #endregion

            #region fv2
            field_values field_Value2 = testHelpers.CreateFieldValue(aCase1, cl2, f2, ud2.Id, null, "tomt2", 61234, worker1);

            #endregion

            #region fv3
            field_values field_Value3 = testHelpers.CreateFieldValue(aCase1, cl2, f3, ud3.Id, null, "tomt3", 61234, worker1);

            #endregion

            #region fv4
            field_values field_Value4 = testHelpers.CreateFieldValue(aCase1, cl2, f4, ud4.Id, null, "tomt4", 61234, worker1);

            #endregion

            #region fv5
            field_values field_Value5 = testHelpers.CreateFieldValue(aCase1, cl2, f5, ud5.Id, null, "tomt5", 61234, worker1);

            #endregion

            #region fv6
            field_values field_Value6 = testHelpers.CreateFieldValue(aCase1, cl2, f6, ud6.Id, null, "tomt6", 61234, worker1);

            #endregion

            #region fv7
            field_values field_Value7 = testHelpers.CreateFieldValue(aCase1, cl2, f7, ud7.Id, null, "tomt7", 61234, worker1);

            #endregion

            #region fv8
            field_values field_Value8 = testHelpers.CreateFieldValue(aCase1, cl2, f8, ud8.Id, null, "tomt8", 61234, worker1);

            #endregion

            #region fv9
            field_values field_Value9 = testHelpers.CreateFieldValue(aCase1, cl2, f9, ud9.Id, null, "tomt9", 61234, worker1);

            #endregion

            #region fv10
            field_values field_Value10 = testHelpers.CreateFieldValue(aCase1, cl2, f10, ud10.Id, null, "tomt10", 61234, worker1);

            #endregion


            #endregion
            #endregion
            // Act
            cases theCase = DbContext.cases.First();
            Assert.NotNull(theCase);
            check_lists theCheckList = DbContext.check_lists.First();

            theCheckList.Field1 = f1.Id;
            theCheckList.Field2 = f2.Id;
            theCheckList.Field3 = f3.Id;
            theCheckList.Field4 = f4.Id;
            theCheckList.Field5 = f5.Id;
            theCheckList.Field6 = f6.Id;
            theCheckList.Field7 = f7.Id;
            theCheckList.Field8 = f8.Id;
            theCheckList.Field9 = f9.Id;
            theCheckList.Field10 = f10.Id;

            Assert.AreEqual(null, theCase.FieldValue1);
            Assert.AreEqual(null, theCase.FieldValue2);
            Assert.AreEqual(null, theCase.FieldValue3);
            Assert.AreEqual(null, theCase.FieldValue4);
            Assert.AreEqual(null, theCase.FieldValue5);
            Assert.AreEqual(null, theCase.FieldValue6);
            Assert.AreEqual(null, theCase.FieldValue7);
            Assert.AreEqual(null, theCase.FieldValue8);
            Assert.AreEqual(null, theCase.FieldValue9);
            Assert.AreEqual(null, theCase.FieldValue10);

            var testThis = sut.CaseUpdateFieldValues(aCase1.Id);

            // Assert
            cases theCaseAfter = DbContext.cases.AsNoTracking().First();

            Assert.NotNull(theCaseAfter);

            theCaseAfter.FieldValue1 = field_Value1.value;
            theCaseAfter.FieldValue2 = field_Value2.value;
            theCaseAfter.FieldValue3 = field_Value3.value;
            theCaseAfter.FieldValue4 = field_Value4.value;
            theCaseAfter.FieldValue5 = field_Value5.value;
            theCaseAfter.FieldValue6 = field_Value6.value;
            theCaseAfter.FieldValue7 = field_Value7.value;
            theCaseAfter.FieldValue8 = field_Value8.value;
            theCaseAfter.FieldValue9 = field_Value9.value;
            theCaseAfter.FieldValue10 = field_Value10.value;


            Assert.True(testThis);

            Assert.AreEqual("tomt1", theCaseAfter.FieldValue1);
            Assert.AreEqual("tomt2", theCaseAfter.FieldValue2);
            Assert.AreEqual("tomt3", theCaseAfter.FieldValue3);
            Assert.AreEqual("tomt4", theCaseAfter.FieldValue4);
            Assert.AreEqual("tomt5", theCaseAfter.FieldValue5);
            Assert.AreEqual("tomt6", theCaseAfter.FieldValue6);
            Assert.AreEqual("tomt7", theCaseAfter.FieldValue7);
            Assert.AreEqual("tomt8", theCaseAfter.FieldValue8);
            Assert.AreEqual("tomt9", theCaseAfter.FieldValue9);
            Assert.AreEqual("tomt10", theCaseAfter.FieldValue10);

        }

        [Test]
        public void SQL_PostCase_CaseReadByMUId_Returns_ReturnCase()
        {
            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region Case1

            cases aCase = testHelpers.CreateCase("caseUId", cl1, DateTime.Now, "custom", DateTime.Now,
                worker, "microtingCheckUId", "microtingUId",
               site, 66, "caseType", unit, DateTime.Now, 1, worker, Constants.WorkflowStates.Created);

            #endregion

            #region UploadedData
            #region ud1
            uploaded_data ud1 = testHelpers.CreateUploadedData("checksum", "File1", "no", "hjgjghjhg", "File1", 1, worker,
                "local", 55, false);
            #endregion

            #region ud2
            uploaded_data ud2 = testHelpers.CreateUploadedData("checksum", "File1", "no", "hjgjghjhg", "File2", 1, worker,
                "local", 55, false);
            #endregion

            #region ud3
            uploaded_data ud3 = testHelpers.CreateUploadedData("checksum", "File1", "no", "hjgjghjhg", "File3", 1, worker,
                "local", 55, false);
            #endregion

            #region ud4
            uploaded_data ud4 = testHelpers.CreateUploadedData("checksum", "File1", "no", "hjgjghjhg", "File4", 1, worker,
                "local", 55, false);
            #endregion

            #region ud5
            uploaded_data ud5 = testHelpers.CreateUploadedData("checksum", "File1", "no", "hjgjghjhg", "File5", 1, worker,
                "local", 55, false);
            #endregion

            #endregion

            #region Check List Values
            check_list_values check_List_Values = testHelpers.CreateCheckListValue(aCase, cl2, "checked", null, 865);


            #endregion

            #region Field Values
            #region fv1
            field_values field_Value1 = testHelpers.CreateFieldValue(aCase, cl2, f1, ud1.Id, null, "tomt1", 61234, worker);

            #endregion

            #region fv2
            field_values field_Value2 = testHelpers.CreateFieldValue(aCase, cl2, f2, ud2.Id, null, "tomt2", 61234, worker);

            #endregion

            #region fv3
            field_values field_Value3 = testHelpers.CreateFieldValue(aCase, cl2, f3, ud3.Id, null, "tomt3", 61234, worker);

            #endregion

            #region fv4
            field_values field_Value4 = testHelpers.CreateFieldValue(aCase, cl2, f4, ud4.Id, null, "tomt4", 61234, worker);

            #endregion

            #region fv5
            field_values field_Value5 = testHelpers.CreateFieldValue(aCase, cl2, f5, ud5.Id, null, "tomt5", 61234, worker);

            #endregion


            #endregion
            #endregion


            // Act

            var match = sut.CaseReadByMUId(aCase.MicrotingUid);

            // Assert

            Assert.AreEqual(aCase.MicrotingUid, match.MicrotingUId);


        }

        [Test]
        public void SQL_PostCase_CaseReadByCaseId_Returns_cDto()
        {
            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region Case1

            cases aCase = testHelpers.CreateCase("caseUId", cl1, DateTime.Now, "custom", DateTime.Now,
                worker, "microtingCheckUId", "microtingUId",
               site, 66, "caseType", unit, DateTime.Now, 1, worker, Constants.WorkflowStates.Created);

            #endregion


            #endregion


            // Act

            var match = sut.CaseReadByCaseId(aCase.Id);

            // Assert

            Assert.AreEqual(aCase.Id, match.CaseId);
        }

        [Test]
        public void SQL_Postcase_CaseReadByCaseUId_Returns_lstDto()
        {
            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region Case1

            cases aCase = testHelpers.CreateCase("caseUId", cl1, DateTime.Now, "custom", DateTime.Now,
                worker, "microtingCheckUId", "microtingUId",
               site, 66, "caseType", unit, DateTime.Now, 1, worker, Constants.WorkflowStates.Created);

            #endregion


            #endregion


            // Act

            var match = sut.CaseReadByCaseUId(aCase.CaseUid);


            // Assert

            Assert.AreEqual(aCase.CaseUid, match[0].CaseUId);
        }

        [Test]
        public void SQL_PostCase_CaseReadFull()
        {
            // Arrance
            #region Arrance
            #region Template1
            DateTime cl1_Ca = DateTime.Now;
            DateTime cl1_Ua = DateTime.Now;
            check_lists cl1 = testHelpers.CreateTemplate(cl1_Ca, cl1_Ua, "A", "D", "CheckList", "Template1FolderName", 1, 1);

            #endregion

            #region SubTemplate1
            check_lists cl2 = testHelpers.CreateSubTemplate("A.1", "D.1", "CheckList", 1, 1, cl1);


            #endregion

            #region Fields
            #region field1


            fields f1 = testHelpers.CreateField(1, "barcode", cl2, "e2f4fb", "custom", null, "", "Comment field description",
                5, 1, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0, "Comment field", 1, 55, "55", "0", 0, 0, null, 1, 0,
                0, 0, "", 49);

            #endregion

            #region field2


            fields f2 = testHelpers.CreateField(1, "barcode", cl2, "f5eafa", "custom", null, "", "showPDf Description",
                45, 1, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 0, 1, 0, 0,
                "ShowPdf", 0, 5, "5", "0", 0, 0, null, 0, 0, 0, 0, "", 9);


            #endregion

            #region field3

            fields f3 = testHelpers.CreateField(0, "barcode", cl2, "f0f8db", "custom", 3, "", "Number Field Description",
                83, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Numberfield", 1, 8, "4865", "0", 0, 1, null, 1, 0, 0, 0, "", 1);


            #endregion

            #region field4


            fields f4 = testHelpers.CreateField(1, "barcode", cl2, "fff6df", "custom", null, "", "date Description",
                84, 0, DbContext.field_types.Where(x => x.FieldType == "picture").First(), 0, 0, 1, 0,
                "Date", 1, 666, "41153", "0", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion

            #region field5

            fields f5 = testHelpers.CreateField(0, "barcode", cl2, "ffe4e4", "custom", null, "", "picture Description",
                85, 0, DbContext.field_types.Where(x => x.FieldType == "comment").First(), 1, 0, 1, 0,
                "Picture", 1, 69, "69", "1", 0, 1, null, 0, 1, 0, 0, "", 1);


            #endregion
            #endregion

            #region Worker

            workers worker = testHelpers.CreateWorker("aa@tak.dk", "Arne", "Jensen", 21);

            #endregion

            #region site
            sites site = testHelpers.CreateSite("SiteName", 88);

            #endregion

            #region units
            units unit = testHelpers.CreateUnit(48, 49, site, 348);

            #endregion

            #region site_workers
            site_workers site_workers = testHelpers.CreateSiteWorker(55, site, worker);

            #endregion

            #region Case1

            cases aCase = testHelpers.CreateCase("caseUId", cl1, DateTime.Now, "custom", DateTime.Now,
                worker, "microtingCheckUId", "microtingUId",
               site, 66, "caseType", unit, DateTime.Now, 1, worker, Constants.WorkflowStates.Created);

            #endregion


            #endregion
            // Act
            var match = sut.CaseReadFull(aCase.MicrotingUid, aCase.MicrotingCheckUid);
            // Assert
            Assert.AreEqual(aCase.MicrotingUid, match.MicrotingUid);
            Assert.AreEqual(aCase.MicrotingCheckUid, match.MicrotingCheckUid);
        }

        #region eventhandlers
        public void EventCaseCreated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseRetrived(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseCompleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseDeleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventFileDownloaded(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventSiteActivated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }
        #endregion
    }

}