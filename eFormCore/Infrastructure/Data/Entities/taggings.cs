﻿/*
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Microting.eForm.Infrastructure.Data.Entities
{
    public partial class taggings : BaseEntity
    {
        [ForeignKey("tag")]
        public int? TagId { get; set; }

        [ForeignKey("check_list")]
        public int? CheckListId { get; set; }

        public int? TaggerId { get; set; } // this will refer to some user Id.

        public virtual tags Tag { get; set; }

        public virtual check_lists CheckList { get; set; }

        public async Task Create(MicrotingDbContext dbContext)
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Version = 1;
            WorkflowState = Constants.Constants.WorkflowStates.Created;
            
            dbContext.taggings.Add(this);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            dbContext.tagging_versions.Add(MapTaggingVersions(this));
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Update(MicrotingDbContext dbContext)
        {
            taggings tagging = await dbContext.taggings.FirstOrDefaultAsync(x => x.Id == Id);

            if (tagging == null)
            {
                throw new NullReferenceException($"Could not find tagging with Id: {Id}");
            }

            tagging.WorkflowState = WorkflowState;
            tagging.UpdatedAt = DateTime.UtcNow;
            tagging.TaggerId = TaggerId;
            tagging.Version += 1;

            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            dbContext.tagging_versions.Add(MapTaggingVersions(tagging));
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Delete(MicrotingDbContext dbContext)
        {
            taggings tagging = await dbContext.taggings.FirstOrDefaultAsync(x => x.Id == Id);

            if (tagging == null)
            {
                throw new NullReferenceException($"Could not find tagging with Id: {Id}");
            }

            tagging.WorkflowState = Constants.Constants.WorkflowStates.Removed;
            tagging.UpdatedAt = DateTime.UtcNow;
            tagging.Version += 1;

            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            dbContext.tagging_versions.Add(MapTaggingVersions(tagging));
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        
        private tagging_versions MapTaggingVersions(taggings tagging)
        {
            return new tagging_versions
            {
                WorkflowState = tagging.WorkflowState,
                Version = tagging.Version,
                CreatedAt = tagging.CreatedAt,
                UpdatedAt = tagging.UpdatedAt,
                CheckListId = tagging.CheckListId,
                TagId = tagging.TagId,
                TaggerId = tagging.TaggerId,
                TaggingId = tagging.Id
            };
        }
    }
}
