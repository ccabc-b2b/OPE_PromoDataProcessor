using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace SAPPromotion
    {
    public class SAPPromotionData
    {
        private static IConfiguration _configuration;
        public SAPPromotionData(IConfiguration configuration)
            {
            _configuration = configuration;
            }
      
        public int SavePromotionMasterDetailsdata(SAPPromotionMasterDetailsEntity promotionMasterDetailsdata)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("PromotionMasterDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", promotionMasterDetailsdata.PromotionID);
                cmd.Parameters.AddWithValue("@PromotionDescription", promotionMasterDetailsdata.PromotionDescription);
                cmd.Parameters.AddWithValue("@PromoInactiveIndicator", promotionMasterDetailsdata.PromoInactiveIndicator);
                cmd.Parameters.AddWithValue("@PromoExternalRef", promotionMasterDetailsdata.PromoExternalRef);
                cmd.Parameters.AddWithValue("@ExtendedDescription", promotionMasterDetailsdata.ExtendedDescription);
                cmd.Parameters.AddWithValue("@PromotionType", promotionMasterDetailsdata.PromotionType.TrimStart('0'));
                cmd.Parameters.AddWithValue("@PromotionCategory", promotionMasterDetailsdata.PromotionCategory);
                cmd.Parameters.AddWithValue("@PromotionCalculationType", promotionMasterDetailsdata.PromotionCalculationType);
                cmd.Parameters.AddWithValue("@ConditionType", promotionMasterDetailsdata.ConditionType);
                cmd.Parameters.AddWithValue("@AgreementValidToDate", DateTime.ParseExact(promotionMasterDetailsdata.AgreementValidToDate,"yyyy-MM-dd",CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@AgreementValidFromDate", DateTime.ParseExact(promotionMasterDetailsdata.AgreementValidFromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));             
                cmd.Parameters.AddWithValue("@PromoMaintenanceSource", promotionMasterDetailsdata.PromoMaintenanceSource);
                cmd.Parameters.AddWithValue("@CustAssortmentHandling", promotionMasterDetailsdata.CustAssortmentHandling);
                cmd.Parameters.AddWithValue("@CustomerInHierarchy", promotionMasterDetailsdata.CustomerInHierarchy);
                cmd.Parameters.AddWithValue("@ScaleType", promotionMasterDetailsdata.ScaleType);
                cmd.Parameters.AddWithValue("@ScaleBaseType", promotionMasterDetailsdata.ScaleBaseType);
                cmd.Parameters.AddWithValue("@ReqUom", promotionMasterDetailsdata.ReqUom);
                cmd.Parameters.AddWithValue("@ReqCurrency", promotionMasterDetailsdata.ReqCurrency);
                cmd.Parameters.AddWithValue("@MinQty", promotionMasterDetailsdata.MinQty);
                cmd.Parameters.AddWithValue("@MinValue", promotionMasterDetailsdata.MinValue);
                cmd.Parameters.AddWithValue("@PerUom", promotionMasterDetailsdata.PerUom);
                cmd.Parameters.AddWithValue("@PerCurrency", promotionMasterDetailsdata.PerCurrency);
                cmd.Parameters.AddWithValue("@RewardUom", promotionMasterDetailsdata.RewardUom);
                cmd.Parameters.AddWithValue("@RewardCurrency", promotionMasterDetailsdata.RewardCurrency);
                cmd.Parameters.AddWithValue("@MaxQty", promotionMasterDetailsdata.MaxQty);
                cmd.Parameters.AddWithValue("@MaxValue", promotionMasterDetailsdata.MaxValue);
                cmd.Parameters.AddWithValue("@CapQty", promotionMasterDetailsdata.CapQty);
                cmd.Parameters.AddWithValue("@CapValue", promotionMasterDetailsdata.CapValue);
                cmd.Parameters.AddWithValue("@ConditionCreatedIndicator", promotionMasterDetailsdata.ConditionCreatedIndicator);
                cmd.Parameters.AddWithValue("@EntryTime", promotionMasterDetailsdata.EntryTime);
                cmd.Parameters.AddWithValue("@ObjectChangedTime", promotionMasterDetailsdata.ObjectChangedTime);
                cmd.Parameters.AddWithValue("@RecordCreatedDate", promotionMasterDetailsdata.RecordCreatedDate);
                cmd.Parameters.AddWithValue("@ObjectCreatorName", promotionMasterDetailsdata.ObjectCreatorName);
                cmd.Parameters.AddWithValue("@ObjectChangedDate", promotionMasterDetailsdata.ObjectChangedDate);
                cmd.Parameters.AddWithValue("@ObjectChangerName", promotionMasterDetailsdata.ObjectChangerName);
                cmd.Parameters.AddWithValue("@BundlePromotionFlag", promotionMasterDetailsdata.ObjectChangerName);
                cmd.Parameters.AddWithValue("@SalesOrganization_HD", promotionMasterDetailsdata.SalesOrganization_HD);
                cmd.Parameters.AddWithValue("@DistributionChannel_HD", promotionMasterDetailsdata.DistributionChannel_HD);
                cmd.Parameters.AddWithValue("@Division_HD", promotionMasterDetailsdata.Division_HD);

                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "PromotionMasterDetails_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SavePromotionRequirementDetailsdata(SAPPromotionRequirementsDetailsEntity promotionRequirementDetailsdata)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("PromotionRequirementDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", promotionRequirementDetailsdata.PromotionID);
                cmd.Parameters.AddWithValue("@RequirementId", promotionRequirementDetailsdata.RequirementId);
                cmd.Parameters.AddWithValue("@MaterialGroupID", promotionRequirementDetailsdata.MaterialGroupID);
                cmd.Parameters.AddWithValue("@MaterialNumber ", promotionRequirementDetailsdata.MaterialNumber);
                cmd.Parameters.AddWithValue("@ProductSegmentID", promotionRequirementDetailsdata.ProductSegmentID);
                cmd.Parameters.AddWithValue("@RequirementQty ", promotionRequirementDetailsdata.RequirementQty);
                cmd.Parameters.AddWithValue("@RequirementValue", promotionRequirementDetailsdata.RequirementValue);
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "PromotionRequirementDetails_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SavePromotionRewardDetailsdata(SAPPromotionRewardDetailsEntity promotionRewardDetailsdata)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("PromotionRewardDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", promotionRewardDetailsdata.PromotionID);
                cmd.Parameters.AddWithValue("@RequirementId_RWD", promotionRewardDetailsdata.RequirementId_RWD);
                cmd.Parameters.AddWithValue("@PromoRewardID", promotionRewardDetailsdata.PromoRewardID);
                cmd.Parameters.AddWithValue("@MaterialNumber", promotionRewardDetailsdata.MaterialNumber);
                cmd.Parameters.AddWithValue("@MaterialGroupID", promotionRewardDetailsdata.MaterialGroupID);
                cmd.Parameters.AddWithValue("@RequirementQty_RWD", promotionRewardDetailsdata.RequirementQty_RWD);
                cmd.Parameters.AddWithValue("@RequirementValue_RWD", promotionRewardDetailsdata.RequirementValue_RWD);
                cmd.Parameters.AddWithValue("@RewardQty", promotionRewardDetailsdata.RewardQty);
                cmd.Parameters.AddWithValue("@RewardValue", promotionRewardDetailsdata.RewardValue);
                cmd.Parameters.AddWithValue("@RewardPercentage", promotionRewardDetailsdata.RewardPercentage);
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "PromotionRewardDetails_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SaveCustomerGroupPromotionsdata(SAPCustomerGroupPromotionEntity customerGroupPromotionsdata)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("CustomerGroupPromotions_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", customerGroupPromotionsdata.PromotionID);
                cmd.Parameters.AddWithValue("@SalesOrganization", customerGroupPromotionsdata.SalesOrganization);
                cmd.Parameters.AddWithValue("@DistributionChannel", customerGroupPromotionsdata.DistributionChannel);
                cmd.Parameters.AddWithValue("@Division", customerGroupPromotionsdata.Division);
                cmd.Parameters.AddWithValue("@CustomerGroupingDescription", customerGroupPromotionsdata.CustomerGroupingDescription);
                cmd.Parameters.AddWithValue("@CustInactiveIndicator", customerGroupPromotionsdata.CustInactiveIndicator);
                cmd.Parameters.AddWithValue("@CRMMarketingTargetGroupID", customerGroupPromotionsdata.CRMMarketingTargetGroupID);
                cmd.Parameters.AddWithValue("@CustomerGrouping", customerGroupPromotionsdata.CustomerGrouping);
                cmd.Parameters.AddWithValue("@ObjectCreatedTime", customerGroupPromotionsdata.ObjectCreatedTime);
                cmd.Parameters.AddWithValue("@LastObjectChangedTime", customerGroupPromotionsdata.LastObjectChangedTime);
                cmd.Parameters.AddWithValue("@UpdateIndicator", customerGroupPromotionsdata.UpdateIndicator);
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "CustomerGroupPromotions_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SaveCustomerPromotionDetailsdata(DataTable customerPromotionDetailsData)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("CustomerPromotionDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerPromotionDetailTableData", customerPromotionDetailsData);
                cmd.Parameters.AddWithValue("@PromotionID", customerPromotionDetailsData.Rows[0].Field<string>(0));
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "CustomerPromotionDetails_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SaveMaterialGroupPromotionsdata(SAPMaterialGroupPromotionEntity materialGroupPromotionsData)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("MaterialGroupPromotions_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", materialGroupPromotionsData.PromotionID);
                cmd.Parameters.AddWithValue("@MaterialGroupingDescription", materialGroupPromotionsData.MaterialGroupingDescription);
                cmd.Parameters.AddWithValue("@MaterialGrpInactiveIndicator", materialGroupPromotionsData.MaterialGrpInactiveIndicator);
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "MaterialGroupPromotions_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public int SaveMaterialPromotionDetailsdata(SAPMaterialPromotionDetailsEntity materialPromotionDetailsdata)
        {
            try
            {

                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("MaterialPromotionDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PromotionID", materialPromotionDetailsdata.PromotionID);
                cmd.Parameters.AddWithValue("@MaterialID", materialPromotionDetailsdata.MaterialNumber);
                cmd.Parameters.AddWithValue("@MaterialGrouping", materialPromotionDetailsdata.MaterialGrouping);
                cmd.Parameters.Add("@returnObj", SqlDbType.BigInt);
                cmd.Parameters["@returnObj"].Direction = ParameterDirection.Output;
                con.Open();
                int k = cmd.ExecuteNonQuery();
                con.Close();
                if (k != 0)
                {
                    return k;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Promotion";
                errorLog.ParentNodeName = "MaterialPromotionDetails_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                return 0;
            }
        }
        public void SaveErrorLogData(SAPErrorLogEntity errorLogData)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("ErrorLogDetails_save", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PipeLineName", errorLogData.PipeLineName);
                cmd.Parameters.AddWithValue("@FileName", errorLogData.FileName);
                cmd.Parameters.AddWithValue("@ParentNodeName", errorLogData.ParentNodeName);
                cmd.Parameters.AddWithValue("@ErrorMessage", errorLogData.ErrorMessage);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();


            }
            catch (Exception ex)
            {
                Logger logger = new Logger(_configuration);
                logger.ErrorLogData(ex, ex.Message);
                }
        }
        public List<string> BulkSavePromotionRequirementDetailsdata(DataTable dt)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("ConditionItems_BulkSave", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@conditionItemsLst", SqlDbType.Structured)
                {
                    TypeName = "[Pricing].[UT_ConditionItems]",
                    Value = dt
                };
                cmd.Parameters.Add(param);
                con.Open();
                cmd.CommandTimeout = 0;
                SqlDataReader rdr = cmd.ExecuteReader();
                List<string> errorConditionRecNum = new List<string>();
                while (rdr.Read())
                {
                    errorConditionRecNum.Add(rdr["ConditionRecordNumber"].ToString());
                }
                con.Close();
                return errorConditionRecNum;
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Pricing";
                errorLog.ParentNodeName = "ConditionItems_BulkSave";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                return new List<string>();
            }
        }
        public List<string> BulkSavePromotionRewardDetailsdata(DataTable dt)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration["DatabaseConnectionString"]);
                SqlCommand cmd = new SqlCommand("FilterSegments_BulkSave", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = new SqlParameter("@filterSegmentsLst", SqlDbType.Structured)
                {
                    TypeName = "[Pricing].[UT_FilterSegments]",
                    Value = dt
                };
                cmd.Parameters.Add(param);
                con.Open();
                cmd.CommandTimeout = 0;
                SqlDataReader rdr = cmd.ExecuteReader();
                List<string> errorConditionRecNum = new List<string>();
                while (rdr.Read())
                {
                    errorConditionRecNum.Add(rdr["ConditionRecordNumber"].ToString());
                }
                con.Close();
                return errorConditionRecNum;
            }
            catch (Exception ex)
            {
                var errorLog = new SAPErrorLogEntity();
                errorLog.PipeLineName = "Pricing";
                errorLog.ParentNodeName = "FilterSegments_save";
                errorLog.ErrorMessage = ex.Message;
                SaveErrorLogData(errorLog);
                return new List<string>();
            }
        }
    }
}
