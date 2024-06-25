using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SAPPromotion
    {
    public class SAPPromotionJsonData
    {
        readonly string containerName = Properties.Settings.Default.ContainerName;
        readonly string blobDirectoryPrefix = Properties.Settings.Default.BlobDirectoryPrefix;
        readonly string destblobDirectoryPrefix = Properties.Settings.Default.DestDirectory;
        static IConfiguration _configuration;
        readonly SAPPromotionData promotionData;
        public SAPPromotionJsonData(IConfiguration configuration)
            {
            _configuration = configuration;
            promotionData = new SAPPromotionData(_configuration);
            }

        public void LoadPromotionData()
        {
            try
            {

                List<SAPBlobEntity> blobList = new List<SAPBlobEntity>();
                var storageKey = _configuration["StorageKey"];

                var storageAccount = CloudStorageAccount.Parse(storageKey);
                var myClient = storageAccount.CreateCloudBlobClient();
                var container = myClient.GetContainerReference(containerName);

                var list = container.ListBlobs().OfType<CloudBlobDirectory>().ToList();
                var blobListDirectory = list[0].ListBlobs().OfType<CloudBlobDirectory>().ToList();

                foreach (var blobDirectory in blobListDirectory)
                {
                    if (blobDirectory.Prefix == blobDirectoryPrefix)
                    {
                        foreach (var blobFile in blobDirectory.ListBlobs().OfType<CloudBlockBlob>())
                        {
                            SAPBlobEntity blobDetails = new SAPBlobEntity();
                            string[] blobName = blobFile.Name.Split(new char[] { '/' });
                            string[] filename = blobName[2].Split(new char[] { '.' });
                            string[] fileDateTime = filename[0].Split(new char[] { '_' });
                            string fileCreatedDateTime = fileDateTime[1] + fileDateTime[2];
                            string formatString = "yyyyMMddHHmmss";
                            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFile.Name);
                            blobDetails.Blob = blockBlob;
                            blobDetails.FileName = blobName[2];
                            blobDetails.FileCreatedDate = DateTime.ParseExact(fileCreatedDateTime, formatString, null);
                            blobDetails.FileData = blockBlob.DownloadTextAsync().Result;
                            blobDetails.BlobName = blobFile.Name;

                            blobList.Add(blobDetails);
                        }

                        blobList.OrderByDescending(x => x.FileCreatedDate.Date).ThenByDescending(x => x.FileCreatedDate.TimeOfDay).ToList();
                    }
                }

                foreach (var blobDetails in blobList)
                {
                    CheckRequiredFields(blobDetails, container);
                }
            }
            catch (StorageException ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ErrorMessage = ex.Message;
                promotionData.SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                }
        }

        public void CheckRequiredFields(SAPBlobEntity blobDetails, CloudBlobContainer container)
        {
            try
            {
                List<string> errors = new List<string>();
                if (string.IsNullOrEmpty(blobDetails.FileData))
                {
                    blobDetails.Status = "Error";
                    var errorLog = new SAPErrorLogEntity();
                    errorLog.PipeLineName = "Promotion";
                    errorLog.FileName = blobDetails.FileName;
                    errorLog.ErrorMessage = "File is empty";
                    promotionData.SaveErrorLogData(errorLog);
                    Logger logger = new Logger(_configuration);
                    logger.ErrorLogData(null,"File is empty");
                    }
                else
                {
                    SAPPromotion promotionJsonEntities = JsonConvert.DeserializeObject<SAPPromotion>(blobDetails.FileData, new JsonSerializerSettings
                    {
                        Error = delegate (object sender, ErrorEventArgs args)
                        {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;

                        },
                        Converters = { new IsoDateTimeConverter()
                    }
                    });

                    Dictionary<string, int> returnData = new Dictionary<string, int>();
                    if (promotionJsonEntities == null)
                    {
                        blobDetails.Status = "Error";
                        var errorLog = new SAPErrorLogEntity();
                        errorLog.PipeLineName = "Promotion";
                        errorLog.FileName = blobDetails.FileName;
                        errorLog.ErrorMessage = errors[0];
                        promotionData.SaveErrorLogData(errorLog);
                        Logger logger = new Logger(_configuration);
                        logger.ErrorLogData(null, errors[0]);
                        }
                    else
                    {
                        int countPRORQD = 0;
                        int countPRORWD = 0;
                        int countCUGRIN = 0;
                        int countPROMST = 0;
                        foreach (var promotiondata in promotionJsonEntities.payload)
                        {

                            var CUGRHD = promotiondata.CUGRHD;
                            var MAGRHD = promotiondata.MAGRHD;
                            var PRODHDR = promotiondata.PRODHDR;
                            foreach (var prodhdr in PRODHDR)
                            {
                                countPROMST++;
                                if (PRODHDR == null || PRODHDR.Count == 0 || string.IsNullOrEmpty(prodhdr.PromotionID))
                                {
                                    returnData.Add("PRODHDR", 0);
                                    var errorLog = new SAPErrorLogEntity();
                                    errorLog.PipeLineName = "Promotion";
                                    errorLog.FileName = blobDetails.FileName;
                                    errorLog.ErrorMessage = "Required fields are null";
                                    promotionData.SaveErrorLogData(errorLog);
                                    Logger logger = new Logger(_configuration);
                                    logger.ErrorLogData(null, "Required fields are null");
                                    }
                                else
                                {
                                    //insert to 7 table
                                    if (PRODHDR.Count != 0)
                                    {
                                        var return_PRODHDR = promotionData.SavePromotionMasterDetailsdata(prodhdr);
                                        returnData.Add("PRODHDR" + countPROMST, return_PRODHDR);

                                        if (prodhdr.PRORQD != null)
                                        {
                                            if (prodhdr.PRORQD.Count != 0)
                                            {

                                                foreach (var dataPRORQD in prodhdr.PRORQD)
                                                {
                                                    if (!string.IsNullOrEmpty(dataPRORQD.RequirementId))
                                                    {
                                                        countPRORQD++;
                                                        dataPRORQD.PromotionID = prodhdr.PromotionID;
                                                        var return_PRORQD = promotionData.SavePromotionRequirementDetailsdata(dataPRORQD);
                                                        returnData.Add("PRORQD" + countPRORQD, return_PRORQD);
                                                    }
                                                }
                                            }
                                        }

                                        if (prodhdr.PRORWD != null)
                                        {
                                            if (prodhdr.PRORWD.Count != 0)
                                            {

                                                foreach (var dataPRORWD in prodhdr.PRORWD)
                                                {
                                                    if (!string.IsNullOrEmpty(dataPRORWD.PromoRewardID))
                                                    {
                                                        countPRORWD++;
                                                        dataPRORWD.PromotionID = prodhdr.PromotionID;
                                                        var return_PRORWD = promotionData.SavePromotionRewardDetailsdata(dataPRORWD);
                                                        returnData.Add("PRORWD" + countPRORWD, return_PRORWD);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    if (CUGRHD != null)
                                    {
                                        if (CUGRHD.Count != 0)
                                        {
                                            foreach (var dataCUGRHD in CUGRHD)
                                            {
                                                //if (!string.IsNullOrEmpty(dataCUGRHD.CustomerGrouping))
                                                //{
                                                //    countCUGRHD++;
                                                //    dataCUGRHD.PromotionID = prodhdr.PromotionID;
                                                //    var return_CUGRHD = promotionData.SaveCustomerGroupPromotionsdata(dataCUGRHD);
                                                //    returnData.Add("CUGRHD" + countCUGRHD, return_CUGRHD);
                                                if (dataCUGRHD.CustomerNumber != null)
                                                {
                                                    if (dataCUGRHD.CustomerNumber.Count != 0)
                                                    {

                                                        var listCustomerPromotionDetails = new List<SAPCustomerPromotionDetailsEntity>();
                                                        var dataTable = new DataTable();
                                                        dataTable.Columns.Add("PromotionID");
                                                        dataTable.Columns.Add("CustomerNumber");
                                                        dataTable.Columns.Add("CustomerGrouping");
                                                        foreach (var dataCUGRIN in dataCUGRHD.CustomerNumber)
                                                        {
                                                            if (!string.IsNullOrEmpty(dataCUGRIN))
                                                            {
                                                                dataTable.Rows.Add(prodhdr.PromotionID, dataCUGRIN, dataCUGRHD.CustomerGrouping);
                                                            }
                                                        }
                                                        dataTable.AcceptChanges();
                                                        if (dataTable.Rows.Count != 0)
                                                        {
                                                            var return_CUGRIN = promotionData.SaveCustomerPromotionDetailsdata(dataTable);
                                                            returnData.Add("CUGRIN" + countCUGRIN, return_CUGRIN);
                                                            countCUGRIN++;
                                                        }
                                                    }
                                                }
                                                //}

                                            }
                                        }
                                    }

                                    //not complete
                                    //if (MAGRHD != null)
                                    //{
                                    //    if (MAGRHD.Count != 0)
                                    //    {
                                    //        int countMAGRHD = 0;
                                    //        foreach (var dataMAGRHD in MAGRHD)
                                    //        {
                                    //            if (!string.IsNullOrEmpty(dataMAGRHD.MAGRIN[0].MaterialGrouping))
                                    //            {
                                    //                dataMAGRHD.PromotionID = prodhdr.PromotionID;
                                    //                countMAGRHD++;
                                    //                var return_MAGRHD = promotionData.SaveMaterialGroupPromotionsdata(dataMAGRHD);
                                    //                returnData.Add("MAGRHD" + countMAGRHD, return_MAGRHD);

                                    //                if (dataMAGRHD.MAGRIN != null)
                                    //                {
                                    //                    if (dataMAGRHD.MAGRIN.Count != 0)
                                    //                    {
                                    //                        int countMAGRIN = 0;
                                    //                        foreach (var dataMAGRIN in dataMAGRHD.MAGRIN)
                                    //                        {
                                    //                            if (!string.IsNullOrEmpty(dataMAGRIN.MaterialNumber))
                                    //                            {
                                    //                                countMAGRIN++;
                                    //                                dataMAGRIN.PromotionID = prodhdr.PromotionID;
                                    //                                var return_MAGRIN = promotionData.SaveMaterialPromotionDetailsdata(dataMAGRIN);
                                    //                                returnData.Add("MAGRIN" + countMAGRIN, return_MAGRIN);
                                    //                            }
                                    //                        }

                                    //                    }
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                            }

                            //need to check all the values other than 0
                            foreach (var returnvalue in returnData)
                            {
                                if (returnvalue.Value == 0)
                                {
                                    blobDetails.Status = "Error";
                                    var errorLog = new SAPErrorLogEntity();
                                    errorLog.PipeLineName = "Promotion";
                                    errorLog.FileName = blobDetails.FileName;
                                    errorLog.ParentNodeName = returnvalue.Key;
                                    promotionData.SaveErrorLogData(errorLog);
                                    Logger logger = new Logger(_configuration);
                                    logger.ErrorLogData(null, errorLog.ErrorMessage);
                                    blobDetails.Status = "Error";
                                    break;
                                }
                                else
                                {
                                    blobDetails.Status = "Success";
                                }
                            }
                        }
                    }
                }

                var destDirectory = "source/process/promotion/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
                MoveFile(blobDetails, container, destDirectory);
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "CheckRequiredFields";
                errorLog.ErrorMessage = ex.Message;
                promotionData.SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, errorLog.ErrorMessage);
                }
        }

        public void MoveFile(SAPBlobEntity blob, CloudBlobContainer destContainer, string destDirectory)
        {
            CloudBlockBlob destBlob;
            try
            {
                if (blob.Blob == null)
                    throw new Exception("Source blob cannot be null.");

                if (!destContainer.Exists())
                    throw new Exception("Destination container does not exist.");

                //Copy source blob to destination container
                string name = blob.FileName;
                if (destDirectory != "" && blob.Status == "Success")
                    destBlob = destContainer.GetBlockBlobReference(destDirectory + "\\Success\\" + name);
                else
                    destBlob = destContainer.GetBlockBlobReference(destDirectory + "\\Error\\" + name);

                destBlob.StartCopy(blob.Blob);
                //remove source blob after copy is done.
                blob.Blob.Delete();
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.FileName = blob.FileName;
                errorLog.ParentNodeName = "Promotion move";
                errorLog.ErrorMessage = ex.Message;
                promotionData.SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                }
        }
    }
}
