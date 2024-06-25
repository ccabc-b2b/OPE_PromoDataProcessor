using System.Collections.Generic;

namespace SAPPromotion
    {
    public class SAPCustomerGroupPromotionEntity
    {
        public string PromotionID { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Division { get; set; }
        public string CustomerGroupingDescription { get; set; }
        public string CustInactiveIndicator { get; set; }
        public string CRMMarketingTargetGroupID { get; set; }
        public string CustomerGrouping { get; set; }
        public string ObjectCreatedTime { get; set; }
        public string LastObjectChangedTime { get; set; }
        public string UpdateIndicator { get; set; }
        public List<string> CustomerNumber { get; set; }
    }
}
