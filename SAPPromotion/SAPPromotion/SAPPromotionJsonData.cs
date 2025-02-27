﻿using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SAPPromotion
    {
    public class SAPPromotionJsonData
        {
        readonly string containerName = Properties.Settings.Default.ContainerName;
        readonly string blobDirectoryPrefix = Properties.Settings.Default.BlobDirectoryPrefix;
        readonly string blobDirectoryCustomerPromoPrefix = Properties.Settings.Default.BlobDirectoryCustomerPromoPrefix;
        readonly string destblobDirectoryPrefix = Properties.Settings.Default.DestDirectory;
        readonly string customerPromoDestDirectoryPrefix = Properties.Settings.Default.CustomerPromoDestDirectory;
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
                List<SAPBlobEntity> CustomerPromoBlobList = new List<SAPBlobEntity>();
                var storageKey = _configuration["StorageKey"];

                var storageAccount = CloudStorageAccount.Parse(storageKey);
                var myClient = storageAccount.CreateCloudBlobClient();
                var container = myClient.GetContainerReference(containerName);

                var list = container.ListBlobs().OfType<CloudBlobDirectory>().ToList();
                var blobListDirectory = list[0].ListBlobs().OfType<CloudBlobDirectory>().ToList();

                foreach (var blobDirectory in blobListDirectory)
                    {
                    if (blobDirectory.Prefix == blobDirectoryPrefix || blobDirectory.Prefix == blobDirectoryCustomerPromoPrefix)
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

                            if (blobDirectory.Prefix == blobDirectoryPrefix)
                                {
                                blobList.Add(blobDetails);
                                }
                            else
                                {
                                CustomerPromoBlobList.Add(blobDetails);
                                }
                            // blobList.Add(blobDetails);
                            }

                        if (blobList != null)
                            {
                            blobList.OrderByDescending(x => x.FileCreatedDate.Date).ThenByDescending(x => x.FileCreatedDate.TimeOfDay).ToList();
                            }

                        if (CustomerPromoBlobList != null)
                            {
                            CustomerPromoBlobList.OrderByDescending(x => x.FileCreatedDate.Date).ThenByDescending(x => x.FileCreatedDate.TimeOfDay).ToList();
                            }
                        }
                    }

                foreach (var blobDetails in blobList)
                    {
                    CheckRequiredFields(blobDetails, container);
                    }

                foreach (var blobDetails in CustomerPromoBlobList)
                    {
                    CheckCustomerPromoRequiredFields(blobDetails, container);
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
            catch (Exception ex)
                {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ErrorMessage = ex.Message;
                promotionData.SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                }
            }

        public void CheckCustomerPromoRequiredFields(SAPBlobEntity blobDetails, CloudBlobContainer container)
            {
            try
                {
                List<string> errors = new List<string>();
                if (string.IsNullOrEmpty(blobDetails.FileData))
                    {
                    blobDetails.Status = "Error";
                    var errorLog = new SAPErrorLogEntity();
                    errorLog.PipeLineName = "CustomerPromotion";
                    errorLog.FileName = blobDetails.FileName;
                    errorLog.ErrorMessage = "File is empty";
                    promotionData.SaveErrorLogData(errorLog);
                    Logger logger = new Logger(_configuration);
                    logger.ErrorLogData(null, "File is empty");
                    }
                else
                    {
                    CustomerPromotionJsonEntity customer_promotionJsonEntities = JsonConvert.DeserializeObject<CustomerPromotionJsonEntity>(blobDetails.FileData, new JsonSerializerSettings
                        {
                        Error = delegate (object sender, ErrorEventArgs args)
                        {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;

                            },
                        Converters = { new IsoDateTimeConverter() }
                        });

                    Dictionary<string, int> returnData = new Dictionary<string, int>();
                    if (customer_promotionJsonEntities == null)
                        {
                        returnData.Add("CustomerPromotion", 0);
                        var errorLog = new SAPErrorLogEntity();
                        errorLog.PipeLineName = "CustomerPromotion";
                        errorLog.FileName = blobDetails.FileName;
                        errorLog.ErrorMessage = errors[0];
                        //SaveErrorLogData(errorLog);
                        Logger logger = new Logger(_configuration);
                        logger.ErrorLogData(null, errors[0]);
                        }
                    else
                        {
                        //foreach (var payload in customer_promotionJsonEntities.payload)
                        Parallel.ForEach(customer_promotionJsonEntities.payload, payload =>
                        {
                            if (payload.customerC == null)
                                {
                                returnData.Add("CustomerNumber is null", 0);
                                }
                            else if (payload.dealNoC == null)
                                {
                                returnData.Add("PromotionID is null", 0);
                                }
                            else
                                {
                                //var dataTable = new DataTable();
                                //dataTable.Columns.Add("PromotionID");
                                //dataTable.Columns.Add("CustomerNumber");
                                //dataTable.Columns.Add("CustomerGrouping");

                                //dataTable.Rows.Add(payload.dealNoC, payload.customerC, null);

                                //dataTable.AcceptChanges();
                                //if (dataTable.Rows.Count > 0)
                                //    {
                                SAPCustomerPromotionDetailsEntity customerPromotionDetailsEntity= new SAPCustomerPromotionDetailsEntity();
                                customerPromotionDetailsEntity.PromotionID = payload.dealNoC;
                                customerPromotionDetailsEntity.CustomerNumber = payload.customerC;
                                customerPromotionDetailsEntity.CustomerGrouping = payload.salesRouteC;
                                var return_CustomerPromo = promotionData.SaveCustomerPromotionDetailsdata(customerPromotionDetailsEntity);
                                if (!returnData.ContainsKey("CustomerPromotion" + payload.customerC + "_" + payload.dealNoC))
                                    {
                                    returnData.Add("CustomerPromotion" + payload.customerC + "_" + payload.dealNoC, return_CustomerPromo);
                                    }
                                    //}
                                }
                        }
                        );
                        }

                    //foreach (var returnvalue in returnData)
                    //    {
                    //    if (returnvalue.Value == 0)
                    //        {
                    //        blobDetails.Status = "Error";
                    //        var errorLog2 = new SAPErrorLogEntity();
                    //        errorLog2.PipeLineName = "CustomerPromotion";
                    //        errorLog2.FileName = blobDetails.FileName;
                    //        errorLog2.ParentNodeName = returnvalue.Key;
                    //        Logger logger = new Logger(_configuration);
                    //        logger.ErrorLogData(null, errors[0]);
                    //        //SaveErrorLogData(errorLog2);
                    //        // break;
                    //        }
                    //    else
                    //        {
                    //        blobDetails.Status = "Success";
                    //        }
                    //    }


                    bool hasError = returnData.Any(returnvalue => returnvalue.Value == 0);

                    if (hasError)
                        {
                        var errorEntries = returnData.Where(returnvalue => returnvalue.Value == 0);

                        foreach (var entry in errorEntries)
                            {
                            var errorLog2 = new SAPErrorLogEntity
                                {
                                PipeLineName = "CustomerPromotion",
                                FileName = blobDetails.FileName,
                                ParentNodeName = entry.Key
                                };

                            Logger logger = new Logger(_configuration);
                            logger.ErrorLogData(null,"Error Found in file "+blobDetails.FileName);
                            // Uncomment the following line if you want to save the error log data
                            // SaveErrorLogData(errorLog2);
                            }

                        blobDetails.Status = "Error";
                        }
                    else
                        {
                        blobDetails.Status = "Success";
                        }
                    }
                var destDirectory = "source/process/customer-promotion/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
                MoveFile(blobDetails, container, destDirectory);
                }
            catch (Exception ex)
                {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "CustomerPromotion";
                errorLog.ParentNodeName = "CheckCustomerPromoRequiredFields";
                errorLog.ErrorMessage = ex.Message;
                promotionData.SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, errorLog.ErrorMessage);
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
                    logger.ErrorLogData(null, "File is empty");
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
                        var rewardId = 1;
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

                                        var dataPROMGR = new SAPPromotionMaterialGroupMasterDetails();

                                        if (prodhdr.PRORQD != null && prodhdr.PRORQD.Count != 0)
                                            {                                           
                                                foreach (var dataPRORQD in prodhdr.PRORQD)
                                                    {
                                                    if (!string.IsNullOrEmpty(dataPRORQD.RequirementId))
                                                        {
                                                        countPRORQD++;
                                                        dataPRORQD.PromotionID = prodhdr.PromotionID;
                                                        var return_PRORQD = promotionData.SavePromotionRequirementDetailsdata(dataPRORQD);

                                                        dataPROMGR.MaterialNumber = dataPRORQD.MaterialNumber;
                                                        dataPROMGR.MaterialGroup = dataPRORQD.MaterialGroupID;
                                                        dataPROMGR.GroupType = "REQ";

                                                        var return_promgr = promotionData.SavePromotionMaterialGroupMasterDetails(dataPROMGR);

                                                        returnData.Add("PRORQD" + countPRORQD, return_PRORQD);
                                                    }
                                                }
                                            }

                                        if (prodhdr.PRORWD != null && prodhdr.PRORWD.Count != 0)
                                            {                                          
                                                foreach (var dataPRORWD in prodhdr.PRORWD)
                                                    {
                                                    if (!string.IsNullOrEmpty(dataPRORWD.PromoRewardID))
                                                        {
                                                        countPRORWD++;
                                                        dataPRORWD.PromotionID = prodhdr.PromotionID;
                                                        var return_PRORWD = promotionData.SavePromotionRewardDetailsdata(dataPRORWD);

                                                       dataPROMGR.MaterialNumber = dataPRORWD.MaterialNumber;
                                                       dataPROMGR.MaterialGroup = dataPRORWD.MaterialGroupID;
                                                       dataPROMGR.GroupType = "REW";

                                                       var return_promgr = promotionData.SavePromotionMaterialGroupMasterDetails(dataPROMGR);
                                                       returnData.Add("PRORWD" + countPRORWD, return_PRORWD);
                                                        }
                                                    }
                                            }

                                        if (prodhdr.IsSlab != 0 && prodhdr.Slabs != null)
                                            {
                                            if (prodhdr.Slabs.REQ.Count != 0)
                                                {
                                                foreach (var slabDataREQ in prodhdr.Slabs.REQ)
                                                    {
                                                    countPRORQD++;
                                                    var dataPRORQD = new SAPPromotionRequirementsDetailsEntity();
                                                    dataPRORQD.PromotionID = slabDataREQ.deal;
                                                    dataPRORQD.RequirementId = slabDataREQ.itemn;
                                                    dataPRORQD.MaterialGroupID = slabDataREQ.reqmgroup;
                                                    dataPRORQD.MaterialNumber = "NULL";
                                                    dataPRORQD.FromQTY = slabDataREQ.fromqty;
                                                    dataPRORQD.ToQTY = slabDataREQ.toqty;
                                                    dataPRORQD.ActiveFrom = slabDataREQ.activefrom;
                                                    dataPRORQD.ActiveTo = slabDataREQ.activeto;
                                                    var return_PRORQD = promotionData.SavePromotionRequirementDetailsdata(dataPRORQD);
                                                    }
                                                }
                                            if (prodhdr.Slabs.REW.Count != 0)
                                                {
                                                foreach (var slabDataREW in prodhdr.Slabs.REW)
                                                    {
                                                    countPRORWD++;
                                                    var dataPRORWD = new SAPPromotionRewardDetailsEntity();
                                                    dataPRORWD.PromotionID = slabDataREW.deal;
                                                    dataPRORWD.PromoRewardID = rewardId.ToString();
                                                    dataPRORWD.RequirementId_RWD = slabDataREW.itemn;
                                                    dataPRORWD.MaterialGroupID = slabDataREW.rewmgroup;
                                                    dataPRORWD.MaterialNumber = "NULL";
                                                    dataPRORWD.RewardValue = slabDataREW.discountvalue;
                                                    dataPRORWD.RewardPercentage = slabDataREW.discountrate;
                                                    dataPRORWD.RewardQty = slabDataREW.discountqty;
                                                    dataPRORWD.DiscountType = slabDataREW.discounttype;
                                                    dataPRORWD.FreeGoodQTY = slabDataREW.freegoodsqty;
                                                    var return_PRORWD = promotionData.SavePromotionRewardDetailsdata(dataPRORWD);
                                                    returnData.Add("PRORWD" + countPRORWD, return_PRORWD);
                                                    rewardId++;
                                                    }
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
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(null, "File " + blob.FileName + " processed");
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
