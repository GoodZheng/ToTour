namespace ToTour.Dtos
{
    public class LinkDto
    {
        public string Href { get; set; } //url

        public string Rel { get; set; } //关系 描述url

        public string Method { get; set; } //http的方法

        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
