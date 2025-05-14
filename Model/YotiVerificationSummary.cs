namespace Calyx_Solutions.Model
{
    public class YotiVerificationSummary
    {
        public string SessionId { get; set; }
        public string UserTrackingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OverallStatus { get; set; }
        public DocumentCheck Document { get; set; }
        public LivenessCheck Liveness { get; set; }
        public FaceMatchCheck FaceMatch { get; set; }
        public WatchlistCheck Watchlist { get; set; }
        public TextDataCheck TextData { get; set; }
    }

    public class DocumentCheck
    {
        // Document Authenticity Check
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public List<Breakdown> Breakdowns { get; set; }

        // Document Details
        public string DocumentType { get; set; }
        public string IssuingCountry { get; set; }
        public string DocumentId { get; set; }
        public List<DocumentPage> Pages { get; set; }
        public Image DocumentPhoto { get; set; }

        // Extracted Document Fields
        public string FullName { get; set; }
        public string GivenNames { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string NamePrefix { get; set; }
        public string DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string PlaceOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string Gender { get; set; }
        public string DocumentNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string DateOfIssue { get; set; }
        public string IssuingAuthority { get; set; }
    }

    public class LivenessCheck
    {
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public List<Breakdown> Breakdowns { get; set; }
        public List<Image> Images { get; set; }
    }

    public class FaceMatchCheck
    {
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public double ConfidenceScore { get; set; }
        public List<Breakdown> Breakdowns { get; set; }
        public List<Image> FaceCaptures { get; set; }
    }

    public class WatchlistCheck
    {
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public List<Breakdown> Breakdowns { get; set; }
    }

    public class TextDataCheck
    {
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public Dictionary<string, string> DocumentFields { get; set; }
    }

    public class Breakdown
    {
        public string SubCheck { get; set; }
        public string Result { get; set; }
        public List<Detail> Details { get; set; }
    }

    public class Detail
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class DocumentPage
    {
        public string CaptureMethod { get; set; }
        public string MediaId { get; set; }
        public string Base64Image { get; set; }
    }

    public class Image
    {
        public string MediaId { get; set; }
        public string Base64Image { get; set; }
    }
}
