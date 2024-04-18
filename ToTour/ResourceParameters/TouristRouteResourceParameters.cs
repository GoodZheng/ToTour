using System.Text.RegularExpressions;

namespace ToTour.ResourceParameters
{
    public class TouristRouteResourceParameters
    {

        public string? OrderBy { get; set; } //排序参数

        public string? Keyword { get; set; }


        public string? OperatorType { get; set; }
        public int? RatingValue { get; set; }

        private string? _rating;
        public string? Rating
        {
            get { return _rating; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        OperatorType = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }
                }
                _rating = value;
            }
        }

        public string? Fields {  get; set; }
    }
}
