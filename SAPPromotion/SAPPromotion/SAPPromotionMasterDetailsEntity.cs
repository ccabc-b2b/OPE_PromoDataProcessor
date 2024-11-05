using System;
using System.Collections.Generic;

namespace SAPPromotion
    {
    public class SAPPromotionMasterDetailsEntity
    {
        public string PromotionID { get; set; }
        public string SalesOrganization_HD { get; set; }
        public string DistributionChannel_HD { get; set; }
        public string Division_HD { get; set; }
        public string PromoMaintenanceSource { get; set; }
        public string PromotionDescription { get; set; }
        public string PromoInactiveIndicator { get; set; }
        public string PromoExternalRef { get; set; }
        public string ExtendedDescription { get; set; }
        public string PromotionType { get; set; }
        public string PromotionCategory { get; set; }
        public string PromotionCalculationType { get; set; }
        public string ConditionType { get; set; }
        public string AgreementValidToDate { get; set; }
        public string AgreementValidFromDate { get; set; }
        public string CustAssortmentHandling { get; set; }
        public string CustomerInHierarchy { get; set; }
        public string ScaleType { get; set; }
        public string ScaleBaseType { get; set; }
        public string ReqUom { get; set; }
        public string ReqCurrency { get; set; }
        public string MinQty { get; set; }
        public string MinValue { get; set; }
        public string PerUom { get; set; }
        public string PerCurrency { get; set; }
        public string RewardUom { get; set; }
        public string RewardCurrency { get; set; }
        public string MaxQty { get; set; }
        public string MaxValue { get; set; }
        public string CapQty { get; set; }
        public string CapValue { get; set; }
        public string ConditionCreatedIndicator { get; set; }
        public string EntryTime { get; set; }
        public string RecordCreatedDate { get; set; }
        public string ObjectCreatorName { get; set; }
        public string ObjectChangedTime { get; set; }
        public string ObjectChangedDate { get; set; }
        public string ObjectChangerName { get; set; }
        public string BundlePromotionFlag { get; set; }
        public List<SAPPromotionRequirementsDetailsEntity> PRORQD { get; set; }
        public List<SAPPromotionRewardDetailsEntity> PRORWD { get; set; }
        public int IsSlab { get; set; }
        public List<SAPPromotionSlabDetailsEntity> Slabs { get; set; }
        }
}
