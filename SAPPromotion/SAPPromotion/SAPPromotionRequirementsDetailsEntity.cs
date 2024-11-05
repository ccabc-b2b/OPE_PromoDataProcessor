namespace SAPPromotion
    {
    public class SAPPromotionRequirementsDetailsEntity
    {
        public string PromotionID { get; set; }
        public string RequirementId { get; set; }
        public string MaterialGroupID { get; set; }
        public string MaterialNumber { get; set; }
        public string ProductSegmentID { get; set; }
        public string RequirementQty { get; set; }
        public string RequirementValue { get; set; }
        public string FromQTY { get; set; }
        public string ToQTY { get; set; }
        public string ActiveFrom{ get; set; }
        public string ActiveTo { get; set; }

        }
}
