﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 microting

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

using eForm.Messages;
using eFormSqlController;
using Rebus.Handlers;
using System;
using System.Threading.Tasks;
using eFormShared;
using eFormCommunicator;
using Newtonsoft.Json.Linq;

namespace eFormCore.Handlers
{
    public class TranscriptionCompletedHandler : IHandleMessages<TranscriptionCompleted>
    {
        private readonly SqlController sqlController;
        private readonly Communicator communicator;
        private readonly Log log;
        private readonly Core core;
        Tools t = new Tools();

        public TranscriptionCompletedHandler(SqlController sqlController, Communicator communicator, Log log, Core core)
        {
            this.sqlController = sqlController;
            this.communicator = communicator;
            this.log = log;
            this.core = core;
        }

#pragma warning disable 1998
        public async Task Handle(TranscriptionCompleted message)
        {
            try
            {
                field_values fv = sqlController.GetFieldValueByTranscriptionId(int.Parse(message.MicrotringUUID));
                JToken result = communicator.SpeechToText(int.Parse(message.MicrotringUUID));

                sqlController.FieldValueUpdate((int)fv.CaseId, (int)fv.Id, result["text"].ToString());

                #region download file
                uploaded_data ud = sqlController.GetUploaded_DataByTranscriptionId(int.Parse(message.MicrotringUUID));

                if (ud.FileName.Contains("3gp"))
                {
                    log.LogStandard(t.GetMethodName("TranscriptionCompletedHandler"), "file_name contains 3gp");
                    string urlStr = sqlController.SettingRead(Settings.comSpeechToText) + "/download_file/" + message.MicrotringUUID + ".wav?token=" + sqlController.SettingRead(Settings.token);
                    string fileLocationPicture = sqlController.SettingRead(Settings.fileLocationPicture);
                    using (var client = new System.Net.WebClient())
                    {
                        try
                        {
                            log.LogStandard(t.GetMethodName("TranscriptionCompletedHandler"), "Trying to donwload file from : " + urlStr);
                            client.DownloadFile(urlStr, fileLocationPicture + ud.FileName.Replace(".3gp", ".wav"));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Downloading and creating fil locally failed.", ex);
                        }
                    }
                }
                #endregion

                sqlController.NotificationUpdate(message.notificationUId, message.MicrotringUUID, Constants.WorkflowStates.Processed, "", "");

                log.LogStandard(t.GetMethodName("TranscriptionCompletedHandler"), "Transcription with id " + message.MicrotringUUID + " has been transcribed");
            }
            catch (Exception ex)
            {
                sqlController.NotificationUpdate(message.notificationUId, message.MicrotringUUID, Constants.WorkflowStates.NotFound, ex.Message, ex.StackTrace.ToString());
            }
        }
    }
}
