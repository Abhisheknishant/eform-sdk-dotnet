using System.Collections.Generic;
using System.Threading.Tasks;
using Microting.eForm.Infrastructure;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eForm.Infrastructure.Models;

namespace Microting.eForm.Helpers
{
    public static class ValidationHelper
    {
        
        public static async Task<List<string>> FieldValidation(MicrotingDbContext dbContext, MainElement mainElement)
        {
            string methodName = "Core.FieldValidation";

            //await log.LogStandard(methodName, "called");

            List<string> errorLst = new List<string>();
            var dataItems = mainElement.DataItemGetAll();

            foreach (var dataItem in dataItems)
            {
                #region entities

                if (dataItem.GetType() == typeof(EntitySearch))
                {
                    EntitySearch entitySearch = (EntitySearch) dataItem;
                    var temp = await entity_groups.ReadSorted(dbContext, entitySearch.EntityTypeId.ToString(), "Id", "");// _sqlController.EntityGroupRead(entitySearch.EntityTypeId.ToString());
                    if (temp == null)
                        errorLst.Add("Element entitySearch.EntityTypeId:'" + entitySearch.EntityTypeId +
                                     "' is an reference to a local unknown EntitySearch group. Please update reference");
                }

                if (dataItem.GetType() == typeof(EntitySelect))
                {
                    EntitySelect entitySelect = (EntitySelect) dataItem;
                    var temp = await entity_groups.ReadSorted(dbContext, entitySelect.Source.ToString(), "Id", "");//_sqlController.EntityGroupRead(entitySelect.Source.ToString());
                    if (temp == null)
                        errorLst.Add("Element entitySelect.Source:'" + entitySelect.Source +
                                     "' is an reference to a local unknown EntitySearch group. Please update reference");
                }

                #endregion

                #region PDF

                if (dataItem.GetType() == typeof(ShowPdf))
                {
                    ShowPdf showPdf = (ShowPdf) dataItem;
                    errorLst.AddRange(await PdfValidate(showPdf.Value, showPdf.Id));
                }
                
                List<string> acceptedColors = new List<string>();
                acceptedColors.Add(Constants.FieldColors.Grey);
                acceptedColors.Add(Constants.FieldColors.Red);
                acceptedColors.Add(Constants.FieldColors.Green);
                acceptedColors.Add(Constants.FieldColors.Blue);
                acceptedColors.Add(Constants.FieldColors.Purple);
                acceptedColors.Add(Constants.FieldColors.Yellow);
                acceptedColors.Add(Constants.FieldColors.Default);
                acceptedColors.Add(Constants.FieldColors.None);
                if (!acceptedColors.Contains(dataItem.Color) && !string.IsNullOrEmpty(dataItem.Color))
                {
                    errorLst.Add($"DataItem with label {dataItem.Label} did supply color {dataItem.Color}, but the only allowed values are: e8eaf6 for grey, ffe4e4 for red, f0f8db for green, e2f4fb for blue, e2f4fb for purple, fff6df for yellow, None for default or leave it blank.");
                }

                #endregion
            }

            return errorLst;

        }
        
        private static async Task<List<string>> PdfValidate(string pdfString, int pdfId)
        {
            await Task.Run(() => { }); // TODO FIX ME
            List<string> errorLst = new List<string>();

            if (pdfString.ToLower().Contains("microting.com"))
                errorLst.Add("Element showPdf.Id:'" + pdfId + "' contains an URL that points to Microting's builder temporary hosting. Indicating that it's not a proper hash");
            if (pdfString.ToLower().Contains("http") || pdfString.ToLower().Contains("https"))
                errorLst.Add("Element showPdf.Id:'" + pdfId + "' contains an HTTP or HTTPS. Indicating that it's not a proper hash");
            if (pdfString.Length != 32)
                errorLst.Add("Element showPdf.Id:'" + pdfId + "' length is not the correct length (32). Indicating that it's not a proper hash");

            if (errorLst.Count > 0)
                errorLst.Add("Element showPdf.Id:'" + pdfId + "' please check 'value' input, and consider running PdfUpload");

            return errorLst;
        }
        
        public static async Task<List<string>> CheckListValidation(MainElement mainElement)
        {
            string methodName = "Core.CheckListValidation";
            //await log.LogStandard(methodName, "called");
            List<string> errorLst = new List<string>();
            
            List<string> acceptedColors = new List<string>();
            acceptedColors.Add(Constants.CheckListColors.Grey);
            acceptedColors.Add(Constants.CheckListColors.Red);
            acceptedColors.Add(Constants.CheckListColors.Green);

            if (!acceptedColors.Contains(mainElement.Color) && !string.IsNullOrEmpty(mainElement.Color))
            {
                errorLst.Add($"mainElement with label {mainElement.Label} did supply color {mainElement.Color}, but the only allowed colors are: grey, red, green or leave it blank.");
            }
            
            return errorLst;
        }
    }
}