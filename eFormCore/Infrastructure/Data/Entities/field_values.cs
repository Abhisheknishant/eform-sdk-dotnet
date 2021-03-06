/*
The MIT License (MIT)

Copyright (c) 2007 - 2020 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Microting.eForm.Infrastructure.Data.Entities
{
    public partial class field_values : BaseEntity
    {
        public DateTime? DoneAt { get; set; }

        public DateTime? Date { get; set; }

        [ForeignKey("worker")]
        public int? WorkerId { get; set; }

        public int? CaseId { get; set; }

        [ForeignKey("field")]
        public int? FieldId { get; set; }

        [ForeignKey("check_list")]
        public int? CheckListId { get; set; }

        public int? CheckListDuplicateId { get; set; }

        [ForeignKey("uploaded_data")]
        public int? UploadedDataId { get; set; }

        public string Value { get; set; }

        [StringLength(255)]
        public string Latitude { get; set; }

        [StringLength(255)]
        public string Longitude { get; set; }

        [StringLength(255)]
        public string Altitude { get; set; }

        [StringLength(255)]
        public string Heading { get; set; }

        [StringLength(255)]
        public string Accuracy { get; set; }

        public virtual workers Worker { get; set; }

        public virtual fields Field { get; set; }

        public virtual check_lists CheckList { get; set; }

        public virtual uploaded_data UploadedData { get; set; }

        public async Task Create(MicrotingDbContext dbContext)
        {
            WorkflowState = Constants.Constants.WorkflowStates.Created;
            Version = 1;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            dbContext.field_values.Add(this);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            dbContext.field_value_versions.Add(MapFieldValueVersions(this));
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Update(MicrotingDbContext dbContext)
        {
            field_values fieldValues = await dbContext.field_values.FirstOrDefaultAsync(x => x.Id == Id);

            if (fieldValues == null)
            {
                throw new NullReferenceException($"Could not find Field Value with Id: {Id}");
            }

            fieldValues.DoneAt = DoneAt;
            fieldValues.Date = Date;
            fieldValues.WorkerId = WorkerId;
            fieldValues.CaseId = CaseId;
            fieldValues.FieldId = FieldId;
            fieldValues.CheckListId = CheckListId;
            fieldValues.CheckListDuplicateId = CheckListDuplicateId;
            fieldValues.UploadedDataId = UploadedDataId;
            fieldValues.Value = Value;
            fieldValues.Latitude = Latitude;
            fieldValues.Longitude = Longitude;
            fieldValues.Altitude = Altitude;
            fieldValues.Heading = Heading;
            fieldValues.Accuracy = Accuracy;

            if (dbContext.ChangeTracker.HasChanges())
            {
                fieldValues.UpdatedAt = DateTime.UtcNow;
                fieldValues.Version += 1;

                dbContext.field_value_versions.Add(MapFieldValueVersions(fieldValues));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(MicrotingDbContext dbContext)
        {
            field_values fieldValues = await dbContext.field_values.FirstOrDefaultAsync(x => x.Id == Id);

            if (fieldValues == null)
            {
                throw new NullReferenceException($"Could not find Field Value with Id: {Id}");
            }

            fieldValues.WorkflowState = Constants.Constants.WorkflowStates.Removed;
            
            if (dbContext.ChangeTracker.HasChanges())
            {
                fieldValues.UpdatedAt = DateTime.UtcNow;
                fieldValues.Version += 1;

                dbContext.field_value_versions.Add(MapFieldValueVersions(fieldValues));
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        
        
        private field_value_versions MapFieldValueVersions(field_values fieldValue)
        {
            return new field_value_versions
            {
                CreatedAt = fieldValue.CreatedAt,
                UpdatedAt = fieldValue.UpdatedAt,
                Value = fieldValue.Value,
                Latitude = fieldValue.Latitude,
                Longitude = fieldValue.Longitude,
                Altitude = fieldValue.Altitude,
                Heading = fieldValue.Heading,
                Date = fieldValue.Date,
                Accuracy = fieldValue.Accuracy,
                UploadedDataId = fieldValue.UploadedDataId,
                Version = fieldValue.Version,
                CaseId = fieldValue.CaseId,
                FieldId = fieldValue.FieldId,
                WorkerId = fieldValue.WorkerId,
                WorkflowState = fieldValue.WorkflowState,
                CheckListId = fieldValue.CheckListId,
                CheckListDuplicateId = fieldValue.CheckListDuplicateId,
                DoneAt = fieldValue.DoneAt,
                FieldValueId = fieldValue.Id
            };
        }
    }
}
